using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Common.UI.WPF.VisualScreen
{
    public class FrameTabItem : TabItem
    {
        static FrameTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameTabItem), new FrameworkPropertyMetadata(typeof(FrameTabItem)));
            ForegroundProperty.OverrideMetadata(typeof(FrameTabItem), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White)) { Inherits = false });
            FontSizeProperty.OverrideMetadata(typeof(FrameTabItem), new FrameworkPropertyMetadata(16.0));
            FontWeightProperty.OverrideMetadata(typeof(FrameTabItem), new FrameworkPropertyMetadata(FontWeights.Bold));
        }
    }
}