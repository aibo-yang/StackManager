using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Common.UI.WPF.VisualScreen
{
    public class FrameHeader : ContentControl
    {
        static FrameHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameHeader), new FrameworkPropertyMetadata(typeof(FrameHeader)));
            BackgroundProperty.OverrideMetadata(typeof(FrameHeader), new FrameworkPropertyMetadata(Brushes.White));
        }

        #region Title
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(FrameHeader), new FrameworkPropertyMetadata(string.Empty));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleTextSpaceProperty =
            DependencyProperty.Register("TitleTextSpace", typeof(Thickness), typeof(FrameHeader), new FrameworkPropertyMetadata(new Thickness(2)));

        public Thickness TitleTextSpace
        {
            get { return (Thickness)GetValue(TitleTextSpaceProperty); }
            set { SetValue(TitleTextSpaceProperty, value); }
        }

        public static readonly DependencyProperty TitleFontSizeProperty =
            DependencyProperty.Register("TitleFontSize", typeof(double), typeof(FrameHeader), new FrameworkPropertyMetadata(20.0));

        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(FrameHeader), new FrameworkPropertyMetadata(Brushes.White));

        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }
        #endregion

        #region Content
        public static readonly DependencyProperty LeftContentProperty =
            DependencyProperty.Register("LeftContent", typeof(object), typeof(FrameHeader), new FrameworkPropertyMetadata(null));

        public object LeftContent
        {
            get { return (object)GetValue(LeftContentProperty); }
            set { SetValue(LeftContentProperty, value); }
        }

        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register("RightContent", typeof(object), typeof(FrameHeader), new FrameworkPropertyMetadata(Brushes.White));

        public object RightContent
        {
            get { return (object)GetValue(RightContentProperty); }
            set { SetValue(RightContentProperty, value); }
        }
        #endregion
    }
}