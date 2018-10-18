using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Interpreter_WPF_3
{
    class StringToRichTextBoxConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var commandsList=new List<Command>();
            foreach (var val in (ObservableCollection<Command>) value)
            {
                commandsList.Add(val);
            }
            var result = string.Join("\n", commandsList);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var commands = FromsStringtoCommands(System.Convert.ToString(value));
            return commands;
        }

        private ObservableCollection<Command> FromsStringtoCommands(string text)
        {
            var commands = new ObservableCollection<Command>();
            var parsedString = text.Split('\n');
            for (int i = 0; i < parsedString.Length; i++)
            {
                commands.Add(new Command(parsedString[i], i + 1));
            }
            return commands;
        }
    }
}
