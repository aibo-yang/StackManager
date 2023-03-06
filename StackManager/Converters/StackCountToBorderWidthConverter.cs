using System;
using System.Globalization;
using System.Windows.Data;
using StackManager.UI;

namespace StackManager.Converters
{
    public class StackCountToBorderWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && double.TryParse(values[0].ToString(), out var percentage) && double.TryParse(values[1].ToString(), out var max))
            {
                return max * percentage;
            }
            return 0d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
