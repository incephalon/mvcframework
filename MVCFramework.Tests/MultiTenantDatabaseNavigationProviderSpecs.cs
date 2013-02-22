using MVCFramework.Business.Providers.Navigation;
using NUnit.Framework;

namespace MVCFramework.Tests
{
    [TestFixture]
    public class MultiTenantDatabaseNavigationProviderSpecs
    {
        [Test]
        public void GetDefaultNavigation()
        {
            NavigationProviderManager.Provider.GetDefaultNavigation();
        }

        [Test]
        public void GetUserNavigation()
        {
            var navs = NavigationProviderManager.Provider.GetUserNavigations("master");
            Assert.AreEqual(navs.Count, 2);
        }

        [Test]
        public void GetRoleNavigation()
        {
            var nav = NavigationProviderManager.Provider.GetRoleNavigation("administrator");
            Assert.IsNotNull(nav);
        }


    }
}
