using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Common.UI.WPF.PropertyGrid.Converters
{
    public class IsDefaultCategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string categoryName)
            {
                return (categoryName == CategoryAttribute.Default.Category);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
