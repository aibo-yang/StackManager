using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Common.UI.WPF.VisualScreen
{
    public class FrameTabControl : TabControl
    {
        static FrameTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameTabControl), new FrameworkPropertyMetadata(typeof(FrameTabControl)));
            BackgroundProperty.OverrideMetadata(typeof(FrameTabControl), new FrameworkPropertyMetadata(Brushes.White));
        }
    }
}