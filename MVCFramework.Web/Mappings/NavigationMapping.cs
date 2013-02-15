using AutoMapper;
using MVCFramework.Model.Entities;
using MVCFramework.Web.Infrastructure;
using MVCFramework.Web.Models;

namespace MVCFramework.Web.Mappings
{
    public class NavigationMappings : Profile
    {
        protected override void Configure()
        {
            Mapper
                .CreateMap<NavigationItem, NavigationItemModel>()
               
                .IgnoreAllNonExisting();

            Mapper
                .CreateMap<Navigation, NavigationModel>();

        }
    }
}