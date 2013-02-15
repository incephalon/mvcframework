using NUnit.Framework;
using System.Web.Security;

namespace MVCFramework.Tests
{
    [TestFixture]
    public class MultiTenantRoleProviderSpecs
    {
        [Test]
        public void CheckIfUserIsInRole()
        {
            Assert.True(Roles.IsUserInRole("roxi", "administrator"));
        }

        [Test]
        public void GetUserRoles()
        {
            Roles.GetRolesForUser("roxi");
        }
    }
}
