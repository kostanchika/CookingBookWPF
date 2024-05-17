using CookingBookWPF.Models;
using System.Globalization;
using System.Windows.Data;

namespace CookingBookWPF.Converters
{
    internal class ComboBoxToStringDifficultyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (str == string.Empty)
                {
                    return -1;
                }
                return Array.IndexOf(GlobalResources.Difficulties, value) - 1;
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int num)
            {
                return GlobalResources.Difficulties[num + 1];
            }
            return value;
        }
    }

    internal class ComboBoxToStringCuisineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (str == string.Empty)
                {
                    return -1;
                }
                return Array.IndexOf(GlobalResources.Cuisines, value) - 1;
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int num)
            {
                return GlobalResources.Cuisines[num + 1];
            }
            return value;
        }
    }

    internal class ComboBoxToStringCookingTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (str == string.Empty)
                {
                    return -1;
                }
                return Array.IndexOf(GlobalResources.CookingTimes, value) - 1;
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int num)
            {
                return GlobalResources.CookingTimes[num + 1];
            }
            return value;
        }
    }

    internal class ComboBoxToStringCategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (str == string.Empty)
                {
                    return -1;
                }
                return Array.IndexOf(GlobalResources.Categories, value) - 1;
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int num)
            {
                return GlobalResources.Categories[num + 1];
            }
            return value;
        }
    }
}
