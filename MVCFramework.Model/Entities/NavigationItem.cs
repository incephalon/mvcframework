namespace MVCFramework.Model.Entities
{
    public class NavigationItem : IEntity<int>
    {
        public virtual int ID { get; set; }

        public virtual string Text { get; set; }
        public virtual string Url { get; set; }
        public virtual string Icon { get; set; }
        public virtual int Order { get; set; }
        public virtual bool ShowInMenu { get; set; }

        public virtual Navigation Navigation { get; set; }
        public virtual NavigationItem ParentItem { get; set; }
    }
}
