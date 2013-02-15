
using System.Configuration.Provider;
using MVCFramework.Business.Providers.Caching;
using MVCFramework.Business.Providers.NHibernateSession;
using MVCFramework.Business.Repository.Entities;

namespace MVCFramework.Business.Providers.Portal
{
    public class PortalProvider : IPortalProvider
    {
        private Model.Entities.Portal _portal;


        private PortalRepository PortalRepository
        {
            get
            {
                return new PortalRepository(NHibernateSessionProvider.Instance.CurrentSession);
            }
        }

        public void Initialize(string host, string cacheKey)
        {
            bool caching = !string.IsNullOrEmpty(cacheKey);
            CacheProviderBase cache = null;

            if (caching)
            {
                cache = CacheProviderManager.Provider;
                cacheKey = string.Format("{0}:{1}", cacheKey, host); // compose the cache key = constant + host
                _portal = cache.Get(cacheKey) as Model.Entities.Portal;
            }

            if (_portal == null) // if not in cache, request the portal from database
            {
                _portal = PortalRepository.GetPortalByUrl(host) ?? PortalRepository.GetPortalByAliasUrl(host);

                //if portal url was not found in database, look in portal aliases

                if (caching && _portal != null) // portal was found, cache it
                    cache.Insert(cacheKey, _portal);
                return;
            }

            throw new ProviderException(string.Format("Could not find portal for host '{0}'.", host));
        }

        public Model.Entities.Portal GetCurrentPortal()
        {
            return _portal;
        }

    }
}
