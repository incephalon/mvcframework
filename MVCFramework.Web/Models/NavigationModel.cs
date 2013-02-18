using System.Collections.Generic;
using AutoMapper;
using MVCFramework.Business.Providers.Navigation;
using MVCFramework.Model.Entities;

namespace MVCFramework.Web.Models
{
    public class NavigationModel
    {
        public List<NavigationItemModel> Items { get; set; }

        public NavigationModel()
        {
            Items = new List<NavigationItemModel>();
        }
    }

    public class NavigationItemModel
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }

        public string Text { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int Order { get; set; }
        public bool ShowInMenu { get; set; }

        public bool Selected { get; set; }
    }

    public sealed class NavigationModelFactory
    {
        public static NavigationModel GetDefaultNavigationModel()
        {
            Navigation defaultNavigation = NavigationProviderManager.Provider.GetDefaultNavigation();
            NavigationModel model = Mapper.Map<Navigation, NavigationModel>(defaultNavigation);

            return model;
        }

        public static List<NavigationModel> GetNavigationModel(string username)
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