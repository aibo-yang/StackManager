using System.ComponentModel;
using System.Windows;

namespace Common.UI.WPF.PropertyGrid
{
    internal interface IPropertyContainer
    {
        ContainerHelper ContainerHelper { get; }

        Style PropertyContainerStyle { get; }

        EditorDefinitionCollection EditorDefinitions { get; }

        PropertyDefinitionCollection PropertyDefinitions { get; }

        bool IsCategorized { get; }

        bool IsSortedAlphabetically { get; }

        bool AutoGenerateProperties { get; }

        bool HideInheritedProperties { get; }

        FilterInfo FilterInfo { get; }

        bool? IsPropertyVisible(PropertyDescriptor pd);
    }
}
