using System;
using System.Collections.Generic;
using System.Linq;
using MVCFramework.Model.Entities;
using NHibernate;

namespace MVCFramework.Business.Repository.Entities
{
    public class RoleRepository : Repository<Guid, Role>
    {
        public RoleRepository(ISession session)
            : base(session)
        {

        }

        public IList<string> GetUserRoles(string username, Guid tenantID)
        {
            BeginTransaction();
            var q = from user in new UserRepository(_session).All()
                    where user.UserName == username && user.Tenant.ID == tenantID
                    from role in user.Roles
                    select role.Name;

            var roles = q.ToList();
            CommitTransaction();

            return roles;
        }

        public bool IsUserInRole(string username, Guid tenantID, string rolename)
        {
            BeginTransaction();
            var uq = from role in All()
                     where role.Name == rolename && role.Tenant.ID == tenantID
                         from user in role.Users
                         where user.UserName == username
                     select user.ID;

            var uid = uq.SingleOrDefault();
            CommitTransaction();

            return uid != default(Guid);
        }

        public IList<string> GetAllRoles(Guid tenantID)
        {
            BeginTransaction();
            var rq = from role in All()
                     where role.Tenant.ID == tenantID
                     select role.Name;

            var roles = rq.ToList();
            CommitTransaction();

            return roles;
        }
    }
}
