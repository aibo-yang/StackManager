using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Common.UI.WPF.PropertyGrid.Converters
{
    public class SelectedObjectConverter : IValueConverter
    {
        private const string ValidParameterMessage = @"parameter must be one of the following strings: 'Type', 'TypeName', 'SelectedObjectName'";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            if (parameter is not string)
            {
                throw new ArgumentException(SelectedObjectConverter.ValidParameterMessage);
            }

            if (this.CompareParam(parameter, "Type"))
            {
                return this.ConvertToType(value, culture);
            }
            else if (this.CompareParam(parameter, "TypeName"))
            {
                return this.ConvertToTypeName(value, culture);
            }
            else if (this.CompareParam(parameter, "SelectedObjectName"))
            {
                return this.ConvertToSelectedObjectName(value, culture);
            }
            else
            {
                throw new ArgumentException(SelectedObjectConverter.ValidParameterMessage);
            }
        }

        private bool CompareParam(object parameter, string parameterValue)
        {
            return string.Compare((string)parameter, parameterValue, true) == 0;
        }

        private object ConvertToType(object value, CultureInfo culture)
        {
            return value?.GetType();
        }

        private object ConvertToTypeName(object value, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            Type newType = value.GetType();
            
            if (newType.GetInterface("ICustomTypeProvider", true) != null)
            {
                var methodInfo = newType.GetMethod("GetCustomType");
                newType = methodInfo.Invoke(value, null) as Type;
            }

            DisplayNameAttribute displayNameAttribute = newType.GetCustomAttributes(false).OfType<DisplayNameAttribute>().FirstOrDefault();

            return (displayNameAttribute == null) ? newType.Name : displayNameAttribute.DisplayName;
        }

        private object ConvertToSelectedObjectName(object value, CultureInfo culture)
        {
            if (value == null)
            { 
                return String.Empty;
            }

            Type newType = value.GetType();
            PropertyInfo[] properties = newType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "Name")
                {
                    return property.GetValue(value, null);
                }
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
