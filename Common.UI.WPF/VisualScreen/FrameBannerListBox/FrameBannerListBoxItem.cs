using System.Windows;
using System.Windows.Controls;

namespace Common.UI.WPF.VisualScreen
{
    internal class FrameBannerListBoxItem : ContentControl
    {
        static FrameBannerListBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameBannerListBoxItem), new FrameworkPropertyMetadata(typeof(FrameBannerListBoxItem)));
        }
    }
}