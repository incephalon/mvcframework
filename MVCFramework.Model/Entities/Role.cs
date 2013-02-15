using System;
using System.Collections.Generic;

namespace MVCFramework.Model.Entities
{
    public class Role : IEntity<Guid>
    {
        public virtual Guid ID { get; set; }
        
        public virtual string Name { get; set; }

        public virtual Tenant Tenant { get; set; }

        public virtual IList<User> Users { get; set; }
    }
}
