using System;

namespace MVCFramework.Model.Entities
{
    public class Tenant : IEntity<Guid>
    {
        public virtual Guid ID { get; set; }

        public virtual string Name { get; set; }

        public virtual Portal Portal { get; set; }
    }
}
