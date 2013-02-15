using System;
using MVCFramework.Model.Entities;
using NHibernate;

namespace MVCFramework.Business.Repository.Entities
{
    public class NavigationRepository : Repository<int, Navigation>
    {
        public NavigationRepository(ISession session)
            : base(session)
        {

        }

        public Navigation GetDefaultNavigation(Guid tenantID)
        {
            throw new NotImplementedException();
        }

        public Navigation GetNavigation(string username, Guid tenantID)
        {
            throw new NotImplementedException();
        }
    }
}
