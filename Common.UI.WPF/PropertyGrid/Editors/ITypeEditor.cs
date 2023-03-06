using System.Windows;

namespace Common.UI.WPF.PropertyGrid.Editors
{
    public interface ITypeEditor
    {
        FrameworkElement ResolveEditor(PropertyItem propertyItem);
    }
}
