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

    }
}
