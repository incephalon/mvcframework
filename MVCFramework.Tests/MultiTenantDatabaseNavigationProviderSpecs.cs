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
            var navs = NavigationProviderManager.Provider.GetUserNavigations("roxi");
            Assert.AreEqual(navs.Count, 2);
        }

    }
}
