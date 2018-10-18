using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_WPF_3
{
    class CommandException : Exception
    {
        public CommandException(Command command, string message) : base(message) { }

        public CommandException(Command command)
        {

        }

    }
}
