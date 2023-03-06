using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Demo.Colors
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IniWrapPanel();
        }

        void IniWrapPanel()
        {
            foreach (var pi in typeof(System.Windows.Media.Colors).GetProperties())
            {
                var color = (Color)ColorConverter.ConvertFromString(pi.Name);
                this.wrapPanel.Children.Add(new Label
                {
                    Height = 32,
                    Width = 140,
                    Background = new SolidColorBrush(color),
                    Content = pi.Name,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(1),
                });
            }
        }
    }
}
