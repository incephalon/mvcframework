namespace MVCFramework.Business.Providers.Navigation
{
    public abstract class NavigationProviderBase : System.Configuration.Provider.ProviderBase
    {
        public abstract Model.Entities.Navigation GetNavigation();
        public abstract Model.Entities.Navigation GetNavigation(string username, string portal);
    }
}
