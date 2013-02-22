using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using MVCFramework.Web.Infrastructure;
using System.Web.Mvc;
using MVCFramework.Web.Models;

namespace MVCFramework.Web.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        public JsonNetResult GetAllRoles()
        {
            var roleModels = new List<RoleModel>()
                                 {
                                     new RoleModel()
                                         {
                                             Title = "anonymous"
                                         }
                                 };

            roleModels.AddRange(Roles.GetAllRoles().Select(r => new RoleModel()
                                                                       {
                                                                           Name = r,
                                                                           Title = r
                                                                       }));

            return new JsonNetResult(roleModels);
        }
    }
}
