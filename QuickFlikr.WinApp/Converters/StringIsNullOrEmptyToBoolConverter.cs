using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QuickFlikr.WinApp.Converters
{
    public class StringIsNullOrEmptyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string input)
            {
                return string.IsNullOrEmpty(input) ? Visibility.Collapsed : Visibility.Visible;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}