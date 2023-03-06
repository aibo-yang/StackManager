using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;

namespace Common.UI.WPF.PropertyGrid.Editors
{
    public class TextBoxEditor : TypeEditor<WatermarkTextBox>
    {
        protected override WatermarkTextBox CreateEditor()
        {
            return new PropertyGridEditorTextBox();
        }

        protected override void SetControlProperties(PropertyItem propertyItem)
        {
            var displayAttribute = PropertyGridUtilities.GetAttribute<DisplayAttribute>(propertyItem.PropertyDescriptor);
            if (displayAttribute != null)
            {
                this.Editor.Watermark = displayAttribute.GetPrompt();
            }
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = TextBox.TextProperty;
        }
    }

    public class PropertyGridEditorTextBox : WatermarkTextBox
    {
        static PropertyGridEditorTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTextBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTextBox)));
        }
    }
}
