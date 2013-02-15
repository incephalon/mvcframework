using System.Web;
using System.Web.Security;

namespace MVCFramework.Web.Infrastructure
{
    public class ProviderInitializationHttpModule : IHttpModule
    {
        public ProviderInitializationHttpModule(MembershipProvider membershipProvider)
        {
        }

        public void Init(HttpApplication context)
        {
        }

        public void Dispose()
        {
        }
    }
}