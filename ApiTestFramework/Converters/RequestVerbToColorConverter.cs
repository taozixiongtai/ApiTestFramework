using ApiTestFramework.Infrastructure.Enum;
using System.Windows.Data;
using System.Windows.Media;

namespace ApiTestFramework.Converters;

public class RequestVerbToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is RequestVerbEnum verb)
        {
            return verb switch
            {
                RequestVerbEnum.Get => new SolidColorBrush(Color.FromRgb(97, 175, 239)),
                RequestVerbEnum.Post => new SolidColorBrush(Color.FromRgb(73, 204, 144)),
                RequestVerbEnum.Put => new SolidColorBrush(Color.FromRgb(252, 161, 48)),
                RequestVerbEnum.Delete => new SolidColorBrush(Color.FromRgb(249, 62, 62)),
                RequestVerbEnum.Patch => new SolidColorBrush(Color.FromRgb(80, 139, 172)),
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
