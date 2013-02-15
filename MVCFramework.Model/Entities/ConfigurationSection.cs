namespace MVCFramework.Model.Entities
{
    public class ConfigurationSection : IEntity<int>
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }

        public virtual Configuration Configuration { get; set; }
    }
}
