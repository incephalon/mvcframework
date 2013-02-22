using MVCFramework.Web.Helpers;
using MVCFramework.Web.Infrastructure;
using System.Web.Mvc;

namespace MVCFramework.Web.Controllers
{
    [Authorize]
    public class NavigationController : Controller
    {
        //
        // GET: /Navigation/

        public ActionResult Index()
        {
            return View();
        }

        public JsonNetResult GetNavigationForRole(string role)
        {
            var model = role == "anonymous" 
                ? NavigationModelHelper.GetDefaultNavigationModel()
                : NavigationModelHelper.GetRoleNavigationModel(role);

            return new JsonNetResult(model.Items);
        }

    }
}
