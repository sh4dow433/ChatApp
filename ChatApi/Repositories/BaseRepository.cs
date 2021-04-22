using ChatApi.DbAccess;
using ChatApi.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ChatApi.Repositories
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext _dbContext;

        public BaseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            return _dbContext.Set<TEntity>().Where(filter).ToList();
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>().ToList();
        }

        public virtual TEntity GetByID(object id)
        {
            return _dbContext.Set<TEntity>().Find(id);
        }


        public virtual void Insert(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }


        public virtual void Delete(TEntity entityToDelete)
        {
            _dbContext.Set<TEntity>().Remove(entityToDelete);
        }

        public void Delete(object id)
        {
            var entity = _dbContext.Set<TEntity>().Find(id);
            Delete(entity);
        }

    }
}
