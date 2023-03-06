using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Data;
using Common.UI.WPF.Primitives;

namespace Common.UI.WPF.PropertyGrid.Editors
{
    public class UpDownEditor<TEditor, TType> : TypeEditor<TEditor> where TEditor : UpDownBase<TType>, new()
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = UpDownBase<TType>.ValueProperty;
        }

        internal void SetMinMaxFromRangeAttribute(PropertyDescriptor propertyDescriptor, TypeConverter converter)
        {
            if (propertyDescriptor == null)
                return;

            var rangeAttribute = PropertyGridUtilities.GetAttribute<RangeAttribute>(propertyDescriptor);
            if (rangeAttribute != null)
            {
                Editor.Maximum = ((TType)converter.ConvertFrom(rangeAttribute.Maximum.ToString()));
                Editor.Minimum = ((TType)converter.ConvertFrom(rangeAttribute.Minimum.ToString()));
            }
        }
    }

    public class NumericUpDownEditor<TEditor, TType> : UpDownEditor<TEditor, TType> where TEditor : UpDownBase<TType>, new()
    {
        protected override void SetControlProperties(PropertyItem propertyItem)
        {
            base.SetControlProperties(propertyItem);

            var binding = new Binding("IsInvalid");
            binding.Source = this.Editor;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(propertyItem, PropertyItem.IsInvalidProperty, binding);
        }
    }

    public class IntegerUpDownEditor : NumericUpDownEditor<IntegerUpDown, int?>
    {
        protected override IntegerUpDown CreateEditor()
        {
            return new PropertyGridEditorIntegerUpDown();
        }

        protected override void SetControlProperties(PropertyItem propertyItem)
        {
            base.SetControlProperties(propertyItem);
            this.SetMinMaxFromRangeAttribute(propertyItem.PropertyDescriptor, TypeDescriptor.GetConverter(typeof(int)));
        }
    }

    public class PropertyGridEditorIntegerUpDown : IntegerUpDown
    {
        static PropertyGridEditorIntegerUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorIntegerUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorIntegerUpDown)));
        }
    }
}
