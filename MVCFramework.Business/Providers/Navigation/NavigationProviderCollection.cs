using System.Configuration.Provider;

namespace MVCFramework.Business.Providers.Navigation
{
    public class NavigationProviderCollection : ProviderCollection
    {
        new public NavigationProviderBase this[string name]
        {
            get { return (NavigationProviderBase)base[name]; }
        }
    }
}
