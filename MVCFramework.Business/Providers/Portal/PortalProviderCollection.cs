using System.Configuration.Provider;

namespace MVCFramework.Business.Providers.Portal
{
    public class PortalProviderCollection : ProviderCollection
    {
        new public PortalProviderBase this[string name]
        {
            get { return (PortalProviderBase)base[name]; }
        }
    }
}
