using System.Globalization;
using System.Windows.Data;

namespace CookingBookWPF.Converters
{
    internal class LipsConverterX : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Random random = new();
            return 100 - random.NextDouble() * 200;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class LipsConverterY : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Random random = new();
            return random.NextDouble() * 20 + 240;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class LipsConverterAngle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Random random = new();
            return 5 - random.NextDouble() * 10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class LipsConverterImage : IValueConverter
    {
        private static readonly string[] _images = [
                                           "pack://application:,,,/Resources/lips.png",
                                           "pack://application:,,,/Resources/lips2.png",
                                           "pack://application:,,,/Resources/Blank.png",
                                           "pack://application:,,,/Resources/Blank.png",
                                           "pack://application:,,,/Resources/Blank.png",
                                           "pack://application:,,,/Resources/Blank.png",
                                          ];
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Random random = new();
            random.Shuffle(_images);
            return _images[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
