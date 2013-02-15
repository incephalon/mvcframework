using System.Configuration.Provider;

namespace MVCFramework.Business.Providers.Configuration
{
    public class SettingsProviderCollection : ProviderCollection
    {
        new public SettingsProviderBase this[string name]
        {
            get { return (SettingsProviderBase)base[name]; }
        }
    }
}
