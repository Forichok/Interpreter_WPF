using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Interpreter_WPF_3
{
    class BitArrToStrConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bitArr = value as BitArray;
            return bitArr.ToInt();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bitArr = value as String;
            int len = System.Convert.ToInt32(parameter);

            try
            {
                return BitArrayExtension.GetBitArray(System.Convert.ToInt32(bitArr), len);
            }
            catch (Exception e)
            {
                return new BitArray(0);

            }
        }
    }
}
