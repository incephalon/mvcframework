using System;
using System.Linq;
using MVCFramework.Model.Entities;
using NHibernate;
using NHibernate.Linq;

namespace MVCFramework.Business.Repository.Entities
{
    public class UserRepository : Repository<Guid, User>
    {
        public UserRepository(ISession session)
            : base(session)
        {

        }

        public string GetUserNameByEmail(string email)
        {
            BeginTransaction();

            var q = from user in All()
                    where user.Email == email
                    select user.UserName;

            string username = q.SingleOrDefault();

            CommitTransaction();

            return username;
        }

        public User GetUserByName(string username, Guid tenantID)
        {
            BeginTransaction();

            var q = from user in All()
                    where user.UserName == username
                          && user.Tenant.ID == tenantID
                    select user;

            var result = q.SingleOrDefault();

            CommitTransaction();

            return result;
        }

        public UserProfile GetProfileByUserName(string username, Guid tenantID, out Guid userID)
        {
            BeginTransaction();

            var qu = from user in All()
                    where user.UserName == username
                          && user.Tenant.ID == tenantID
                    select user.ID;

           var _userID = qu.SingleOrDefault();

            var qp = from user in All()
                     where user.ID == _userID
                     select user.Profile;
            

            var profile = qp.SingleOrDefault();

            CommitTransaction();
            userID = _userID;

            return profile;
        }

        public void SaveProfile(UserProfile profile)
        {
            new Repository<Guid, UserProfile>(_session)
                .Save(profile);
        }

        public void UpdatePassword(string username, Guid tenantID, string p)
        {
            throw new NotImplementedException();
        }

    }
}
