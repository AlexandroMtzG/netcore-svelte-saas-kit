using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetcoreSaas.Domain.Enums.Core.Users;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Application.Repositories.Core
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByEmail(string email);
        Task<User> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersWithTenants();
        Task<IEnumerable<TenantUser>> GetUserTenants(User user, bool onlyActive = false);
        void ChangeUserDefaultTenant(User user, Tenant tenant);
        User AddNewUser(string email, UserType tenant, string firstName, string lastName, string phone, UserLoginType loginType, string password = null, Guid? token = null);
        void RemoveUser(User user);
    }
}
