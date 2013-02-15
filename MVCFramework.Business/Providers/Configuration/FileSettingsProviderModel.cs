using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MVCFramework.Business.Providers.Configuration
{
    [Serializable]
    [XmlRoot("Application")]
    public class FileSettingsProviderModel
    {
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlArray]
        public List<Configuration> Configurations
        {
            get;
            set;
        }

        public FileSettingsProviderModel()
        {
            Configurations = new List<Configuration>();
        }
    }

    [Serializable]
    public class Configuration
    {
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlArray]
        public List<Section> Sections
        {
            get;
            set;
        }

        public Configuration()
        {
            Sections = new List<Section>();
        }

        public Configuration(string name)
            : this()
        {
            Name = name;
        }
    }

    [Serializable]
    public class Section
    {
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlArray]
        public List<Setting> Settings
        {
            get;
            set;
        }

        public Section()
        {
            Settings = new List<Setting>();
        }

        public Section(string name) :
            this()
        {
            Name = name;
        }
    }

    [Serializable]
    public class Setting
    {
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Value
        {
            get;
            set;
        }

        public Setting()
        {

        }

        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
