using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BepcoinTicker
{
    class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((DateTime?) value)?.ToString("yyyy-MM-dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (DateTime?)value;
        }
    }

    class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Currency)value;
        }
    }
}
