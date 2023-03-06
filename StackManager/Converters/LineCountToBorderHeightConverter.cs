using System;
using System.Globalization;
using System.Windows.Data;

namespace StackManager.Converters
{
    public class LineCountToBorderHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && int.TryParse(values[0].ToString(), out var currentCount) && int.TryParse(values[1].ToString(), out var maxCount))
            {
                if (maxCount != 0)
                {
                    return currentCount / (double)maxCount * 320;
                }
            }
            return 0d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
