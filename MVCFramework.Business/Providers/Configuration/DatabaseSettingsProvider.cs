using System;
using System.Collections.Generic;
using System.Linq;
using MVCFramework.Business.Helpers;
using MVCFramework.Business.Providers.NHibernateSession;
using MVCFramework.Business.Repository.Entities;

namespace MVCFramework.Business.Providers.Configuration
{
    public class DatabaseSettingsProvider : SettingsProviderBase
    {

        private ConfigurationRepository ConfigurationRepository
        {
            get { return new ConfigurationRepository(NHibernateSessionProvider.Instance.CurrentSession); }
        }

        public override IEnumerable<string> GetConfigurations()
        {
            return ConfigurationRepository.GetConfigurations()
                .Select(c => c.Name);
        }

        public override void EnsureConfiguration(string configuration)
        {
            ConfigurationRepository.CreateConfigurationIfNotExists(configuration);
        }

        public override IEnumerable<string> GetSections(string configuration)
        {
            //var config = EnumHelper<Model.Enums.Configuration>.GetEnumValue(configuration);
            //return ConfigSectionManager.Instance.GetByConfiguration(config).Select(s => s.Name);

            throw new NotImplementedException();
        }

        public override void EnsureSection(string configuration, string section)
        {
            //var config = EnumHelper<Model.Enums.Configuration>.GetEnumValue(configuration);
            //ConfigSectionManager.Instance.CreateIfNotExists(config, section);

            throw new NotImplementedException();
        }

        public override IDictionary<string, string> GetSettings(string configuration, string section)
        {
            //var config = EnumHelper<Model.Enums.Configuration>.GetEnumValue(configuration);
            //return ConfigSettingManager.Instance.GetSettings(config, section);

            throw new NotImplementedException();
        }

        public override string GetSetting(string configuration, string section, string setting)
        {
            //var config = EnumHelper<Model.Enums.Configuration>.GetEnumValue(configuration);
            //ConfigSetting configSetting = ConfigSettingManager.Instance.GetSetting(config, section, setting);
            //return configSetting == null ? null : configSetting.DefaultValue;

            throw new NotImplementedException();
        }

        public override IDictionary<string, string> GetUserSettings(int userID, string configuration, string section)
        {
            //var config = EnumHelper<Model.Enums.Configuration>.GetEnumValue(configuration);
            //return ConfigSettingManager.Instance.GetUserSettings(userID, config, section).ToDictionary(k => k.Setting.Name, v => v.Value);

            throw new NotImplementedException();
        }

        public override string GetUserSetting(int userID, string configuration, string section, string setting)
        {
            //var config = EnumHelper<Model.Enums.Configuration>.GetEnumValue(configuration);
            //UserConfigSetting userConfigSetting = ConfigSettingManager.Instance.GetUserSetting(userID, config, section, setting);
            //return userConfigSetting == null ? null : userConfigSetting.Value;

            throw new NotImplementedException();
        }

        public override IDictionary<string, string> GetTenantSettings(int tenantID, string configuration, string section)
        {
            //var config = EnumHelper<Model.Enums.Configuration>.GetEnumValue(configuration);
            //return ConfigSettingManager.Instance.GetTenantSettingsList(tenantID, config, section).ToDictionary(k => k.Setting.Name, v => v.Value);

            throw new NotImplementedException();
        }

        public override string GetTenantSetting(int tenantID, string configuration, string section, string setting)
        {
            //var config = EnumHelper<FieldService.Model.Enums.Configuration>.GetEnumValue(configuration);
            //TenantConfigSetting tenantConfigSetting = ConfigSettingManager.Instance.GetTenantSetting(tenantID, config, section, setting);
            //return tenantConfigSetting == null ? null : tenantConfigSetting.Value;

            throw new NotImplementedException();
        }

        public override void SetSetting(string configuration, string section, string setting, string value)
        {
            //var config = EnumHelper<Model.Enums.Configuration>.GetEnumValue(configuration);
            //ConfigSettingManager.Instance.SetSettingValue(config, section, setting, value);

            throw new NotImplementedException();
        }

        public override void SetUserSetting(int userID, string configuration, string section, string setting, string value)
        {
            //var config = EnumHelper<Model.Enums.Configuration>.GetEnumValue(configuration);
            //ConfigSettingManager.Instance.SetUserSettingValue(userID, config, section, setting, value);

            throw new NotImplementedException();
        }

        public override void SetTenantSetting(int tenantID, string configuration, string section, string setting, string value)
        {
            //var config = EnumHelper<Model.Enums.Configuration>.GetEnumValue(configuration);
            //ConfigSettingManager.Instance.SetTenantSettingValue(tenantID, config, section, setting, value);

            throw new NotImplementedException();
        }

        public override void PersistSettings()
        {
            throw new NotSupportedException("This provider persists settings imediately and does not implement this method.");
        }

    }
}
