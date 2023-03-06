using System.Collections.ObjectModel;

namespace Common.Toolkits.Entities
{
    public class MenuModel : ViewModelBase
    {
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChangedEvent(nameof(Title));
            }
        }

        private string icon;
        public string Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                RaisePropertyChangedEvent(nameof(Icon));
            }
        }

        private string regionName;
        public string RegionName
        {
            get { return regionName; }
            set
            {
                regionName = value;
                RaisePropertyChangedEvent(nameof(RegionName));
            }
        }

        private string viewName;
        public string ViewName
        {
            get { return viewName; }
            set
            {
                viewName = value;
                RaisePropertyChangedEvent(nameof(ViewName));
            }
        }

        private bool isEnable = true;
        public bool IsEnable
        {
            get { return isEnable; }
            set
            {
                isEnable = value;
                RaisePropertyChangedEvent(nameof(IsEnable));
            }
        }

        private bool isDefault;
        public bool IsDefault
        {
            get { return isDefault; }
            set
            {
                isDefault = value;
                RaisePropertyChangedEvent(nameof(IsDefault));
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChangedEvent(nameof(IsSelected));
            }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                RaisePropertyChangedEvent(nameof(IsExpanded));
            }
        }

        public ObservableCollection<MenuModel> children;
        public ObservableCollection<MenuModel> Children
        {
            get { return children; }
            set
            {
                children = value;
                RaisePropertyChangedEvent(nameof(Children));
            }
        }
    }
}
