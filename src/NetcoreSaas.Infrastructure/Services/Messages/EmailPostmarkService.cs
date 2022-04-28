using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NetcoreSaas.Application.Extensions;
using NetcoreSaas.Application.Services.Messages;
using NetcoreSaas.Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetcoreSaas.Application.Dtos.Core.Emails;
using NetcoreSaas.Domain.Enums.App.Contracts;
using NetcoreSaas.Domain.Models.App.Contracts;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Core.Workspaces;
using PostmarkDotNet;
using PostmarkDotNet.Model;

namespace NetcoreSaas.Infrastructure.Services.Messages
{
    public class EmailPostmarkService : IEmailService
    {
        private readonly IConfiguration _conf;
        private readonly EmailSettings _emailSettings;
        private readonly PostmarkClient _postmarkClient;
        private readonly Dictionary<string, object> _postmarkGlobalVariables;

        public EmailPostmarkService(IOptions<EmailSettings> emailSettings, IConfiguration conf)
        {
            _emailSettings = emailSettings.Value;
            _conf = conf;

            var postmarkApiBaseUrl = "https://api.postmarkapp.com";
            if (_emailSettings.PostmarkServerToken == "POSTMARK_API_TEST")
                postmarkApiBaseUrl = "https://api.postmarkapp.com/email";

            _postmarkClient = new PostmarkClient(_emailSettings.PostmarkServerToken, postmarkApiBaseUrl);
            _postmarkGlobalVariables = new Dictionary<string, object>
            {
                {"product_url", _conf["App:URL"]},
                {"login_url", _conf["App:URL"] + "/login"},
                {"product_name", _conf["App:Name"]},
                {"support_email", _conf["App:SupportEmail"]},
                {"sender_name", _emailSettings.PostmarkSenderName},
                {"company_name", _conf["App:CompanyName"]},
                {"company_address", _conf["App:CompanyAddress"]},
            };
        }
        
        
        private List<EmailTemplateDto> LocalTemplates()
        {
            var templates = new List<EmailTemplateDto>();
            var mdFiles = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Emails"));
            if (mdFiles.Length > 0)
            {
                foreach (var file in mdFiles)
                {
                    var content = System.IO.File.ReadAllText(file).Split('`');

                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var alias = content[1];
                    var subject = content[3];
                    var body = content[5];

                    var template = new EmailTemplateDto()
                    {
                        Created = false,
                        Alias = alias,
                        Name = fileName,
                        Subject = subject,
                        HtmlBody = body,
                        Active = false,
                        Type = alias.StartsWith("layout-") ? TemplateType.Layout : TemplateType.Standard
                    };
                    templates.Add(template);
                }
            }

            return templates.OrderBy(f=>f.Name).ToList();
        }

        public async Task<IEnumerable<EmailTemplateDto>> GetAllTemplates()
        {
            var postmarkTemplates = await _postmarkClient.GetTemplatesAsync();
            if (postmarkTemplates.TotalCount == 0)
                return LocalTemplates();

            var templates = new List<EmailTemplateDto>();
            foreach (var templateBasicInfo in postmarkTemplates.Templates)
            {
                var postmarkTemplate = await _postmarkClient.GetTemplateAsync(templateBasicInfo.TemplateId);

                var template = new EmailTemplateDto()
                {
                    Created = true,
                    Alias = postmarkTemplate.Alias,
                    Name = postmarkTemplate.Name,
                    Subject = postmarkTemplate.Subject,
                    HtmlBody = postmarkTemplate.HtmlBody,
                    Active = postmarkTemplate.Active,
                    AssociatedServerId = postmarkTemplate.AssociatedServerId,
                    TemplateId = postmarkTemplate.TemplateId
                };
                templates.Add(template);
            }

            return templates.OrderBy(f=>f.Name).ToList();
        }

        public async Task<IEnumerable<EmailTemplateDto>> CreateTemplates()
        {
            var localTempaltes = LocalTemplates();

            if (!localTempaltes.Any())
                throw new Exception("There are no .md templates at /Emails folder");
            
            var postmarkTemplates = await _postmarkClient.GetTemplatesAsync();
            if (!postmarkTemplates.Templates.ToList().Exists(f => f.TemplateType == TemplateType.Layout))
            {
                var layout = localTempaltes.Find(f => f.Type == TemplateType.Layout);
                if (layout != null)
                {
                    await _postmarkClient.CreateTemplateAsync(layout.Name, layout.Subject, layout.HtmlBody, null, layout.Alias, TemplateType.Layout);
                }
            }
            foreach (var localTempalte in localTempaltes.Where(f=>f.Type != TemplateType.Layout))
            {
                if (postmarkTemplates.Templates.Count(f => f.Alias == localTempalte.Alias) == 0)
                {
                    await _postmarkClient.CreateTemplateAsync(localTempalte.Name, localTempalte.Subject, localTempalte.HtmlBody, null, localTempalte.Alias, TemplateType.Standard, "layout-basic");
                }
            }

            return await GetAllTemplates();
        }

        public async Task SendWelcome(User user)
        {
            var actionUrl = _conf["App:URL"] + $"/app";
            if (ProjectConfiguration.GlobalConfiguration.RequiresVerification)
                actionUrl = _conf["App:URL"] + $"/verify?e={user.Email}&t={user.Token}";
            
            var message = new TemplatedPostmarkMessage
            {
                From = _emailSettings.PostmarkSenderEmail,
                To = user.EmailTo(),
                TemplateAlias = "welcome",
                TemplateModel = new Dictionary<string, object>
                {
                    { "name", user.FirstName },
                    { "action_url", actionUrl },
                }.AddRange(_postmarkGlobalVariables),
            };
            try
            {
                await _postmarkClient.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task SendResetPassword(User user)
        {
            var message = new TemplatedPostmarkMessage
            {
                From = _emailSettings.PostmarkSenderEmail,
                To = user.EmailTo(),
                TemplateAlias = "password-reset",
                TemplateModel = new Dictionary<string, object>
                {
                    { "name", user.FirstName }, 
                    { "action_url", _conf["App:URL"] + $"/reset?e={user.Email}&t={user.Token}" }
                }.AddRange(_postmarkGlobalVariables),
            };
            try
            {
                await _postmarkClient.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        public async Task SendUserInvitation(Tenant tenant, User user, TenantUser tenantUser)
        {
            var message = new TemplatedPostmarkMessage
            {
                From = _emailSettings.PostmarkSenderEmail,
                To = tenantUser.User.EmailTo(),
                TemplateAlias = "user-invitation",
                TemplateModel = new Dictionary<string, object>
                {
                    {"name", tenantUser.User.FirstName},
                    {"invite_sender_name", user.FirstName},
                    {"invite_sender_organization", tenant.Name},
                    {
                        "action_url",
                        _conf["App:URL"] + "/invitation?i=" + tenantUser.InvitationLink + "&e=" + tenantUser.User.Email
                    },
                }.AddRange(_postmarkGlobalVariables),
            };
            try
            {
                await _postmarkClient.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public async Task SendRequestedAccess(TenantUser tenantUser, Tenant tenant)
        {
            var message = new TemplatedPostmarkMessage
            {
                From = _emailSettings.PostmarkSenderEmail,
                To = string.Join(",", tenant.GetOwners().Select(f => f.User.EmailTo())),
                TemplateAlias = "request-access",
                TemplateModel = new Dictionary<string, object>
                {
                    {"user_name", tenantUser.User.FirstName},
                    {"user_email", tenantUser.User.Email},
                    {"organization", tenant.Name},
                    { "action_url", _conf["App:URL"] + "/app/settings/organization/members?au=" + tenantUser.User.Email.ToLower().Trim() },
                }.AddRange(_postmarkGlobalVariables),
            };
            
            try
            {
                await _postmarkClient.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        public async Task SendUserAccepted(Tenant tenant, User admin, TenantUser acceptedUser)
        {
            var message = new TemplatedPostmarkMessage
            {
                From = _emailSettings.PostmarkSenderEmail,
                To = acceptedUser.User.EmailTo(),
                TemplateAlias = "user-accepted",
                TemplateModel = new Dictionary<string, object>
                {
                    {"admin_name", admin.FirstName ?? admin.Email},
                    {"product_name", _conf["App:Name"]},
                    {"name", acceptedUser.User.FirstName},
                    {"organization", tenant.Name},
                    {
                        "action_url",
                        _conf["App:URL"] + "/join/" + tenant.TenantJoinSettings.Link + "?e=" + acceptedUser.User.Email
                    },
                }.AddRange(_postmarkGlobalVariables),
            };
            try
            {
                await _postmarkClient.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        public async Task SendInviteCompanyToCreateAccount(LinkInvitation invitation)
        {
            var message = new TemplatedPostmarkMessage
            {
                From = _emailSettings.PostmarkSenderEmail,
                To = invitation.Email,
                TemplateAlias = "invite-company-to-create-account",
                TemplateModel = new Dictionary<string, object>
                {
                    { "invite_sender_email", invitation.CreatedByUser.Email },
                    { "invite_sender_name", invitation.CreatedByUser.FirstName },
                    { "workspace_creator", invitation.CreatedByWorkspace.Name },
                    { "workspace_invitee", invitation.WorkspaceName },
                    { "message", invitation.Message },
                    { "invitation_role", invitation.InviteeIsProvider ?
                        $"as a provider" :
                        $"as a client"},
                    { "action_url", $"{_conf["App:URL"]}/register?i={invitation.Id}" },
                }.AddRange(_postmarkGlobalVariables),
            };
            try
            {
                await _postmarkClient.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        public async Task SendInviteUserToLinkWorkspace(User fromUser, Workspace fromWorkspace, Link link, User user, Workspace workspace, bool asProvider)
        {
            var message = new TemplatedPostmarkMessage
            {
                From = _emailSettings.PostmarkSenderEmail,
                To = user.EmailTo(),
                TemplateAlias = "invite-user-to-link-workspace",
                TemplateModel = new Dictionary<string, object>
                {
                    { "name", user.FirstName },
                    { "invite_sender_name", fromUser.FirstName },
                    { "invite_sender_email", fromUser.Email },
                    { "workspace_invitee", workspace.Name },
                    { "workspace_creator", fromWorkspace.Name },
                    { "message", "" },
                    {"invitation_role", asProvider ?
                        $"as a provider" :
                        $"as a client"},
                    { "action_url", $"{_conf["App:URL"]}/app/links/pending" },
                }.AddRange(_postmarkGlobalVariables),
            };
            try
            {
                await _postmarkClient.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task SendLinkInvitationAccepted(User userAccepting, Link link, bool asProvider)
        {
            var message = new TemplatedPostmarkMessage
            {
                From = _emailSettings.PostmarkSenderEmail,
                To = link.CreatedByUser.EmailTo(),
                TemplateAlias = "link-invitation-accepted",
                TemplateModel = new Dictionary<string, object>
                {
                    { "name", link.CreatedByUser.FirstName},
                    { "user_invitee_name", userAccepting.FirstName},
                    { "user_invitee_email", userAccepting.Email},
                    { "workspace_invitee", asProvider ? link.ProviderWorkspace.Name : link.ClientWorkspace.Name },
                    { "action_text",
                        asProvider ?
                            $"View my providers" :
                            $"View my clients"
                    },
                    { "action_url",
                        asProvider ?
                            $"{_conf["App:URL"]}/app/providers" :
                            $"{_conf["App:URL"]}/app/clients"
                         },
                }.AddRange(_postmarkGlobalVariables),
            };
            try
            {
                await _postmarkClient.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task SendLinkInvitationRejected(string email, string workspaceName, User toUser)
        {
            var message = new TemplatedPostmarkMessage
            {
                From = _emailSettings.PostmarkSenderEmail,
                To = toUser.EmailTo(),
                TemplateAlias = "link-invitation-rejected",
                TemplateModel = new Dictionary<string, object>
                {
                    {"name", toUser.FirstName},
                    { "email", email},
                    { "workspace", workspaceName},
                }.AddRange(_postmarkGlobalVariables),
            };
            try
            {
                await _postmarkClient.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        public async Task SendContractNew(User userCreator, Link link, Contract contract)
        {
            try
            {
                var attachments = new List<PostmarkMessageAttachment>();
                var attachment =
                    new PostmarkMessageAttachment(contract.GetFileData(), contract.Name, "application/pdf");
                attachments.Add(attachment);
                foreach (var member in contract.Members)
                {
                    var contractMembers = new List<Dictionary<string, string>>();
                    foreach (var contractMember in contract.Members)
                    {
                        var dictMember = new Dictionary<string, string>
                        {
                            { "first_name", contractMember.User.FirstName },
                            { "last_name", contractMember.User.LastName },
                            { "email", contractMember.User.Email },
                            { "role", contractMember.Role == ContractMemberRole.Signatory ? "Signatory" : "Spectator" }
                        };
                        contractMembers.Add(dictMember);
                    }

                    var contractEmployees = new List<Dictionary<string, string>>();
                    foreach (var contractEmployee in contract.Employees)
                    {
                        var dictMember = new Dictionary<string, string>
                        {
                            { "first_name", contractEmployee.Employee.FirstName },
                            { "last_name", contractEmployee.Employee.LastName },
                            { "email", contractEmployee.Employee.Email }
                        };
                        contractEmployees.Add(dictMember);
                    }

                    var message = new TemplatedPostmarkMessage
                    {
                        From = _emailSettings.PostmarkSenderEmail,
                        To = member.User.EmailTo(),
                        TemplateAlias = "contract-new",
                        Attachments = attachments,
                        TemplateModel = new Dictionary<string, object>
                        {
                            { "user_creator_firstName", userCreator.FirstName },
                            { "user_creator_email", userCreator.Email },
                            { "contract_name", contract.Name },
                            { "workspace_creator", link.CreatedByWorkspace.Name },
                            { "workspace_provider", link.ProviderWorkspace.Name },
                            { "workspace_client", link.ClientWorkspace.Name },
                            { "contract_description", contract.Description },
                            { "action_url", $"{_conf["App:URL"]}/app/contract/{contract.Id}" },
                            { "members", contractMembers },
                            { "employees", contractEmployees }
                        }.AddRange(_postmarkGlobalVariables),
                    };

                    await _postmarkClient.SendMessageAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}