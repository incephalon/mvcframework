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

                throw new ArgumentNullException("ApplicationName", "Could not determine the application name for the membership provider.");
            }
            set { _applicationName = value; }
        }


        private PortalProvider _portalProvider;
        private PortalProvider PortalProvider
        {
            get
            {
                if (_portalProvider == null)
                {
                    _portalProvider = new PortalProvider();
                    _portalProvider.Initialize(ApplicationName, ConfigurationKeys.PortalCache.ToString());
                }
                return _portalProvider;
            }
        }

        private NavigationRepository NavigationRepository
        {
            get { return new NavigationRepository(NHibernateSessionProvider.Instance.CurrentSession); }
        }

        public override Model.Entities.Navigation GetNavigation()
        {
            var navigation = NavigationRepository.GetDefaultNavigation(PortalProvider.GetCurrentPortal().Tenant.ID);
            if (navigation == null)
                throw new BusinessException("Default navigation is not defined.");

            return navigation;
        }

        public override Model.Entities.Navigation GetNavigation(string username, string portal)
        {
            var navigation = NavigationRepository.GetNavigation(username, PortalProvider.GetCurrentPortal().Tenant.ID);

            return navigation;
        }

    }
}
