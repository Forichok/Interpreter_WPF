using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DevExpress.Mvvm;
using Interpreter_WPF_3.TreeView;

namespace Interpreter_WPF_3
{
    class MainViewModel : ViewModelBase
    {
        #region Properties
        private int OperationLenght { get; }
        private int OperandLenght { get; }
        private string FileExtension { get; }
        private Command CurrentCommand { get; set; }
        public FileSystemInfo CurrentFileInfo { get; set; }
        public ObservableCollection<FileSystemObjectInfo> CurrentDirectories { get; set; }        
        public bool CannotStartProject => !CanStartProject;
        public bool CanStartProject { get; set; }
        public ObservableCollection<Command> Commands { get; set; }
        private ObservableCollection<int> BreakPoints { get; set; }
        public string Message { private set; get; }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            Message = "Program has been started.";
            CanStartProject = true;
            FileExtension = ".wtf";
            OperandLenght = 9;
            OperationLenght = 5;
            using (File.Open(Directory.GetCurrentDirectory() + "\\Project.wtf", FileMode.OpenOrCreate))


                CurrentFileInfo = new FileInfo(Directory.GetCurrentDirectory() + "\\Project.wtf");
            Commands = new ObservableCollection<Command>(ReadFromBinaryFile(CurrentFileInfo.FullName));
            if (Commands.Count > 0)
                CurrentCommand = Commands.First();
            BreakPoints = new ObservableCollection<int>();

            var curDir = new FileSystemObjectInfo(new DirectoryInfo(Directory.GetCurrentDirectory()));
            CurrentDirectories = new ObservableCollection<FileSystemObjectInfo>() { curDir };

            RaisePropertyChanged("Commands");
        }
        #endregion

        #region Commands

        public ICommand NextStepCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Continue();
                });
            }
        }
        public ICommand StartCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Start();
                    RaisePropertyChanged("Commands");
                });
            }
        }
        public ICommand StopCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    CurrentCommand.IsCurrent = false;
                    CurrentCommand = Commands[0];
                    CanStartProject = true;
                    RaisePropertyChanged("Commands");
                });
            }
        }

        public ICommand BuildCommand
        {
            get
            {
                return new DelegateCommand<string>((str) =>
                {
                    Build(str);
                });
            }
        }

        public ICommand NewProjectCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var fbd = new OpenFileDialog
                    {
                        Title = @"Create New File",
                        Filter = @"wtf files (*.wtf)|*.wtf|All files (*.*)|*.*",
                        InitialDirectory = CurrentDirectories.First().FileSystemInfo.FullName,
                        CheckFileExists = false,
                    };

                    var result = fbd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        CurrentCommand = null;
                        Commands.Clear();
                        File.Create(fbd.FileName);
                        CurrentFileInfo = new FileInfo(fbd.FileName);
                        foreach (var dir in CurrentDirectories)
                        {
                            dir.RaisePropertiesChanged();
                        }
                        RaisePropertyChanged("Commands");
                    }
                });
            }
        }

        public ICommand<string> AddNewCommand
        {
            get
            {
                return new DelegateCommand<string>((str) =>
                {
                    var newCommand = new Command(str, Commands.Count + 1);
                    newCommand.PropertyChanged += Command_PropertyChanged;
                    Commands.Add(newCommand);
                    if (Commands.Count == 1)
                    {
                        CurrentCommand = Commands.First();
                    }
                    RaisePropertyChanged("Commands");
                });
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    System.Windows.Application.Current.Shutdown();
                });
            }
        }

        public ICommand SelectRootCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var fbd = new FolderBrowserDialog();
                    var result = fbd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        Commands.Clear();
                        CurrentFileInfo = null;
                        CurrentCommand = null;
                        CurrentDirectories.Clear();
                        CurrentDirectories.Add(new FileSystemObjectInfo(new DirectoryInfo(fbd.SelectedPath)));
                        RaisePropertyChanged("Commands");
                    }
                });
            }
        }

        public ICommand SaveProjectCommand
        {
            get
            {
                return new DelegateCommand<string>((str) =>
                {
                    var pathToSave = CurrentFileInfo.FullName;
                    if (str == "SaveAs")
                    {
                        var fbd = new SaveFileDialog
                        {
                            Title = @"Select File To Save",
                            Filter = @"wtf files (*.wtf)|*.wtf|All files (*.*)|*.*",
                            InitialDirectory = Directory.GetCurrentDirectory()
                        };
                        var result = fbd.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            pathToSave = fbd.FileName;
                        }
                        else
                        {
                            return;
                        }
                    }
                    using (var binaryWriter = new BinaryWriter(new FileStream(pathToSave, FileMode.OpenOrCreate)))
                    {
                        foreach (var command in Commands)
                        {
                            binaryWriter.Write(command.GetFullCommandInt());
                        }
                    }
                    foreach (var dir in CurrentDirectories)
                    {
                        dir.RaisePropertiesChanged();
                    }
                });
            }
        }
        public ICommand OpenProjectCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var fbd = new OpenFileDialog();

                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        CurrentFileInfo = new FileInfo(fbd.FileName);
                        Commands.Clear();
                        Commands = new ObservableCollection<Command>(ReadFromBinaryFile(CurrentFileInfo.FullName));
                    }
                });
            }
        }

        public ICommand TreeDoubleClickCommand
        {
            get
            {
                return new DelegateCommand<object>((obj) =>
                {
                    try
                    {
                        var fileSystemObjectInfo = obj as FileSystemObjectInfo;
                        if (fileSystemObjectInfo?.FileSystemInfo.Extension == ".wtf")
                        {
                            CurrentFileInfo = fileSystemObjectInfo.FileSystemInfo;
                            Commands.Clear();
                            Commands = new ObservableCollection<Command>(ReadFromBinaryFile(CurrentFileInfo.FullName));
                            CurrentCommand = Commands.First();
                        }
                    }
                    catch (Exception)
                    {

                    }
                });
            }
        }
        #endregion

        #region Command Helpers

        private async Task Build(string str)
        {
            str = str.Trim();
            var resultCode = CheckCode(str);

            if (resultCode != 0)
            {
                Message = $"line {resultCode}: Error! IncorrectCommand";
                return;
            }
            BreakPoints.Clear();
            foreach (var command in Commands)
            {
                if (command.IsBreakpointSet) BreakPoints.Add(command.CommandId);
            }
            Commands = FromsStringtoCommands(str);
            for (int i = 0; i < Commands.Count; i++)
            {
                if (BreakPoints.Contains(Commands[i].CommandId))
                {
                    Commands[i].IsBreakpointSet = true;
                }
            }
            CurrentCommand = Commands.First();

            SaveProjectCommand.Execute("Build");
            await Task.Factory.StartNew(() => SendMessage("Sucessfully Compiled!"));

            Start();
        }

        private int CheckCode(string str)
        {
            var parsedString = str.Split('\n');
            for (int i = 0; i < parsedString.Length; i++)
            {
                var command = parsedString[i];
                if (command.Trim() == "") continue;
                var len = command.Trim().Split(' ').Length;
                if (len != 4)
                    return i + 1;

                if (!command.Trim().All(k => Char.IsDigit(k) || Char.IsSeparator(k)))
                {
                    return i + 1;
                }
            }
            return 0;
        }

        private void Start()
        {
            if (Commands.Count == 0) return;
            CanStartProject = false;
            if (Commands.First().IsBreakpointSet)
            {
                Commands.First().IsCurrent = true;
                return;
            }
            Continue();
        }

        private async Task Continue()
        {
            var startCommandIndex = Commands.IndexOf(CurrentCommand);
            string result;

            RaisePropertyChanged("Commands");
            if (Commands.Count == 0)
                return;


            for (var i = startCommandIndex; i < Commands.Count; i++)
            {
                if (i + 1 >= Commands.Count) break;

                CurrentCommand.IsCurrent = false;

                Message = $"line {Commands.IndexOf(CurrentCommand) + 1}: {CurrentCommand} ";
                result = Commands[i].Execute();
                Message += GetCommandMessageResult(result);

                CurrentCommand = Commands[i + 1];
                CurrentCommand.IsCurrent = true;

                if (((i + 1 < this.Commands.Count)) && (Commands[i + 1].IsBreakpointSet))
                {
                    return;
                }
            }
            CurrentCommand.IsCurrent = false;

            Message = $"line {Commands.IndexOf(CurrentCommand) + 1}: {CurrentCommand} ";

            result = Commands.Last().Execute();

            Message += GetCommandMessageResult(result);

            Message += "\nProgramm has exited with code 0 (0x0).";
            await Task.Factory.StartNew(() => SendMessage("Ready To Start!"));

            CurrentCommand = Commands.First();
            CanStartProject = true;
        }
        #endregion

        #region Methods
        private ObservableCollection<Command> FromsStringtoCommands(string str)
        {
            var commands = new ObservableCollection<Command>();
            var parsedString = str.Split('\n');
            for (int i = 0; i < parsedString.Length; i++)
            {
                if ((parsedString[i].Split(' ').Length != 4) || (parsedString[i] == " ")) continue;

                var newCommand = new Command(parsedString[i], i + 1);
                newCommand.PropertyChanged += Command_PropertyChanged;
                commands.Add(newCommand);
            }
            return commands;
        }

        private async Task SendMessage(string message)
        {
            Thread.Sleep(1000);
            Message = message;
        }
        private string GetCommandMessageResult(string result)
        {
            var message = result == "gg" ? $" => {CurrentCommand}" : " => Error!";
            return message;
        }

        private IEnumerable<Command> ReadFromBinaryFile(string path)
        {
            var commandList = new List<Command>();

            try
            {
                using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    var id = 1;
                    while (reader.PeekChar() > -1)
                    {
                        var num = reader.ReadInt32();
                        var newBitArray = BitArrayExtension.GetBitArray(num, OperandLenght * 3 + OperationLenght);
                        var newCommand = new Command(newBitArray, id++);
                        newCommand.PropertyChanged += Command_PropertyChanged;
                        commandList.Add(newCommand);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return commandList;
        }

        private void Command_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("Commands");
        }
        #endregion
    }
}
