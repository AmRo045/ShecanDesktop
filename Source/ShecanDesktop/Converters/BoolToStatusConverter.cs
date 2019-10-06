using System;
using System.Globalization;
using System.Windows.Data;

namespace ShecanDesktop.Converters
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = System.Convert.ToBoolean(value);
            return !status ? "OFF" : "ON";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}