using System;
using System.Linq;
using System.Threading.Tasks;
using NetcoreSaas.Application.Contracts.App.Contracts;
using NetcoreSaas.Application.Services.App;
using NetcoreSaas.Application.Services.Messages;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.App.Contracts;
using NetcoreSaas.Domain.Models.App.Contracts;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Infrastructure.Services.App
{
    public class ContractService : IContractService
    {
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly IEmailService _emailService;
        
        public ContractService(IMasterUnitOfWork masterUnitOfWork, IEmailService emailService)
        {
            _masterUnitOfWork = masterUnitOfWork;
            _emailService = emailService;
        }
        
        public async Task<Contract> Create(User currentUser, Link link, CreateContractRequest request)
        {
            var providerMembers = (await _masterUnitOfWork.Workspaces.GetUsersWithWorkspace(link.ProviderWorkspaceId)).ToList();
            var clientMembers = (await _masterUnitOfWork.Workspaces.GetUsersWithWorkspace(link.ClientWorkspaceId)).ToList();

            var contract = new Contract()
            {
                CreatedByUserId = currentUser.Id,
                Name = request.Name,
                Description = request.Description,
                LinkId = request.LinkId,
                File = request.File,
                Status = ContractStatus.Pending
            };
            foreach (var member in request.Members)
            {
                var contractMember = new ContractMember()
                {
                    ContractId = contract.Id,
                    Role = member.Role,
                    UserId = member.UserId
                };
                contract.Members.Add(contractMember);
            }

            contract.Activity.Add(new ContractActivity()
            {
                CreatedAt = new DateTime(),
                CreatedByUserId = currentUser.Id,
                ContractId = contract.Id,
                Type = ContractActivityType.Created
            });

            foreach (var employee in request.Employees)
            {
                contract.Employees.Add(new ContractEmployee()
                {
                    EmployeeId = employee.Id
                });
            }
            
            _masterUnitOfWork.Contracts.Add(contract);

            foreach (var member in contract.Members)
            {
                member.User = _masterUnitOfWork.Users.GetById(member.UserId);
                var workspace = providerMembers.FirstOrDefault(f => f.UserId == member.UserId) ?? clientMembers.FirstOrDefault(f => f.UserId == member.UserId);
                if (workspace != null)
                    member.WorkspaceName = workspace.Workspace.Name;
            }

            // CreateDigitalDocument(contract);

            await _masterUnitOfWork.CommitAsync();

            foreach (var employee in contract.Employees)
                employee.Employee = _masterUnitOfWork.Employees.GetById(employee.EmployeeId);
            
            await _emailService.SendContractNew(currentUser, link, contract);
            
            return contract;
        }
    }
}