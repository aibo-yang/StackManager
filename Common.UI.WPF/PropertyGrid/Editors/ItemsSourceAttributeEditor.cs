using System;
using Common.UI.WPF.PropertyGrid.Attributes;

namespace Common.UI.WPF.PropertyGrid.Editors
{
    public class ItemsSourceAttributeEditor : TypeEditor<System.Windows.Controls.ComboBox>
    {
        private readonly ItemsSourceAttribute attribute;

        public ItemsSourceAttributeEditor(ItemsSourceAttribute attribute)
        {
            this.attribute = attribute;
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = System.Windows.Controls.ComboBox.SelectedValueProperty;
        }

        protected override System.Windows.Controls.ComboBox CreateEditor()
        {
            return new PropertyGridEditorComboBox();
        }

        protected override void ResolveValueBinding(PropertyItem propertyItem)
        {
            SetItemsSource();
            base.ResolveValueBinding(propertyItem);
        }

        protected override void SetControlProperties(PropertyItem propertyItem)
        {
            Editor.DisplayMemberPath = "DisplayName";
            Editor.SelectedValuePath = "Value";
            if (propertyItem != null)
            {
                Editor.IsEnabled = !propertyItem.IsReadOnly;
            }
        }

        private void SetItemsSource()
        {
            Editor.ItemsSource = CreateItemsSource();
        }

        private System.Collections.IEnumerable CreateItemsSource()
        {
            var instance = Activator.CreateInstance(attribute.Type);
            return (instance as IItemsSource).GetValues();
        }
    }
}
