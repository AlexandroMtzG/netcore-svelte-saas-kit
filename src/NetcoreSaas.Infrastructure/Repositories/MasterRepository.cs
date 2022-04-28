using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetcoreSaas.Application.Repositories;
using NetcoreSaas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NetcoreSaas.Domain.Models.Core;

namespace NetcoreSaas.Infrastructure.Repositories
{
    public class MasterRepository<TEntity> : IMasterRepository<TEntity> where TEntity : MasterEntity
    {
        protected readonly MasterDbContext Context;

        private DbSet<TEntity> Dbset
        {
            get { return Context.Set<TEntity>(); }
        }

        public MasterRepository(MasterDbContext dbContext)
        {
            Context = dbContext;
        }

        public IQueryable<TEntity> GetAllWithPermission()
        {
            return Dbset.AsQueryable();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Dbset.AsEnumerable();
        }

        public TEntity GetById(Guid id)
        {
            return Dbset.Find(id);
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

        //public void Update(TEntity entity)
        //{
        //    if (entity == null)
        //        throw new ArgumentNullException("Null entity");
        //    Dbset.Attach(entity);
        //    context.Entry(entity).State = EntityState.Modified;
        //}

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
            IEnumerable<TEntity> objects = Dbset.Where(where).AsEnumerable();
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

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> where)
        {
            return await Dbset.CountAsync(where);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Dbset.ToListAsync();
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
