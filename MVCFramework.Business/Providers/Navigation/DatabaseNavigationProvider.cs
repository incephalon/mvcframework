using System.Collections.Generic;
using System.Web;
using MVCFramework.Business.Exceptions;
using System;
using MVCFramework.Business.Providers.NHibernateSession;
using MVCFramework.Business.Providers.Portal;
using MVCFramework.Business.Repository.Entities;

namespace MVCFramework.Business.Providers.Navigation
{
    public class DatabaseNavigationProvider : NavigationProviderBase
    {
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

        private NavigationRepository NavigationRepository
        {
            get { return new NavigationRepository(NHibernateSessionProvider.Instance.CurrentSession); }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentException("config");

            if (string.IsNullOrEmpty(name))
                name = "DatabaseNavigationProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Database navigation provider");
            }

            base.Initialize(name, config);

            string applicationName = config["applicationName"];

            if (!string.IsNullOrEmpty(applicationName))
                ApplicationName = applicationName;
        }

        public override Model.Entities.Navigation GetDefaultNavigation()
        {
            var navigation = NavigationRepository.GetDefaultNavigation(PortalProviderManager.Provider.GetCurrentPortal().Tenant.ID);

            if (navigation == null)
                throw new BusinessException("Default navigation is not defined.");

            return navigation;
        }

        public override Model.Entities.Navigation GetRoleNavigation(string role)
        {
            var navigation = NavigationRepository.GetRoleNavigation(role, PortalProviderManager.Provider.GetCurrentPortal().Tenant.ID);
            return navigation;
        }

        public override List<Model.Entities.Navigation> GetUserNavigations(string username)
        {
            var navigation = NavigationRepository.GetUserNavigations(username, PortalProviderManager.Provider.GetCurrentPortal().Tenant.ID);

            return navigation;
        }

    }
}
