namespace MVCFramework.Model.Entities
{
    public class PortalAlias : IEntity<int>
    {
        public virtual int ID { get; set; }

        public virtual string Url { get; set; }

        public virtual Portal Portal { get; set; }
    }
}
