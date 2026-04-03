using System.Windows.Data;
using System.Windows.Media;

namespace ApiTestFramework.UI.Converters;

public class StatusCodeToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is int statusCode)
        {
            return statusCode switch
            {
                >= 200 and < 300 => new SolidColorBrush(Color.FromRgb(73, 204, 144)),
                >= 300 and < 400 => new SolidColorBrush(Color.FromRgb(252, 161, 48)),
                >= 400 and < 500 => new SolidColorBrush(Color.FromRgb(249, 62, 62)),
                >= 500 => new SolidColorBrush(Color.FromRgb(249, 62, 62)),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
