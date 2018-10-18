using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Interpreter_WPF_3
{
    class BreakpointConverter:IMultiValueConverter
    {
        

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isBreakPointSet = System.Convert.ToBoolean(values[0]);
            var isCurrent = System.Convert.ToBoolean(values[1]);
            if (isBreakPointSet)

               if (isCurrent)
                {
                    return new SolidColorBrush(Colors.DarkGoldenrod);
                }
            else
            return new SolidColorBrush(Colors.DarkRed);

            return new SolidColorBrush(Colors.LightSteelBlue);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
