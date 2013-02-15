namespace MVCFramework.Model.Entities
{
    public class Configuration : IEntity<int>
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }

        protected Configuration()
        {
            
        }

        public Configuration(string name)
        {
            Name = name;
        }
    }
}
