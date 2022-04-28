using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetcoreSaas.Application.Repositories;
using NetcoreSaas.Domain.Models;
using NetcoreSaas.Domain.Models.Interfaces;
using NetcoreSaas.Infrastructure.Data;
using NetcoreSaas.Infrastructure.Middleware.Tenancy;
using Microsoft.EntityFrameworkCore;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;

namespace NetcoreSaas.Infrastructure.Repositories
{
    public class AppRepository<TEntity> : IRepository<TEntity> where TEntity : Entity, IAppEntity
    {
        protected readonly BaseDbContext Context;
        private readonly ITenantAccessService _tenantAccessService;
        private DbSet<TEntity> Dbset
        {
            get { return Context.Set<TEntity>(); }
        }

        public AppRepository(BaseDbContext dbContext, ITenantAccessService tenantAccessService = null)
        {
            Context = dbContext;
            _tenantAccessService = tenantAccessService;
        }

        public IQueryable<TEntity> GetAllWithPermission()
        {
            var records = Dbset.AsQueryable();

            if (_tenantAccessService == null)
                return Dbset;
            var userType = _tenantAccessService.GetUserType();
            if (userType == UserType.Admin) return Dbset;

            var userId = _tenantAccessService.GetUserId();
            var userRole = _tenantAccessService.GetTenantUserRole();
            
            if (userRole == TenantUserRole.Member || userRole == TenantUserRole.Guest)
                records = records.Where(f => f.CreatedByUserId == userId).AsQueryable();

            return records;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Dbset.AsEnumerable();
        }

        public TEntity GetById(Guid id)
        {
            var record = Dbset.Find(id);
            return record;
        }

        public void Add(TEntity entity)
        {
            Dbset.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Dbset.Add(entity);
            }
        }

        public void Remove(Guid id)
        {
            var entity = GetById(id);
            if (entity == null)
                throw new Exception("Null entity");
            Dbset.Remove(entity);
        }

        public void Remove(TEntity entity)
        {
            Dbset.Remove(entity);
        }

        public void Remove(Expression<Func<TEntity, bool>> @where)
        {
            var objects = Dbset.Where(where).AsEnumerable();
            foreach (TEntity obj in objects)
                Dbset.Remove(obj);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> @where)
        {
            return Dbset.Where(where).FirstOrDefault();
        }

        public IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
        {
            return Dbset.Where(where).ToList();
        }

        public int Count()
        {
            return Dbset.Count();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Dbset.ToListAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> where)
        {
            return await Dbset.CountAsync(where);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> @where)
        {
            return await Dbset.Where(where).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> @where)
        {
            return await Dbset.Where(where).ToListAsync();
        }
    }
}
