using System;
using System.Collections.Generic;

namespace MVCFramework.Model.Entities
{
    public class Portal : IEntity<Guid>
    {
        public virtual Guid ID { get; set; }

        public virtual string Title { get; set; }
        public virtual string Url { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual IList<PortalAlias> Aliases { get; set; }
    }
}