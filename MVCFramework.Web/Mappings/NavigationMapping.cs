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
                .ForMember(m => m.NavigationID, o => o.MapFrom(s => s.Navigation.ID))
                .ForMember(m => m.ParentID, o => o.MapFrom(s => s.ParentItem.ID))
                .IgnoreAllNonExisting();

            Mapper
                .CreateMap<Navigation, NavigationModel>()
                .ForMember(m => m.Role, o => o.MapFrom(s => s.Role.Name));

            Mapper.CreateMap<NavigationItemModel, NavigationItem>()
                .ConstructUsing(s => new NavigationItem()
                                       {
                                           Navigation = new Navigation() { ID = s.NavigationID },
                                           ParentItem = s.ParentID.HasValue ? new NavigationItem()
                                                                                    {
                                                                                        ID = s.ParentID.Value
                                                                                    } : null
                                       });
        }
    }
}