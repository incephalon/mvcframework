using System.Collections.Generic;

namespace MVCFramework.Web.Models
{
    public class NavigationModel
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public string Role { get; set; }

        public List<NavigationItemModel> Items { get; set; }

        public NavigationModel()
        {
            Items = new List<NavigationItemModel>();
        }
    }

    public class NavigationItemModel
    {
        public int ID { get; set; }
        public int NavigationID { get; set; }
        public int? ParentID { get; set; }

        public string Text { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int Order { get; set; }
        public bool ShowInMenu { get; set; }

        public bool Selected { get; set; }
    }
}