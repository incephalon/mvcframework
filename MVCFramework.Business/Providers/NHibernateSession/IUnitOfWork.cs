using NHibernate;
using System;

namespace MVCFramework.Business.Providers.NHibernateSession
{
    public interface IUnitOfWork : IDisposable
    {
        ISession Session { get; }
        void Commit();
        void Rollback();
    }
}
