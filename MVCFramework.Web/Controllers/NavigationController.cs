using AutoMapper;
using MVCFramework.Business.Repository;
using MVCFramework.Model.Entities;
using MVCFramework.Web.Helpers;
using MVCFramework.Web.Infrastructure;
using MVCFramework.Web.Models;
using Ninject;
using System.Web.Mvc;

namespace MVCFramework.Web.Controllers
{
    [Authorize]
    public class NavigationController : Controller
    {
        private readonly IKeyedRepository<int, NavigationItem> _navigationRepository;

        public NavigationController(IKeyedRepository<int, NavigationItem> navigationRepository)
        {
            this._navigationRepository = navigationRepository;
        }
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

            return new JsonNetResult(model);
        }

        [HttpPost]
        public JsonNetResult CreateNavigationItem(NavigationItemModel model)
        {
            var item = Mapper.Map<NavigationItemModel, NavigationItem>(model);
            _navigationRepository.Insert(item);
            Mapper.Map(item, model);

            return new JsonNetResult(model);
        }

        [HttpPut]
        public JsonNetResult UpdateNavigationItem(NavigationItemModel model)
        {
            var item = Mapper.Map<NavigationItemModel, NavigationItem>(model);
            _navigationRepository.Update(item);

            return new JsonNetResult(model);
        }

        [HttpDelete]
        public JsonNetResult DeleteNavigationItem(int id)
        {
            _navigationRepository.Delete(new NavigationItem { ID = id });
            return new JsonNetResult("deleted");
        }

    }
}
