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

            //Mapper.CreateMap<UserProfileModel, ProfileBase>()
            //      .ConvertUsing<ProfileTypeConverter>();
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

        //public class ProfileTypeConverter : ITypeConverter<UserProfileModel, ProfileBase>
        //{
        //    public ProfileBase Convert(ResolutionContext context)
        //    {
        //        var profile = (ProfileBase) context.DestinationValue;
        //        return profile;
        //    }
        //}
    }
}