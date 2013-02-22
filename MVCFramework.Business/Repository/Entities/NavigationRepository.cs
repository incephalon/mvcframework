using System;
using System.Collections.Generic;
using System.Linq;
using MVCFramework.Model.Entities;
using NHibernate;
using NHibernate.Linq;

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
            BeginTransaction();

            var nq = from navigation in All()
                     where navigation.Tenant.ID == tenantID
                           && navigation.Role == null
                     select navigation;

            var result = nq.Fetch(n => n.Items)
                .SingleOrDefault();

            CommitTransaction();

            return result;
        }

        public Navigation GetRoleNavigation(string rolename, Guid tenantID)
        {
            BeginTransaction();

            var rq = from role in new RoleRepository(_session).All()
                     where role.Tenant.ID == tenantID && role.Name == rolename
                     select role.ID;

            var nq = from navigation in All()
                     where navigation.Tenant.ID == tenantID && rq.Contains(navigation.Role.ID)
                     select navigation;

            var result = nq
                .Fetch(n => n.Role)
                .Fetch(n => n.Items)
                           .Distinct()
                           .SingleOrDefault();

            CommitTransaction();

            return result;
        }

        public List<Navigation> GetUserNavigations(string username, Guid tenantID)
        {
            BeginTransaction();

            var rq = from user in new UserRepository(_session).All()
                     where user.Tenant.ID == tenantID && user.UserName == username
                     from role in user.Roles
                     select role.ID;

            var nq = from navigation in All()
                     where navigation.Tenant.ID == tenantID && rq.Contains(navigation.Role.ID)
                     select navigation;

            var result = nq
                .Fetch(n => n.Role)
                .Fetch(n => n.Items)
                           .Distinct()
                           .ToList();

            CommitTransaction();

            return result;

        }
    }
}
