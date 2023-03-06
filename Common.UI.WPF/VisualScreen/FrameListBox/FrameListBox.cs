using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Common.UI.WPF.Core.Utilities;

namespace Common.UI.WPF.VisualScreen
{
    public class FrameListBox : ListBox
    {
        static FrameListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameListBox), new FrameworkPropertyMetadata(typeof(FrameListBox)));
        }

        public static readonly DependencyProperty DisplayHeadersProperty =
            DependencyProperty.Register("DisplayHeaders", typeof(string), typeof(FrameListBox), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnDisplayHeadersChanged)));

        public string DisplayHeaders
        {
            get { return (string)GetValue(DisplayHeadersProperty); }
            set { SetValue(DisplayHeadersProperty, value); }
        }

        public static readonly DependencyProperty DisplayColumnNamesProperty =
            DependencyProperty.Register("DisplayColumnNames", typeof(string), typeof(FrameListBox), new PropertyMetadata(string.Empty));

        public string DisplayColumnNames
        {
            get { return (string)GetValue(DisplayColumnNamesProperty); }
            set { SetValue(DisplayColumnNamesProperty, value); }
        }

        public static readonly DependencyProperty ColumnRowsProperty =
            DependencyProperty.Register("ColumnRows", typeof(int), typeof(FrameListBox), new PropertyMetadata(4));

        public int ColumnRows
        {
            get { return (int)GetValue(ColumnRowsProperty); }
            set { SetValue(ColumnRowsProperty, value); }
        }

        public static readonly DependencyProperty HeaderHeightProperty =
            DependencyProperty.Register("HeaderHeight", typeof(double), typeof(FrameListBox), new PropertyMetadata(30.0));

        public double HeaderHeight
        {
            get { return (double)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }

        //public static readonly DependencyProperty ColumnHeightProperty =
        //    DependencyProperty.Register("ColumnHeight", typeof(double), typeof(FrameListBox));

        //public double ColumnHeight
        //{
        //    get { return (double)GetValue(ColumnHeightProperty); }
        //    set { SetValue(ColumnHeightProperty, value); }
        //}

        public static readonly DependencyProperty ColumnMinHeightProperty =
            DependencyProperty.Register("ColumnMinHeight", typeof(double), typeof(FrameListBox), new PropertyMetadata(30.0));

        public double ColumnMinHeight
        {
            get { return (double)GetValue(ColumnMinHeightProperty); }
            set { SetValue(ColumnMinHeightProperty, value); }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FrameListBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var frameItem = new FrameListBoxItem();
            //BindingOperations.SetBinding(frameItem, ContentControl.HeightProperty, new Binding("ColumnHeight")
            //{
            //    Source = this,
            //});

            BindingOperations.SetBinding(frameItem, ContentControl.MinHeightProperty, new Binding("ColumnMinHeight")
            {
                Source = this,
            });

            frameItem.Loaded += (s, e) => BuildItemChildren(frameItem);
            return frameItem;
        }

        private static void OnDisplayHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameListBox frameList)
            {
                return;
            }

            if (frameList.IsLoaded)
            {
                BuildHeaderChildren(d as FrameListBox);
            }
            else
            {
                frameList.Loaded += (s, e)=> BuildHeaderChildren(s as FrameListBox);
            }
        }

        private static void BuildHeaderChildren(FrameListBox frameList)
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
                    FontSize = 15,
                    Foreground = Brushes.White,
                    Content = headerNames[i].Trim()
                };

                var border = new Border
                {
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    BorderBrush = Brushes.SkyBlue,
                    Opacity = 0.2
                };

                var grid = new Grid();
                grid.Children.Add(border);
                grid.Children.Add(contentControl);

                headerPanel.Children.Add(grid);
            }
        }

        private void BuildItemChildren(FrameListBoxItem frameListItem)
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
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 15,
                    Foreground = Brushes.SkyBlue,
                };

                BindingOperations.SetBinding(contentControl, ContentControl.ContentProperty, new Binding(columnNames[i].Trim())
                {
                    Source = frameListItem.DataContext,
                });

                var border = new Border
                {
                    BorderThickness = new Thickness(0,0,1,1),
                    BorderBrush = Brushes.SkyBlue,
                    Opacity = 0.2
                };

                var grid = new Grid() { Name = $"PART_Column_{columnNames[i].Trim()}"};
                grid.Children.Add(border);
                grid.Children.Add(contentControl);

                itemsPanel.Children.Add(grid);
            }
        }
    }
}