using System.Collections.Generic;

namespace MVCFramework.Business.Providers.Navigation
{
    public abstract class NavigationProviderBase : System.Configuration.Provider.ProviderBase
    {
        public abstract Model.Entities.Navigation GetDefaultNavigation();
        public abstract List<Model.Entities.Navigation> GetUserNavigations(string username);
    }
}
