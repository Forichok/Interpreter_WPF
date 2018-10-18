﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Interpreter_WPF_3
{

class ToolTipOperationConverter:IValueConverter
{
    private static readonly string[] Commands =
    {
        "0 – вывести состояние всех регистров в системе счисления, которая записана в 1 операнде",
        "1 – бинарное отрицание над содержимым 1 операнда, результат сохраняется в 3 операнд",
        "2 – дизъюнкция над 1 и 2 операндом, результат сохраняется в 3 операнд",
        "3 – конъюнкция над 1 и 2 операндом, результат сохраняется в 3 операнд",
        "4 – сложение по модулю 2 над 1 и 2 операндом, результат сохраняется в 3 операнд",
        "5 – импликация над 1 и 2 операндом, результат сохраняется в 3 операнд",
        "6 – коимпликация над 1 и 2 операндом, результат сохраняется в 3 операнд",
        "7 – эквиваленция над 1 и 2 операндом, результат сохраняется в 3 операнд",
        "8 – стрелка Пирса над 1 и 2 операндом, результат сохраняется в 3 операнд",
        "9 – штрих Шеффера над 1 и 2 операндом, результат сохраняется в 3 операнд",
        "10 – сложение 1 и 2 операнда, результат сохраняется в 3 операнд",
        "11 – вычитание из 1 операнда 2 операнда, результат сохраняется в 3 операнд",
        "12 – умножение 1 и 2 операнда, результат сохраняется в 3 операнд",
        "13 – целочисленное деление 1 операнда на 2 операнд, результат сохраняется в 3 операнд",
        "14 – остаток от деления 1 операнда на 2 операнд, результат сохраняется в 3 операнд",
        "15 – обмен содержимого 1 и 2 операндов(операция swap)",
        "16 – занести в 1 операнд в байт с номером, который находится во 2 операнде, байт, значение которое лежит на месте 3 операнда",
        "17 – вывести содержимое операнда 1 в системе счисления, которая записана месте для второго операнда",
        "18 – ввести в операнд 1 в системе счисления, которая записана месте для второго операнда значение с клавиатуры",
        "19 – найти максимальное значение 2𝑝, на которое делится 1 операнд, результат сохраняется в 3 операнд",
        "20 – сдвиг влево содержимого 1 операнда на количество бит, которое находится во 2-ом операнде, результат сохраняетсяв 3 операнд",
        "21 - сдвиг вправо содержимого 1 операнда на количество бит, которое находится во 2-ом операнде, результат сохраняется в 3 операнд",
        "22 – циклический сдвиг влево содержимого 1 операнда на количество бит, которое находится во 2-ом операнде, результат сохраняется в 3 операнд",
        "23 – циклический сдвиг вправо содержимого 1 операнда на количество бит, которое находится во 2-ом операнде, результат сохраняется в 3 операнд",
        "24 – занести в 1 операнд значение, которое стоит на месте 2 операнда"
    };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var a = value as BitArray;
                var num = a.ToInt();
                return Commands[num];
            }
            catch (Exception e)
            {
                return "Unknown Command :(";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
