using System.Collections.Generic;
using AutoMapper;
using MVCFramework.Business.Providers.Navigation;
using MVCFramework.Model.Entities;
using MVCFramework.Web.Models;

namespace MVCFramework.Web.Helpers
{
    public sealed class NavigationModelHelper
    {
        public static NavigationModel GetDefaultNavigationModel()
        {
            Navigation defaultNavigation = NavigationProviderManager.Provider.GetDefaultNavigation();
            NavigationModel model = Mapper.Map<Navigation, NavigationModel>(defaultNavigation);

            return model;
        }

        public static NavigationModel GetRoleNavigationModel(string rolename)
        {
            Navigation roleNavigation = NavigationProviderManager.Provider.GetRoleNavigation(rolename);
            NavigationModel model = Mapper.Map<Navigation, NavigationModel>(roleNavigation);

            return model;
        }

        public static List<NavigationModel> GetUserNavigationModels(string username)
        {
            var navigation = NavigationProviderManager.Provider.GetUserNavigations(username);
            var model = Mapper.Map<List<Navigation>, List<NavigationModel>>(navigation);

            return model;
        }

        public static NavigationModel MergeNavigationModels(NavigationModel source, NavigationModel destination)
        {
            //TODO: for now, just assume the 'source' items do not colide with the 'destination' items, and just add them to root of the navigation
            if (source != null)
                foreach (var item in source.Items)
                    destination.Items.Add(item);

            return destination;
        }
    }
}