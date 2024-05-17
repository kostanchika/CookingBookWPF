using System.Globalization;
using System.Windows.Data;

namespace CookingBookWPF.Converters
{
    internal class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int num)
            {
                if (num == 0)
                {
                    return "";
                }
                return num.ToString();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                try
                {
                    if (str[0] < '1' && str[0] > '9')
                    {
                        return 0;
                    }
                    return System.Convert.ToInt32(str);
                }
                catch
                {
                    return 0;
                }
            }
            return value;
        }
    }
}
