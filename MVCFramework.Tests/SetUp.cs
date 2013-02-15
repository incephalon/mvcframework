using Ninject;
using NUnit.Framework;
using System.Web.Security;

namespace MVCFramework.Tests
{
    [SetUpFixture]
    public class SetUp
    {
        [SetUp]
        public void RunBeforeAnyTests()
        {
            var kernel = new StandardKernel(new NinjectSettings() { InjectNonPublic = true });
            kernel.Load(new Business.Modules.NHibernateRepositoryModule());
            kernel.Inject(Membership.Provider);
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            // nothing for now
        }
    }
}
