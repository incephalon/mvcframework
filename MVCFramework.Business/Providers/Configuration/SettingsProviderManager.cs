using System;
using System.Configuration;
using System.Web.Configuration;

namespace MVCFramework.Business.Providers.Configuration
{
    public class SettingsProviderManager
    {

        private static SettingsProviderBase defaultProvider;
        private static SettingsProviderCollection providers;

        static SettingsProviderManager()
        {
            Initialize();
        }

        private static void Initialize()
        {
            ProviderConfiguration configuration = (ProviderConfiguration)ConfigurationManager.GetSection("settings-providers");

            if (configuration == null)
                throw new ConfigurationErrorsException
                    ("The settings-providers configuration section is not set correctly.");

            providers = new SettingsProviderCollection();

            ProvidersHelper.InstantiateProviders(configuration.Providers
                , providers, typeof(SettingsProviderBase));

            providers.SetReadOnly();

            defaultProvider = providers[configuration.Default];

            if (defaultProvider == null)
                throw new Exception("No default settings provider is defined for the settings-providers section.");
        }

        public static SettingsProviderBase Provider
        {
            get
            {
                return defaultProvider;
            }
        }

        public static SettingsProviderCollection Providers
        {
            get
            {
                return providers;
            }
        }
    }
}
