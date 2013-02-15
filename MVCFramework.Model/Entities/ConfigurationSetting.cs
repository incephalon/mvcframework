namespace MVCFramework.Model.Entities
{
    public class ConfigurationSetting: IEntity<int>
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string DefaultValue { get; set; }

        public virtual ConfigurationSection Section { get; set; }
    }
}
