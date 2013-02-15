using System.Collections.Generic;

namespace MVCFramework.Business.Providers.Configuration
{
    public abstract class SettingsProviderBase : System.Configuration.Provider.ProviderBase
    {

        public abstract IEnumerable<string> GetConfigurations();
        public abstract void EnsureConfiguration(string configuration);

        public abstract IEnumerable<string> GetSections(string configuration);
        public abstract void EnsureSection(string configuration, string section);

        public abstract IDictionary<string, string> GetSettings(string configuration, string section);
        public abstract string GetSetting(string configuration, string section, string setting);

        public abstract void SetSetting(string configuration, string section, string setting, string value);

        // user settings
        public abstract IDictionary<string, string> GetUserSettings(int userID, string configuration, string section);
        public abstract string GetUserSetting(int userID, string configuration, string section, string setting);
        public abstract void SetUserSetting(int userID, string configuration, string section, string setting, string value);

        // tenant settings
        public abstract IDictionary<string, string> GetTenantSettings(int tenantID, string configuration, string section);
        public abstract string GetTenantSetting(int tenantID, string configuration, string section, string setting);
        public abstract void SetTenantSetting(int tenantID, string configuration, string section, string setting, string value);

        /// <summary>
        /// Used for providers that do not persist settings when they are set
        /// </summary>
        public abstract void PersistSettings();
    }
}
