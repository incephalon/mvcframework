using System.Configuration.Provider;

namespace MVCFramework.Business.Providers.Storage
{
    public class StorageProviderCollection : ProviderCollection
    {
        new public StorageProviderBase this[string name]
        {
            get { return (StorageProviderBase)base[name]; }
        }
    }
}
