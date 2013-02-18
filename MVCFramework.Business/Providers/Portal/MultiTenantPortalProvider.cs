using System;
using System.Configuration.Provider;
using System.Web;
using MVCFramework.Business.Providers.Caching;
using MVCFramework.Business.Providers.NHibernateSession;
using MVCFramework.Business.Repository.Entities;

namespace MVCFramework.Business.Providers.Portal
{
    public class MultiTenantPortalProvider : PortalProviderBase
    {
        //private Model.Entities.Portal _portal;

        private string _applicationName;
        public string ApplicationName
        {
            get
            {
                if (!string.IsNullOrEmpty(_applicationName))
                    return _applicationName;

                if (HttpContext.Current != null)
                {
                    // get the request host
                    string host = HttpContext.Current.Request.Headers["Host"]
                        .Split(':')[0] // removes port from host
                        .Replace("www.", string.Empty); // removes www. from host

                    return host;
                }

                throw new ArgumentNullException("ApplicationName", "Could not determine the application name for the navigation provider.");
            }
            set { _applicationName = value; }
        }

        private PortalRepository PortalRepository
        {
            get
            {
                return new PortalRepository(NHibernateSessionProvider.Instance.CurrentSession);
            }
        }

        private Model.Entities.Portal _portal;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentException("config");

            if (string.IsNullOrEmpty(name))
                name = "MultiTenantPortalProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Multi-tenant navigation provider");
            }

            base.Initialize(name, config);

            string applicationName = config["applicationName"];

            if (!string.IsNullOrEmpty(applicationName))
                ApplicationName = applicationName;

            bool caching = GetConfigValue(config["caching"], false);

            CacheProviderBase cache = null;
            string cacheKey = string.Format("PORTAL:{0}", ApplicationName); // compose the cache key = constant + host

            if (caching)
            {
                cache = CacheProviderManager.Provider;
                _portal = cache.Get(cacheKey) as Model.Entities.Portal;
            }

            if (_portal == null) // if not in cache, request the portal from database
            {
                _portal = PortalRepository.GetPortalByUrl(ApplicationName) ?? PortalRepository.GetPortalByAliasUrl(ApplicationName);

                //if portal url was not found in database, look in portal aliases

                if (caching && _portal != null) // portal was found, cache it
                    cache.Insert(cacheKey, _portal);
            }

            if (_portal == null)
                throw new ProviderException(string.Format("Could not find portal for host '{0}'.", ApplicationName));
        }

        #region Provider helpers

        //
        // Helper functions to retrieve config values from the configuration file.
        //

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        private int GetConfigValue(string configValue, int defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return
                Convert.ToInt32(configValue);
        }

        private bool GetConfigValue(string configValue, bool defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return
                Convert.ToBoolean(configValue);
        }

        #endregion

        public override Model.Entities.Portal GetCurrentPortal()
        {
            return _portal;
        }
    }
}
