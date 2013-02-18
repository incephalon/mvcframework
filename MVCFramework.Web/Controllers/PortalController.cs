using System;
using System.IO;
using System.Web.Mvc;
using MVCFramework.Business.Providers.Logging;
using MVCFramework.Business.Providers.Portal;

namespace MVCFramework.Web.Controllers
{
    public class PortalController : Controller
    {
        /// <summary>
        /// Returns the logged-in user ID. 
        /// Returns -1 if no user is logged in or an error occurs while retrieving the logged in user's ID.
        /// </summary>
        protected int LoggedInUserID //TODO: make this singleton
        {
            get
            {
                try
                {
                    var loggedInUser = System.Web.Security.Membership.GetUser();

                    return loggedInUser != null && loggedInUser.ProviderUserKey != null
                               ? (int)loggedInUser.ProviderUserKey
                               : -1;
                }
                catch (ArgumentException)
                {
                    return -1;
                }
            }
        }

        protected string LogEntry(string message, Exception exception, string source, string type, int code)
        {
            string user = HttpContext.User.Identity.Name;

            return LoggingProviderManager.Provider.LogEntry(user, source, type, code, message, exception);
        }

        protected string LogEntry(string message, Exception exception)
        {
            var source = string.Empty;
            var type = string.Empty;
            const int code = 0;

            return LogEntry(message, exception, source, type, code);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.Portal = Portal;

            if (Portal == null)
                filterContext.Result = RedirectToAction("Index", "Error");

            base.OnActionExecuting(filterContext);
        }

        public Model.Entities.Portal Portal
        {
            get
            {
                return PortalProviderManager.Provider.GetCurrentPortal();
            }
        }

        public string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
