using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using MVCFramework.Model;

namespace MVCFramework.Business.Repository
{
    public class Repository<TKey, T> : IKeyedRepository<TKey, T> where T : class, IEntity<TKey>
    {
        protected readonly ISession _session;

        public Repository(ISession session)
        {
            _session = session;
        }

        public T FindByID(TKey id)
        {
            return _session.Get<T>(id);
        }

        public TKey Add(T entity)
        {
            TKey ID = (TKey)_session.Save(entity);

            return ID;
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (T item in items)
                _session.Save(item);
        }

        public TKey Insert(T entity)
        {
            BeginTransaction();
            TKey ID = (TKey)_session.Save(entity);
            CommitTransaction();
            return ID;
        }

        public void Insert(IEnumerable<T> items)
        {
            BeginTransaction();
            foreach (T item in items)
                _session.Save(item);
            CommitTransaction();
        }

        public void Update(T entity)
        {
            BeginTransaction();
            _session.Update(entity);
            CommitTransaction();
        }

        public void Save(T entity)
        {
            BeginTransaction();
            _session.SaveOrUpdate(entity);
            CommitTransaction();
        }

        public void Delete(T entity)
        {
            _session.Delete(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                _session.Delete(entity);
        }

        public IQueryable<T> All()
        {
            return _session.Query<T>();
        }

        public T FindBy(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return FilterBy(expression).SingleOrDefault();
        }

        public IQueryable<T> FilterBy(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return All().Where(expression).AsQueryable();
        }

        public void BeginTransaction()
        {
            _session.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_session.Transaction.IsActive)
                _session.Transaction.Commit();
            else
                throw new InvalidOperationException("There is no active transaction to commit.");
        }

        public void Rollback()
        {
            if (_session.Transaction.IsActive)

                _session.Transaction.Rollback();
            else
                throw new InvalidOperationException("There is no active transaction to rollback.");
        }

    }
}