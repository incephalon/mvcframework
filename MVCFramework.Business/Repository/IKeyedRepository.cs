using MVCFramework.Model;
using System.Collections.Generic;

namespace MVCFramework.Business.Repository
{
    public interface IKeyedRepository<TKey, TEntity> : IReadOnlyRepository<TEntity> where TEntity : class , IEntity<TKey>
    {
        TEntity FindByID(TKey id);
        TKey Add(TEntity entity);
        void Add(IEnumerable<TEntity> items);
        TKey Insert(TEntity entity);
        void Insert(IEnumerable<TEntity> items);
        void Save(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void Delete(IEnumerable<TEntity> entities);
    }
}
