using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Shecan.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isReverse = false;

            if (parameter != null)
                isReverse = System.Convert.ToBoolean(parameter);

            var status = System.Convert.ToBoolean(value);

            if (isReverse)
                return status ? Visibility.Visible : Visibility.Collapsed;

            return !status ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}