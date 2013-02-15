using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using MVCFramework.Business.Exceptions;

namespace MVCFramework.Business.Providers.Configuration
{
    public class FileSettingsProvider : SettingsProviderBase
    {
        protected string Application
        {
            get;
            private set;
        }

        protected string File
        {
            get;
            private set;
        }

        protected string Location
        {
            get;
            private set;
        }

        private string Path
        {
            get
            {
                string path = string.IsNullOrEmpty(Location)
                                  ? Application
                                  : System.IO.Path.Combine(Location, Application);
                return System.IO.Path.Combine(path, File);
            }
        }

        public FileSettingsProviderModel Settings
        {
            get;
            private set;
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            if (config["file"] == null)
                throw new ProviderException("File Settings Provider: No file specified for settings persistence.");
            File = config["file"];

            if (config["application"] == null)
                throw new ProviderException("File Settings Provider: No application name specified.");
            Application = config["application"];

            if (config["location"] != null) // if a location for the file is specified...
            {
                string location = config["location"];

                // see if is a special folder
                Environment.SpecialFolder specialFolder;
                Location = Enum.TryParse<Environment.SpecialFolder>(location, true, out specialFolder) ? Environment.GetFolderPath(specialFolder) : location;
            }

            // ensure the file exists
            try
            {
                if (!System.IO.File.Exists(Path))
                {
                    FileInfo file = new FileInfo(System.IO.Path.GetFullPath(Path));
                    file.Directory.Create();
                    Settings = new FileSettingsProviderModel();
                }
                else // load settings
                {
                    XmlSerializer serializer = new XmlSerializerFactory().CreateSerializer(typeof(FileSettingsProviderModel));
                    using (var stream = System.IO.File.OpenRead(Path))
                    {
                        XmlReader reader = XmlReader.Create(stream);
                        Settings = serializer.Deserialize(reader) as FileSettingsProviderModel;
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Could not load or create the settings file.", ex);
            }
        }

        public override void PersistSettings()
        {
            try
            {
                using (var stream = System.IO.File.Open(Path, System.IO.FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializerFactory().CreateSerializer(typeof(FileSettingsProviderModel));
                    serializer.Serialize(stream, Settings);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Could not persist settings file.", ex);
            }
        }

        public override IEnumerable<string> GetConfigurations()
        {
            return Settings.Configurations.Select(c=>c.Name);
        }

        public override void EnsureConfiguration(string configuration)
        {
            var cfg = Settings.Configurations.SingleOrDefault(c => c.Name == configuration);
            if (cfg == null)
                Settings.Configurations.Add(new Configuration(configuration));
        }

        public override IEnumerable<string> GetSections(string configuration)
        {
            var cfg = Settings.Configurations.SingleOrDefault(c => c.Name == configuration);
            if (cfg != null)
                return cfg.Sections.Select(s => s.Name);

            return null;
        }

        public override void EnsureSection(string configuration, string section)
        {
            var cfg = Settings.Configurations.SingleOrDefault(c => c.Name == configuration);
            if (cfg == null)
                throw new BusinessException(string.Format("Invalid configuration: {0}", configuration));

            var sct = cfg.Sections.SingleOrDefault(s => s.Name == section);

            if (sct == null)
                cfg.Sections.Add(new Section(section));
        }

        public override IDictionary<string, string> GetSettings(string configuration, string section)
        {
            var cfg = Settings.Configurations.SingleOrDefault(c => c.Name == configuration);
            if (cfg != null)
            {
                var sct = cfg.Sections.SingleOrDefault(sc => sc.Name == section);
                if (sct != null)
                    return sct.Settings.ToDictionary<Setting, string, string>(k => k.Name, v => v.Value);
            }

            return null;
        }

        public override string GetSetting(string configuration, string section, string setting)
        {
            var cfg = Settings.Configurations.SingleOrDefault(c => c.Name == configuration);
            if (cfg != null)
            {
                var sct = cfg.Sections.SingleOrDefault(sc => sc.Name == section);
                if (sct != null)
                {
                    var stg = sct.Settings.SingleOrDefault(st => st.Name == setting);
                    if (stg != null)
                        return stg.Value;
                }
            }

            return null;
        }

        public override void SetSetting(string configuration, string section, string setting, string value)
        {
            var cfg = Settings.Configurations.SingleOrDefault(c => c.Name == configuration);
            if (cfg == null)
            {
                cfg = new Configuration(configuration);
                Settings.Configurations.Add(cfg);
            }

            var sct = cfg.Sections.SingleOrDefault(sc => sc.Name == section);
            if (sct == null)
            {
                sct = new Section(section);
                cfg.Sections.Add(sct);
            }

            var stg = sct.Settings.SingleOrDefault(st => st.Name == setting);
            if (stg == null)
            {
                stg = new Setting(setting, value);
                sct.Settings.Add(stg);
            }
            else
                stg.Value = value;
        }

        public override IDictionary<string, string> GetUserSettings(int userID, string configuration, string section)
        {
            throw new NotSupportedException("User specific configuration is not supported by this provider.");
        }

        public override string GetUserSetting(int userID, string configuration, string section, string setting)
        {
            throw new NotSupportedException("User specific configuration is not supported by this provider.");
        }

        public override void SetUserSetting(int userID, string configuration, string section, string setting, string value)
        {
            throw new NotSupportedException("User specific configuration is not supported by this provider.");
        }

        public override IDictionary<string, string> GetTenantSettings(int tenantID, string configuration, string section)
        {
            throw new NotSupportedException("Tenant specific configuration is not supported by this provider.");
        }

        public override string GetTenantSetting(int tenantID, string configuration, string section, string setting)
        {
            throw new NotSupportedException("Tenant specific configuration is not supported by this provider.");
        }

        public override void SetTenantSetting(int tenantID, string configuration, string section, string setting, string value)
        {
            throw new NotSupportedException("Tenant specific configuration is not supported by this provider.");
        }
    }
}
