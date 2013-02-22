using AutoMapper;
using MVCFramework.Model.Entities;
using MVCFramework.Web.Models;

namespace MVCFramework.Web.Mappings
{
    public class RoleMappings : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Role, RoleModel>();
        }
    }
}