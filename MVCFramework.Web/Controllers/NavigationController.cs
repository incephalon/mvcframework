using System.Transactions;
using AutoMapper;
using MVCFramework.Business.Repository;
using MVCFramework.Model.Entities;
using MVCFramework.Web.Helpers;
using MVCFramework.Web.Infrastructure;
using MVCFramework.Web.Models;
using System.Web.Mvc;

namespace MVCFramework.Web.Controllers
{
    [Authorize]
    public class NavigationController : Controller
    {
        private readonly IKeyedRepository<int, NavigationItem> _navigationRepository;

        public NavigationController(IKeyedRepository<int, NavigationItem> navigationRepository)
        {
            _navigationRepository = navigationRepository;
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

            using (TransactionScope ts = new TransactionScope())
            {
                _navigationRepository.BeginTransaction();
                var unordered = _navigationRepository.FilterBy(
                        ni =>
                        ni.Navigation.ID == model.NavigationID && ni.Order >= model.Order &&
                        ni.ParentItem.ID == model.ParentID);
              
                if (unordered != null)
                    foreach (var uitem in unordered)
                        uitem.Order++;

                _navigationRepository.CommitTransaction();

                _navigationRepository.Update(unordered);
                _navigationRepository.Insert(item);

                ts.Complete();
            }

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

            using (TransactionScope ts = new TransactionScope())
            {
                var item = _navigationRepository.FindByID(id);
                var model = Mapper.Map<NavigationItem, NavigationItemModel>(item);

                _navigationRepository.BeginTransaction();
                var unordered = _navigationRepository.FilterBy(
                       ni =>
                       ni.Navigation.ID == model.NavigationID && ni.Order > model.Order &&
                       ni.ParentItem.ID == model.ParentID);

                if (unordered != null)
                    foreach (var uitem in unordered)
                        uitem.Order--;

                _navigationRepository.CommitTransaction();

                _navigationRepository.Update(unordered);
                _navigationRepository.Delete(item);

                ts.Complete();
            }
            
            return new JsonNetResult("deleted");
        }

    }
}
