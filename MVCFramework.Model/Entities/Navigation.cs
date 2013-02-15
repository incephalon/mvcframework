using System.Collections.Generic;

namespace MVCFramework.Model.Entities
{
    public class Navigation : IEntity<int>
    {
        public virtual int ID { get; set; }

        public virtual string Name { get; set; }
        public virtual bool IsDefault { get; set; }

        public virtual Tenant Tenant { get; set; }

        public virtual IList<NavigationItem> Items { get; set; }
    }
}
