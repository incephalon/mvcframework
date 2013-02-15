using MVCFramework.Business.Providers.NHibernateSession;
using NHibernate;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MVCFramework.Business.Modules
{
    public class NHibernateRepositoryModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();
            Bind<ISession>().ToProvider(new NHibernateSessionProvider());
        }
    }
}