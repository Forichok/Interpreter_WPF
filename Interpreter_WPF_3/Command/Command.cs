using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;

namespace Interpreter_WPF_3
{
    class Command : ViewModelBase
    {
        private static int OperandLenght = 9;
        private static int OperationLenght = 5;

        #region Properties
        private readonly List<Func<BitArray>> Operations;
        public BitArray FirstOperand { get; set; }
        public BitArray SecondOperand { get; set; }
        public BitArray ThirdOperand { get; set; }
        public BitArray Operation { get; set; }
        public bool IsBreakpointSet { get; set; }
        public bool IsCurrent { get; set; }
        public int CommandId { get; set; }
        #endregion

        #region Constructors

        public Command(int id = 0)
        {
            ThirdOperand = BitArrayExtension.GetBitArray(0, OperandLenght);
            SecondOperand = BitArrayExtension.GetBitArray(0, OperandLenght);
            FirstOperand = BitArrayExtension.GetBitArray(0, OperandLenght);
            Operation = BitArrayExtension.GetBitArray(0, OperationLenght);
            CommandId = id;
            IsCurrent = false;
            IsBreakpointSet = false;
            Operations = InitializeOperations();
        }

        public Command(string command, int id = 0)
        {
            try
            {
                int[] splitedCommand = command.Split(' ').Select(i => Convert.ToInt32(i)).ToArray();
                if (splitedCommand.Length != 4) throw new CommandException(this, "Incorrect Command Input");
                ThirdOperand = BitArrayExtension.GetBitArray(splitedCommand[0], OperandLenght);
                SecondOperand = BitArrayExtension.GetBitArray(splitedCommand[1], OperandLenght);
                FirstOperand = BitArrayExtension.GetBitArray(splitedCommand[2], OperandLenght);
                Operation = BitArrayExtension.GetBitArray(splitedCommand[3], OperationLenght);
                CommandId = id;
                Operations = InitializeOperations();
            }
            catch (Exception)
            {
                new Command(id);
            }
        }
        public Command(BitArray command, int id = 0)
        {
            try
            {
                if (command.Count != 32) throw new CommandException(this, "Incorrect Command Input");
                var boolList = new List<bool>(command.Cast<bool>());
                ThirdOperand = new BitArray(boolList.GetRange(0, OperandLenght).ToArray());
                SecondOperand = new BitArray(boolList.GetRange(OperandLenght, OperandLenght).ToArray());
                FirstOperand = new BitArray(boolList.GetRange(OperandLenght * 2, OperandLenght).ToArray());
                Operation = new BitArray(boolList.GetRange(OperandLenght * 3, OperationLenght).ToArray());
                CommandId = id;
                Operations = InitializeOperations();
            }
            catch (Exception)
            {
                new Command(id);
            }
        }
        #endregion

        #region Methods
        private List<Func<BitArray>> InitializeOperations()
        {
            return new List<Func<BitArray>>() {
                GetOperandsList,
                NotFirstToThird,
                Or,
                And,
                Xor,
                Impication,
                CoImpication,
                Equivalence,
                Pierce,
                Scheffer,
                Addition,
                Subtraction,
                Multiplication,
                StrongDivision,
                Modulo,
                Swap,
                Insert,
                GetNumInSecOperandBase,
                ReadInBase,
                FindMaxDivider,
                ShiftL,
                ShiftR,
                CycleShiftL,
                CycleShiftR,
                MoveSecToFirst,
            };
        }        
        public string Execute()
        {
            var operationNum = Operation.ToInt();
            if (operationNum >= 0 && operationNum <= 24)
                Operations[operationNum].Invoke();
            RaisePropertyChanged();
            return "gg";
        }
        private string GetStringFromDialog()
        {
            var inputDialog = new InputDialog("Please, input your number:", "0");
            var result = string.Empty;

            if (inputDialog.ShowDialog() == true)
            {
                result = inputDialog.Answer;
            }
            return result;
        }
        private void ShowMessage(string message)
        {
            var messageBox = new MyMessageBox(message);
            var result = messageBox.ShowDialog();
        }
        #endregion

        #region Commands

        private BitArray GetOperandsList() //0 
        {
            var bs = FirstOperand.ToInt();
            if (bs > 36 || bs < 2)
            {
                ShowMessage("Incorrect Base");
                return null;
            }

            string result = ConvertToBase(ThirdOperand.ToInt(),bs) + "  " + ConvertToBase(SecondOperand.ToInt(),bs) + "  " +
                            ConvertToBase(FirstOperand.ToInt(),bs) + "  " + ConvertToBase(Operation.ToInt(),bs);
            ShowMessage(result);
            return null;
        }

        private BitArray NotFirstToThird() //1 
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand.Not();
            return ThirdOperand;
        }

        private BitArray Or() //2 
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand.Or(SecondOperand);
            return ThirdOperand;

        }

        private BitArray And() //3 
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand.And(SecondOperand);
            return ThirdOperand;
        }

        private BitArray Xor() //4 
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand.Xor(SecondOperand);
            return ThirdOperand;
        }

        private BitArray Impication() //5
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand.Not().Or(SecondOperand);
            return ThirdOperand;
        }

        private BitArray CoImpication() //6 
        {
            ThirdOperand = Impication().Not();
            return ThirdOperand;
        }

        private BitArray Equivalence() //7 
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand.And(SecondOperand);

            var tmpOperandA = new BitArray(FirstOperand);
            var tmpOperandB = new BitArray(SecondOperand);

            tmpOperandA.Not().And(tmpOperandB.Not());

            ThirdOperand.Or(tmpOperandA);
            return ThirdOperand;
        }

        private BitArray Pierce() //8 
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand.Or(SecondOperand);
            ThirdOperand.Not();
            return ThirdOperand;
        }

        private BitArray Scheffer() //9 
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand.And(SecondOperand);
            ThirdOperand.Not();
            return ThirdOperand;
        }

        private BitArray Addition() //10 
        {
            var result = FirstOperand.ToInt() + SecondOperand.ToInt();
            ThirdOperand = BitArrayExtension.GetBitArray(result, OperandLenght);
            return ThirdOperand;
        }

        private BitArray Subtraction() //11 
        {
            var result = FirstOperand.ToInt() - SecondOperand.ToInt();
            ThirdOperand = BitArrayExtension.GetBitArray(result, OperandLenght);
            return ThirdOperand;
        }

        private BitArray Multiplication() //12 
        {
            var result = FirstOperand.ToInt() * SecondOperand.ToInt();
            ThirdOperand = BitArrayExtension.GetBitArray(result, OperandLenght);
            return ThirdOperand;
        }

        private BitArray StrongDivision() //13 
        {
            var result = FirstOperand.ToInt() / SecondOperand.ToInt();
            ThirdOperand = BitArrayExtension.GetBitArray(result, OperandLenght);
            return ThirdOperand;
        }

        private BitArray Modulo() //14 
        {
            var result = FirstOperand.ToInt() % SecondOperand.ToInt();
            ThirdOperand = BitArrayExtension.GetBitArray(result, OperandLenght);
            return ThirdOperand;
        }

        private BitArray Swap() //15 
        {
            var tmpOperand = FirstOperand;
            FirstOperand = SecondOperand;
            SecondOperand = tmpOperand;
            return null;
        }

        private BitArray Insert() //16 
        {
            var position = SecondOperand.ToInt();

            var inRange = position < 9 && position >= 0;
            if (!inRange)
                position %= OperandLenght;

            FirstOperand[position] = ThirdOperand[position];

            return FirstOperand;
        }

        private BitArray GetNumInSecOperandBase() //17 
        {
            var num = FirstOperand.ToInt();
            var bs = SecondOperand.ToInt();

            if (bs > 36 || bs < 0)
            {
                ShowMessage("Incorrect Base");
                return BitArrayExtension.GetBitArray(0, 9);
            }

            ShowMessage(ConvertToBase(num,bs));
            return BitArrayExtension.GetBitArray(num,1);
        }

        private BitArray ReadInBase() //18
        {
            var bs = SecondOperand.ToInt();
            var operand = GetStringFromDialog();
            if (bs > 36 || bs < 0)
                return BitArrayExtension.GetBitArray(0, 9);
            try
            {
                operand = operand.ToUpper();

                int result = 0;
                for (int i = operand.Length - 1, mul = 0; i >= 0; i--, mul++)
                    result += (int)Math.Pow(bs, mul) * (Char.IsDigit(operand[i]) ?
                                  (int)Char.GetNumericValue(operand[i]) :
                                  (operand[i] - 55));
                FirstOperand = BitArrayExtension.GetBitArray(result, OperandLenght);
            }
            catch (Exception )
            {
                return BitArrayExtension.GetBitArray(0, 9);
            }
            return FirstOperand;
        }

        private BitArray FindMaxDivider() //19
        {
            var num = FirstOperand.ToInt();
            int divider = 0;
            for (int i = 0; i < 10; i++)
            {
                if (num % (int)Math.Pow(2, i) == 0)
                    divider = i;
            }
            ThirdOperand = BitArrayExtension.GetBitArray(divider, OperandLenght);
            return ThirdOperand;
        }

        private BitArray ShiftL() //20 
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand = ThirdOperand.ShiftL(SecondOperand.ToInt());
            return ThirdOperand;
        }

        private BitArray ShiftR() //21 
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand = ThirdOperand.ShiftR(SecondOperand.ToInt());
            return ThirdOperand;
        }

        private BitArray CycleShiftL() //22
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand = ThirdOperand.CycleShiftL(SecondOperand.ToInt());
            return ThirdOperand;
        }

        private BitArray CycleShiftR() //23 
        {
            ThirdOperand = new BitArray(FirstOperand);
            ThirdOperand = ThirdOperand.CycleShiftR(SecondOperand.ToInt());
            return ThirdOperand;
        }

        private BitArray MoveSecToFirst() //24
        {
            FirstOperand = SecondOperand;
            return FirstOperand;
        }

        #endregion

        #region Converters
        private string ConvertToBase(int num, int nbase)
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (nbase < 2 || nbase > chars.Length)
                return "";
            int r = 0;
            string newNumber = "";

            while (num >= nbase)
            {
                r = num % nbase;
                newNumber = chars[r] + newNumber;
                num = num / nbase;
            }

            newNumber = chars[num] + newNumber;

            return newNumber;
        }
        private string GetStringCommand()
        {
            string command = ThirdOperand.ToInt() + " " + SecondOperand.ToInt() + " " + FirstOperand.ToInt() + " " + Operation.ToInt();
            return command;
        }
        private string GetBinaryStringCommand()
        {
            string command = ThirdOperand.ToStr() + SecondOperand.ToStr() + FirstOperand.ToStr() + Operation.ToStr();
            return command;
        }
        public BitArray GetFullCommand()
        {
            var newBitArray = BitArrayExtension.GetBitArray(Convert.ToInt32(GetBinaryStringCommand(), 2), OperandLenght);
            return newBitArray;
        }
        public int GetFullCommandInt()
        {
            string command = GetBinaryStringCommand();
            var num = Convert.ToInt32(command, 2);
            return num;
        }
        public override string ToString()
        {
            return GetStringCommand();
        }
        #endregion
    }
}
