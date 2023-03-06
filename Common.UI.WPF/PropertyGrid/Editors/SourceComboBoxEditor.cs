using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Common.UI.WPF.PropertyGrid.Converters;

namespace Common.UI.WPF.PropertyGrid.Editors
{
    public class SourceComboBoxEditor : ComboBoxEditor
    {
        internal static string ComboBoxNullValue = "Null";
        private readonly ICollection collection;
        private readonly TypeConverter typeConverter;

        public SourceComboBoxEditor(ICollection collection, TypeConverter typeConverter)
        {
            // Add a "Null" input value in the ComboBox when using a NullableConverter.
            this.collection = (typeConverter is NullableConverter)
                          ? collection.Cast<object>().Select(x => x ?? SourceComboBoxEditor.ComboBoxNullValue).ToArray()
                          : collection;

            this.typeConverter = typeConverter;
        }

        protected override IEnumerable CreateItemsSource(PropertyItem propertyItem)
        {
            return collection;
        }

        protected override IValueConverter CreateValueConverter()
        {
            if (typeConverter != null)
            {
                //When using a stringConverter, we need to convert the value
                if (typeConverter is StringConverter || typeConverter is EnumTypeConverter)
                {
                    return new SourceComboBoxEditorStringConverter(typeConverter);
                }

                //When using a NullableConverter, we need to convert the null value
                if (typeConverter is NullableConverter)
                {
                    return new SourceComboBoxEditorNullableConverter();
                }
            }
            return null;
        }
    }

    internal class SourceComboBoxEditorStringConverter : IValueConverter
    {
        private TypeConverter typeConverter;

        internal SourceComboBoxEditorStringConverter(TypeConverter typeConverter)
        {
            this.typeConverter = typeConverter;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (typeConverter != null)
            {
                if (typeConverter.CanConvertTo(typeof(string)))
                {
                    return typeConverter.ConvertTo(value, typeof(string));
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (typeConverter != null)
            {
                if (typeConverter.CanConvertFrom(value.GetType()))
                {
                    return typeConverter.ConvertFrom(value);
                }
            }
            return value;
        }
    }

    internal class SourceComboBoxEditorNullableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? SourceComboBoxEditor.ComboBoxNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(SourceComboBoxEditor.ComboBoxNullValue) ? null : value;
        }
    }
}
