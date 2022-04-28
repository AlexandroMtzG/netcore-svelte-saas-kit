using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetcoreSaas.Application.Services.Messages;
using NetcoreSaas.Domain.Helpers;

namespace NetcoreSaas.WebApi.Controllers.Core.Setup
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SetupController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public SetupController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet(ApiCoreRoutes.Setup.GetPostmarkTemplates)]
        public async Task<IActionResult> GetPostmarkTemplates()
        {
            var templates = await _emailService.GetAllTemplates();
            return Ok(templates);
        }
        
        
        [HttpPost(ApiCoreRoutes.Setup.CreatePostmarkTemplates)]
        public async Task<IActionResult> CreatePostmarkTemplates()
        {
            try
            {
                var templates = await _emailService.CreateTemplates();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}