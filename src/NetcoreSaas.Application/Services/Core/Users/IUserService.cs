using System;
using System.Threading.Tasks;
using NetcoreSaas.Application.Contracts.Core.Users;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Application.Services.Core.Users
{
    public interface IUserService
    {
        Task<UserLoggedResponse> Authenticate(User user, Guid? tenantId = null);
        Task<User> Register(UserRegisterRequest request, LinkInvitation linkInvitation);
        Task<UserLoggedResponse> GetUserWithTenant(Guid id, Guid? tenantId = null);
        Guid? GetClaim(string name);
        void UpdateClaim(string name, object value);
    }
}
