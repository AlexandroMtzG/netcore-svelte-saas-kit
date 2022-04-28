using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NetcoreSaas.Application.Contracts.Core.Users;
using NetcoreSaas.Application.Helpers;
using NetcoreSaas.Application.Services.Core.Users;
using NetcoreSaas.Application.Services.Messages;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.App.Links;
using NetcoreSaas.Domain.Enums.Core.Users;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Infrastructure.Extensions;

namespace NetcoreSaas.WebApi.Controllers.Core.Users
{
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AuthenticationController(IWebHostEnvironment env, IMasterUnitOfWork masterUnitOfWork, IUserService userService, IEmailService emailService)
        {
            _env = env;
            _masterUnitOfWork = masterUnitOfWork;
            _userService = userService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost(ApiCoreRoutes.Authentication.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var user = await _masterUnitOfWork.Users.GetByEmailAsync(request.Email);
            if (user != null)
            {
                if (user.LoginType == UserLoginType.Password)
                {
                    if (string.IsNullOrEmpty(user.Password))
                    {
                        return BadRequest("api.errors.invalidPassword");
                    }
                    var result = PasswordHasher.VerifyHashedPassword(user.Password, request.Password);
                    if (result != PasswordVerificationResult.Success)
                    {
                        return BadRequest("api.errors.invalidPassword");
                    }
                }
            }
            else
            {
                return BadRequest("api.errors.userNotRegistered");
            }
            try
            {
                return Ok(await _userService.Authenticate(user));
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost(ApiCoreRoutes.Authentication.AdminImpersonate)]
        public async Task<IActionResult> AdminImpersonate(Guid userId)
        {
            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            if (currentUser == null || currentUser.Type != UserType.Admin)
                return BadRequest("api.errors.unauthorized");

            var impersonate = _masterUnitOfWork.Users.GetById(userId);
            if (impersonate == null)
                return NotFound();

            var user = await _masterUnitOfWork.Users.GetByEmailAsync(impersonate.Email);

            return Ok(await _userService.Authenticate(user));
        }

        [AllowAnonymous]
        [HttpPost(ApiCoreRoutes.Authentication.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            if (EmailHelper.IsValidEmail(request.Email) == false) 
                return BadRequest("api.errors.invalidEmail");
            
            LinkInvitation linkInvitation = null;
            if (request.JoinedByLinkInvitation.HasValue)
            {
                linkInvitation = await _masterUnitOfWork.Links.GetInvitation(request.JoinedByLinkInvitation.Value);
                if (linkInvitation == null)
                    return BadRequest("api.errors.invalidLinkInvitation");
                if (linkInvitation.Status != LinkStatus.Pending)
                    return BadRequest("api.errors.notPendingLinkInvitation");
                if (linkInvitation.Email.Trim().ToLower() != request.Email.Trim().ToLower())
                    return BadRequest("api.errors.emailInvalidLinkInvitation");
                if (linkInvitation.WorkspaceName.Trim().ToLower() != request.WorkspaceName.Trim().ToLower())
                    return BadRequest("api.errors.workspaceInvalidLinkInvitation");
            }

            var price = await _masterUnitOfWork.Subscriptions.GetPrice(request.SelectedSubscription.SubscriptionPriceId);
            if(price == null)
                return BadRequest("api.errors.invalidSubscription");

            if(price.Price > 0 && price.TrialDays == 0 && string.IsNullOrEmpty(request.SelectedSubscription.SubscriptionCardToken))
                return BadRequest("api.errors.invalidCard");

            var user = await _masterUnitOfWork.Users.GetByEmailAsync(request.Email);
            if (user != null)
                return BadRequest("api.errors.userAlreadyRegistered");

            await _userService.Register(request, linkInvitation);
            user = await _masterUnitOfWork.Users.GetByEmailAsync(request.Email);

            if (!Infrastructure.ProjectConfiguration.GlobalConfiguration.RequiresVerification && string.IsNullOrEmpty(request.Password) == false)
                return Ok(await _userService.Authenticate(user));

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost(ApiCoreRoutes.Authentication.Verify)]
        public async Task<IActionResult> Verify([FromBody] UserVerifyRequest request)
        {
            var user = await _masterUnitOfWork.Users.GetByEmailAsync(request.Email);
            if (user == null)
                return NotFound();
            if (user.Token != request.Token)
                return BadRequest("api.errors.unauthorized");
            if(user.IsVerified())
                return BadRequest("api.errors.userAlreadyVerified");

            user.Password = PasswordHasher.HashPassword(request.Password);
            user.Token = Guid.Empty;
            await _masterUnitOfWork.CommitAsync();

            return Ok(await _userService.Authenticate(user));
        }

        [AllowAnonymous]
        [HttpPost(ApiCoreRoutes.Authentication.Reset)]
        public async Task<IActionResult> Reset(string email)
        {
            var user = await _masterUnitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            user.Token = Guid.NewGuid();
            await _masterUnitOfWork.CommitAsync();
            await _emailService.SendResetPassword(user);

            if (_env.IsDevelopment())
            {
                return Ok(new { verifyToken = user.Token });
            }
            return Ok();
        }
    }
}
