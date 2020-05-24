using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Launcher.View.Converter {
    public class TimeSpanToTimeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            TimeSpan time = (TimeSpan)value;
            return $"{time.Days}d {time.Hours}h {time.Minutes}m";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }
    }
}
