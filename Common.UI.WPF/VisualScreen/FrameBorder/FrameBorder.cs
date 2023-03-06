using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Common.UI.WPF.VisualScreen
{
    public class FrameBorder : ContentControl
    {
        static FrameBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameBorder), new FrameworkPropertyMetadata(typeof(FrameBorder)));
            BackgroundProperty.OverrideMetadata(typeof(FrameBorder), new FrameworkPropertyMetadata(Brushes.White));
        }

        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register("Title", typeof(string), typeof(FrameBorder), new FrameworkPropertyMetadata(string.Empty));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleVisibilityProperty =
            DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(FrameBorder), new FrameworkPropertyMetadata(Visibility.Visible));

        public Visibility TitleVisibility
        {
            get { return (Visibility)GetValue(TitleVisibilityProperty); }
            set { SetValue(TitleVisibilityProperty, value); }
        }
    }
}