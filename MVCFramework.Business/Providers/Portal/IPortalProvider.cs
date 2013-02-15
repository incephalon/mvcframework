namespace MVCFramework.Business.Providers.Portal
{
    public interface IPortalProvider
    {
        /// <summary>
        /// Initialize the site for the current request.
        /// </summary>
        /// <param name="host">The host name</param>
        /// <param name="cacheKey">The cache key where the portal is stored</param>
        void Initialize(string host, string cacheKey);
        Model.Entities.Portal GetCurrentPortal();
    }
}
