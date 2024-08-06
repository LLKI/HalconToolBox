using Prism.Mvvm;
using System.Collections.ObjectModel;


namespace HalconToolBox.Models
{
	/// <summary>
	/// 菜单功能项模型
	/// </summary>
    public class NavigationItem:BindableBase
    {
		public NavigationItem(string icon,string name,string pageName,ObservableCollection<NavigationItem> items = null)
		{
			Icon = icon;
			Name = name;
			PageName = pageName;
			Items = items;
		}
        private string name { get; set; }
		private string icon;
		private string pageName;
		private ObservableCollection<NavigationItem> items;

		/// <summary>
		/// 菜单子项
		/// </summary>
		public ObservableCollection<NavigationItem> Items
		{
			get { return items; }
			set { items = value;RaisePropertyChanged(); }
		}

		/// <summary>
		/// 菜单指向页面名称
		/// </summary>
		public string PageName
		{
			get { return pageName; }
			set { pageName = value;RaisePropertyChanged(); }
		}
		/// <summary>
		/// 图标
		/// </summary>
		public string Icon
		{
			get { return icon; }
			set { icon = value; RaisePropertyChanged(); }
		}
		/// <summary>
		/// 菜单名称
		/// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }

    }
}
