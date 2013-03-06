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

                if (unordered != null)
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
            using (TransactionScope ts = new TransactionScope())
            {

                var item = _navigationRepository.FindByID(model.ID);
                int oldOrder = item.Order;
                int newOrder = model.Order;


                dynamic unordered = null;

                // if order has changed, update order for items in between
                _navigationRepository.BeginTransaction();
                if (newOrder < oldOrder) // order decreased
                {
                    unordered = _navigationRepository.FilterBy(
                        ni =>
                        ni.Navigation.ID == model.NavigationID &&
                        ni.ParentItem.ID == model.ParentID &&
                        ni.Order >= newOrder && ni.Order < oldOrder
                        );

                    if (unordered != null)
                        foreach (var uitem in unordered)
                            uitem.Order++;
                }
                else if (newOrder > oldOrder) // order increased
                {
                    unordered = _navigationRepository.FilterBy(
                        ni =>
                        ni.Navigation.ID == model.NavigationID &&
                        ni.ParentItem.ID == model.ParentID &&
                        ni.Order > oldOrder && ni.Order <= newOrder
                        );

                    if (unordered != null)
                        foreach (var uitem in unordered)
                            uitem.Order--;
                }
                _navigationRepository.CommitTransaction();

                if (unordered != null)
                    _navigationRepository.Update(unordered);

                Mapper.Map(model, item);
                _navigationRepository.Update(item);

                ts.Complete();
            }

            return new JsonNetResult(model);
        }

        [HttpDelete]
        public JsonNetResult DeleteNavigationItem(int id)
        {

            using (TransactionScope ts = new TransactionScope())
            {
                var item = _navigationRepository.FindByID(id);
                var model = Mapper.Map<NavigationItem, NavigationItemModel>(item);

                // delete children
                _navigationRepository.BeginTransaction();
                var children = _navigationRepository.FilterBy(
                    ni =>
                        ni.ParentItem.ID == model.ID);
                _navigationRepository.CommitTransaction();

                _navigationRepository.Delete(children);

                // update siblings order
                _navigationRepository.BeginTransaction();
                var unordered = _navigationRepository.FilterBy(
                       ni =>
                       ni.Navigation.ID == model.NavigationID && ni.Order > model.Order &&
                       ni.ParentItem.ID == model.ParentID);

                if (unordered != null)
                    foreach (var uitem in unordered)
                        uitem.Order--;

                _navigationRepository.CommitTransaction();

                if (unordered != null)
                    _navigationRepository.Update(unordered);

                // delete item
                _navigationRepository.Delete(item);

                ts.Complete();
            }

            return new JsonNetResult("deleted");
        }

    }
}
