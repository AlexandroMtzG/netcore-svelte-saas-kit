using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetcoreSaas.Application.Repositories.Core;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Infrastructure.Data;

namespace NetcoreSaas.Infrastructure.Repositories.Core
{
    public class UserRepository : MasterRepository<User>, IUserRepository
    {
        public UserRepository(MasterDbContext context) : base(context)
        {
        }

        public User GetByEmail(string email)
        {
            return Context.Users.Include(f => f.Tenants).SingleOrDefault(f => f.Email == email.ToLower().Trim());
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await Context.Users
                .Include(f => f.Tenants).ThenInclude(f=>f.Tenant)
                .FirstOrDefaultAsync(f => f.Email == email.ToLower().Trim());
        }

        public void ChangeUserDefaultTenant(User user, Tenant tenant)
        {
            user.DefaultTenantId = tenant.Id;
            Context.Users.Attach(user);
            Context.Entry(user).Property(f => f.DefaultTenantId).IsModified = true;
        }

        public async Task<IEnumerable<TenantUser>> GetUserTenants(User user, bool onlyActive = false)
        {
            if (onlyActive)
            {
                return await Context.TenantUsers
                    .Include(f => f.User)
                    .Include(f => f.Tenant).ThenInclude(f => f.Products.Where(x => x.Active)).ThenInclude(f=>f.SubscriptionPrice).ThenInclude(f => f.SubscriptionProduct)
                    .Where(f => (f.UserId == user.Id && f.Status == TenantUserStatus.Active))
                    .ToListAsync();
            }

            return await Context.TenantUsers
                .Include(f => f.User)
                .Include(f => f.Tenant).ThenInclude(f => f.Products.Where(x => x.Active)).ThenInclude(f => f.SubscriptionPrice).ThenInclude(f => f.SubscriptionProduct)
                .Where(f => f.UserId == user.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersWithTenants()
        {
            var users = Context.Users
                .Include(f => f.Tenants)
                .ThenInclude(f=>f.Tenant)
                .AsQueryable();

            users = users.Select(f => new User()
            {                        
                Id = f.Id,
                Email = f.Email,
                FirstName = f.FirstName,
                Type = f.Type,
                LastName = f.LastName,
                Phone = f.Phone,
                DefaultTenantId = f.DefaultTenantId,
                Gender = f.Gender,
                CreatedAt = f.CreatedAt,
                Tenants = f.Tenants.Select(x=>new TenantUser()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    TenantId = x.TenantId,
                    Role = x.Role,
                    Tenant = new Tenant()
                    {
                        Id = x.Tenant.Id,
                        Name = x.Tenant.Name
                    }
                }).ToList()
            });
            return await users.ToListAsync();
        }

        public User AddNewUser(string email, UserType type, string firstName, string lastName, string phone, UserLoginType loginType, string password = null, Guid? token = null)
        {
            var user = new User()
            {
                Email = email.ToLower().Trim(),
                Type = type,
                FirstName = firstName,
                LastName = lastName,
                Phone = phone,
                LoginType = loginType,
                Password = password
            };
            if (token.HasValue)
                user.Token = token.Value;
            Context.Users.Add(user);
            return user;
        }

        public void RemoveUser(User user)
        {
            Context.Users.Remove(user);
        }
    }
}
