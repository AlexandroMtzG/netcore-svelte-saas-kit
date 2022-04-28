using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;

namespace NetcoreSaas.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserId(this HttpContext context)
        {
            try
            {
                return Guid.Parse(context.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value!);
            }
            catch
            {
                return Guid.Empty;
            }
        }
            
        public static Guid GetTenantId(this HttpContext context)
        {
            try
            {
                return Guid.Parse(context.User.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value!);
            }
            catch
            {
                return Guid.Empty;
            }
        }
        
        public static Guid GetWorkspaceId(this HttpContext context)
        {
            try
            {
                return Guid.Parse(context.Request.Headers["X-Workspace-Id"]);
            }
            catch
            {
                return Guid.Empty;
            }
        }
        
        public static Guid GetTenantUuid(this HttpContext context)
        {
            try
            {
                return Guid.Parse(context.User.Claims.FirstOrDefault(c => c.Type == "TenantUserUuid")?.Value!);
            }
            catch
            {
                return Guid.Empty;
            }
        }
        
        public static UserType GetUserType(this HttpContext context)
        {
            if (context.User == null)
                throw new UnauthorizedAccessException("No se ha iniciado sesión");
            var type = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if(string.IsNullOrEmpty(type))
                throw new UnauthorizedAccessException("No se ha iniciado sesión");
            return (UserType)Enum.Parse(typeof(UserType), type);
        }
        
        public static string GetUserEmail(this HttpContext context)
        {
            return context.User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
        }
        
        public static TenantUserRole GetTenantUserRole(this HttpContext context)
        {
            try
            {
                var role = context.User.Claims.FirstOrDefault(c => c.Type == "TenantUserRole")?.Value.ToString();

                Enum.TryParse(role, out TenantUserRole userRole);
                return userRole;
            }
            catch
            {
                return TenantUserRole.Guest;
            }
        }
        
        public static Guid GetTenantUserId(this HttpContext context)
        {
            try
            {
                var role = context.User.Claims.FirstOrDefault(c => c.Type == "TenantUserId")?.Value.ToString();
                return Guid.Parse(role!);
            }
            catch
            {
                return Guid.Empty;
            }
        }
    }
}
