using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Common.UI.WPF.VisualScreen.Converters
{
    public class GeometryToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return PathGeometry.CreateFromGeometry(value as Geometry);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
