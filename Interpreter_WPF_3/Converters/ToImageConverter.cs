using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Interpreter_WPF_3
{
    class ToImageConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = System.Convert.ToString(parameter);
            
                if (System.Convert.ToBoolean(value))
                {
                    
                    Uri uri = new Uri($"pack://application:,,,/Images/{str}Enabled.png");
                    BitmapImage source = new BitmapImage(uri);
                    return source;
                }
                else
                {
                    Uri uri = new Uri($"pack://application:,,,/Images/{str}Disabled.png");
                    BitmapImage source = new BitmapImage(uri);
                    return source;
                }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
