using System.Windows;
using System.Windows.Controls;

namespace Common.UI.WPF.PropertyGrid.Editors
{
    public class TextBlockEditor : TypeEditor<TextBlock>
    {
        protected override TextBlock CreateEditor()
        {
            return new PropertyGridEditorTextBlock();
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = TextBlock.TextProperty;
        }
    }

    public class PropertyGridEditorTextBlock : TextBlock
    {
        static PropertyGridEditorTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTextBlock), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTextBlock)));
        }
    }
}
