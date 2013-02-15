using System.Web.Mvc;
using MVCFramework.Business.Repository.Entities;

namespace MVCFramework.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepository _userRepository;

        public HomeController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
         

            return View();
        }

    }
}
