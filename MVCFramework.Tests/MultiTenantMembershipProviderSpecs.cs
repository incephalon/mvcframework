using System.Web.Security;
using NUnit.Framework;

namespace MVCFramework.Tests
{
    [TestFixture]
    public class MultiTenantMembershipProviderSpecs
    {
        [Test]
        public void CanInitializeMultiTenantMembershipProvider()
        {
            Membership.GetUserNameByEmail("cduta@evoshell.com");
        }
    }
}