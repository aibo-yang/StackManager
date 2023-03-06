using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Common.UI.WPF.CustomControls
{
    public class TreeViewLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TreeViewItem item = (TreeViewItem)value;
            var ctrl = ItemsControl.ItemsControlFromItemContainer(item);
            return ctrl.ItemContainerGenerator.IndexFromContainer(item) == ctrl.Items.Count - 1;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }

    public class CustomTreeView : TreeView
    {
        public static readonly DependencyProperty IsExpandedAllProperty =
            DependencyProperty.Register("IsExpandedAll",
                typeof(bool),
                typeof(TreeView),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsMultiSelectionProperty =
            DependencyProperty.Register("IsMultiSelection",
                typeof(bool),
                typeof(TreeView),
                new PropertyMetadata(false));

        static CustomTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTreeView), new FrameworkPropertyMetadata(typeof(CustomTreeView)));
        }

        public bool IsExpandedAll
        {
            get { return (bool)GetValue(IsExpandedAllProperty); }
            set { SetValue(IsExpandedAllProperty, value); }
        }

        public bool IsMultiSelection
        {
            get { return (bool)GetValue(IsMultiSelectionProperty); }
            set { SetValue(IsMultiSelectionProperty, value); }
        }
    }
}