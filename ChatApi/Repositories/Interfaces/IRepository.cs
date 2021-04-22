using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ChatApi.Repositories.Interfaces
{

    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter);
        TEntity GetByID(object id);

        void Insert(TEntity entity);

        void Delete(TEntity entityToDelete);
        void Delete(object id);
    }
}
