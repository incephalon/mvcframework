namespace MVCFramework.Model
{
    public interface IEntity<TKey>
    {
        TKey ID { get; set; }
    }
}
