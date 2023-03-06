using System.Windows;
using System.Windows.Controls;

namespace Common.UI.WPF.PropertyGrid.Editors
{
    public class CheckBoxEditor : TypeEditor<CheckBox>
    {
        protected override CheckBox CreateEditor()
        {
            return new PropertyGridEditorCheckBox();
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = CheckBox.IsCheckedProperty;
        }
    }

    public class PropertyGridEditorCheckBox : CheckBox
    {
        static PropertyGridEditorCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorCheckBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorCheckBox)));
        }
    }
}
