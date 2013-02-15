using System;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using AutoMapper;
using MVCFramework.Business.Exceptions;
using MVCFramework.Web.Infrastructure;
using MVCFramework.Web.Models;
using Omu.ValueInjecter;

namespace MVCFramework.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            bool ajax = Request.IsAjaxRequest();
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1
                        && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return ajax ?
                          new JsonNetResult(new { redirectUrl = returnUrl }) : (ActionResult)Redirect(returnUrl);
                    }

                    // if no return url specified or it's invalid, go to dashboard
                    return ajax ?
                        new JsonNetResult(new { redirectUrl = Url.Action("Index", "Home") }) : (ActionResult)RedirectToAction("Index", "Home");
                }

                // if it did not already return from the above section, assume login failed
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            return ajax ?
               new JsonNetResult(new { message = "Failed to authenticate user." }) : (ActionResult)RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {

            bool ajax = Request.IsAjaxRequest();

            // attempt to register the user
            MembershipCreateStatus createStatus;
            Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);

                // create user's basic profile
                var profile = ProfileBase.Create(model.UserName, true);
                profile.InjectFrom<Mappings.ProfileBaseInjection>(new UserProfileModel()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email
                });
                profile.Save();

                Profile.Initialize(model.UserName, true);

                return ajax ?
                     new JsonNetResult(new { redirectUrl = Url.Action("Index", "Home") }) : (ActionResult)RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("MembershipCreateStatus", ErrorCodeToString(createStatus));

            // if we got this far, something failed, redisplay form
            return ajax ?
               new JsonNetResult(new
               {
                   message = string.Format("Failed to register user. {0}", ErrorCodeToString(createStatus))
               }) : (ActionResult)View(model);
        }

        #region membership status codes

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        public ActionResult UserProfile()
        {
            var model = Mapper.Map<ProfileBase, UserProfileModel>(Profile);
            return View(model);
        }

        public ActionResult EditUserProfile()
        {
            var model = Mapper.Map<ProfileBase, UserProfileModel>(Profile);
            return PartialView("Partials/_EditUserProfile", model);
        }

        [HttpPost]
        public ActionResult EditUserProfile(UserProfileModel model)
        {
            try
            {
                Profile.InjectFrom<Mappings.ProfileBaseInjection>(model);
                Profile.Save();
            }
            catch (Exception ex)
            {
                throw new BusinessException("Failed to save the profile.", ex);
            }
            //TODO: save the profile
            return PartialView("Partials/_UserProfile", model);
        }
    }
}
