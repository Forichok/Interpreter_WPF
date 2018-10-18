using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_WPF_3
{
    internal static class BitArrayExtension
    {
        #region CommandHelpers
        public static BitArray CycleShiftR(this BitArray operand, int count)
        {
            var source = operand.Cast<bool>().ToArray();
            for (int i = 0; i < count; i++)
            {
                var temp = source[source.Length - 1];
                Array.Copy(source, 0, source, 1, source.Length - 1);
                source[0] = temp;
            }
            operand = new BitArray(source);
            return operand;
        }
        public static BitArray CycleShiftL(this BitArray operand, int count)
        {
            var source = operand.Cast<bool>().Reverse().ToArray();
            for (int i = 0; i < count; i++)
            {
                var temp = source[source.Length - 1];
                Array.Copy(source, 0, source, 1, source.Length - 1);
                source[0] = temp;
            }
            source = source.Reverse().ToArray();
            operand = new BitArray(source);
            return operand;
        }
        public static BitArray ShiftR(this BitArray operand, int number)
        {
            var tmpNum = operand.ToInt() >> number;
            operand = GetBitArray(tmpNum, 9);
            return operand;
        }
        public static BitArray ShiftL(this BitArray operand, int number)
        {
            var tmpNum = operand.ToInt() >> number;
            operand = GetBitArray(tmpNum, 9);
            return operand;
        }
        #endregion

        #region Converters               
        public static BitArray GetBitArray(int number, int lenght)
        {
            var bitArray = new BitArray(new[] { number });
            bitArray.Length = lenght;
            bitArray = new BitArray(bitArray.ToBoolArray().Reverse().ToArray());
            return bitArray;
        }
        public static bool[] ToBoolArray(this BitArray bitArray)
        {
            var boolArray = bitArray.Cast<bool>().ToArray();
            return boolArray;
        }
        public static int ToInt(this BitArray operand)
        {
            var binaryOperand = string.Join("", operand.ToBoolArray().Select(Convert.ToInt32));
            binaryOperand = binaryOperand.Length > 9 ? binaryOperand : binaryOperand.PadLeft(9, '0');
            var result = Convert.ToInt32(binaryOperand, 2);
            return result;
        }
        public static string ToStr(this BitArray bitArray)
        {
            var result = string.Join("", bitArray.Cast<bool>().Select(Convert.ToInt32));
            return result;
        }
        #endregion
    }
}
