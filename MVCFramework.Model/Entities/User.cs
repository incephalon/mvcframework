using System;
using System.Collections.Generic;

namespace MVCFramework.Model.Entities
{
    public class User : IEntity<Guid>
    {
        public virtual Guid ID { get; set; }

        public virtual string UserName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Hash { get; set; }
        public virtual bool Enabled { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual UserProfile Profile { get; set; }

        public virtual IList<Role> Roles { get; protected set; }

        public User(Tenant tenant, string username, string email, string hash)
        {
            Tenant = tenant;

            UserName = username;
            Email = email;
            Hash = hash;
        }

        public User(Guid userID)
        {
            ID = userID;
        }

        protected User()
        {

        }
    }
}
