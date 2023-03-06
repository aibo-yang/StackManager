using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Common.UI.WPF.Core.Utilities;

namespace Common.UI.WPF.VisualScreen
{
    public class FrameBannerListBox : ListBox
    {
        static FrameBannerListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameBannerListBox), new FrameworkPropertyMetadata(typeof(FrameBannerListBox)));
        }

        public static readonly DependencyProperty DisplayHeadersProperty =
            DependencyProperty.Register("DisplayHeaders", typeof(string), typeof(FrameBannerListBox), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnDisplayHeadersChanged)));

        public string DisplayHeaders
        {
            get { return (string)GetValue(DisplayHeadersProperty); }
            set { SetValue(DisplayHeadersProperty, value); }
        }

        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(FrameBannerListBox), new PropertyMetadata(15.0));

        public double HeaderFontSize
        {
            get { return (double)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }

        public static readonly DependencyProperty DisplayColumnNamesProperty =
            DependencyProperty.Register("DisplayColumnNames", typeof(string), typeof(FrameBannerListBox), new PropertyMetadata(string.Empty));

        public string DisplayColumnNames
        {
            get { return (string)GetValue(DisplayColumnNamesProperty); }
            set { SetValue(DisplayColumnNamesProperty, value); }
        }

        public static readonly DependencyProperty ColumnRowsProperty =
            DependencyProperty.Register("ColumnRows", typeof(int), typeof(FrameBannerListBox), new PropertyMetadata(4));

        public int ColumnRows
        {
            get { return (int)GetValue(ColumnRowsProperty); }
            set { SetValue(ColumnRowsProperty, value); }
        }

        public static readonly DependencyProperty ColumnFontSizeProperty =
            DependencyProperty.Register("ColumnFontSize", typeof(double), typeof(FrameBannerListBox), new PropertyMetadata(15.0));

        public double ColumnFontSize
        {
            get { return (double)GetValue(ColumnFontSizeProperty); }
            set { SetValue(ColumnFontSizeProperty, value); }
        }

        public static readonly DependencyProperty HeaderWidthProperty =
            DependencyProperty.Register("HeaderWidth", typeof(double), typeof(FrameBannerListBox), new PropertyMetadata(60.0));

        public double HeaderWidth
        {
            get { return (double)GetValue(HeaderWidthProperty); }
            set { SetValue(HeaderWidthProperty, value); }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FrameBannerListBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var frameItem = new FrameBannerListBoxItem();
            frameItem.Loaded += (s, e) => BuildListItemChildren(this, frameItem);
            return frameItem;
        }

        private static void OnDisplayHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameBannerListBox frameList)
            {
                return;
            }

            if (frameList.IsLoaded)
            {
                BuildListHeaderChildren(d as FrameBannerListBox);
            }
            else
            {
                frameList.Loaded += (s, e)=> BuildListHeaderChildren(s as FrameBannerListBox);
            }
        }

        private static void BuildListHeaderChildren(FrameBannerListBox frameList)
        {
            var headerPanel = UIHelper.FindVisualChild<UniformGrid>(frameList, "PART_ListHeaderPanel");
            if (headerPanel == null)
            {
                return;
            }

            headerPanel.Children.Clear();
            var headerNames = frameList.DisplayHeaders.Split(",");

            for (int i = 0; i < headerNames.Length; i++)
            {
                var contentControl = new ContentControl
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.White,
                    Content = headerNames[i].Trim(),
                };

                BindingOperations.SetBinding(contentControl, ContentControl.FontSizeProperty, new Binding("HeaderFontSize")
                {
                    Source = frameList,
                });

                var border = new Border
                {
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    BorderBrush = Brushes.SkyBlue,
                    Opacity = 0.2
                };

                var grid = new Grid();
                grid.Children.Add(border);
                grid.Children.Add(contentControl);

                headerPanel.Children.Add(grid);
            }
        }

        private void BuildListItemChildren(FrameBannerListBox frameList, FrameBannerListBoxItem frameListItem)
        {
            var itemsPanel = UIHelper.FindVisualChild<UniformGrid>(frameListItem, "PART_ListItemsPanel");
            if (itemsPanel == null)
            {
                return;
            }

            itemsPanel.Children.Clear();
            var columnNames = DisplayColumnNames.Split(",");

            for (int i = 0; i < columnNames.Length; i++)
            {
                var contentControl = new ContentControl
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.SkyBlue,
                };

                BindingOperations.SetBinding(contentControl, ContentControl.FontSizeProperty, new Binding("ColumnFontSize")
                {
                    Source = frameList,
                });

                BindingOperations.SetBinding(contentControl, ContentControl.ContentProperty, new Binding(columnNames[i].Trim())
                {
                    Source = frameListItem.DataContext,
                });

                var border = new Border
                {
                    BorderThickness = new Thickness(1,1,1,1),
                    BorderBrush = Brushes.SkyBlue,
                    Opacity = 0.2
                };

                var grid = new Grid() { Name = $"PART_Column_{columnNames[i].Trim()}" };
                grid.Children.Add(border);
                grid.Children.Add(contentControl);

                itemsPanel.Children.Add(grid);
            }
        }
    }
}