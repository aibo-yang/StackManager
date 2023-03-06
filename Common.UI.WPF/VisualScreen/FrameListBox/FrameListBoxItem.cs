using System.Windows;
using System.Windows.Controls;

namespace Common.UI.WPF.VisualScreen
{
    internal class FrameListBoxItem : ContentControl
    {
        static FrameListBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameListBoxItem), new FrameworkPropertyMetadata(typeof(FrameListBoxItem)));
        }
    }
}