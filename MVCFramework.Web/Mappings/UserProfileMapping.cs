using System.Web.Profile;
using AutoMapper;
using MVCFramework.Web.Models;

namespace MVCFramework.Web.Mappings
{
    public class UserProfileMapping : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<ProfileBase, UserProfileModel>()
                  .ForAllMembers(m => m.ResolveUsing<ProfileValueResolver>());
        }


        public class ProfileValueResolver : IValueResolver
        {
            public ResolutionResult Resolve(ResolutionResult source)
            {
                return source.New(
                    ((ProfileBase)source.Value)
                    .GetPropertyValue(source.Context.MemberName)
                );
            }
        }
    }
}