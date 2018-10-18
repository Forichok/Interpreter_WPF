using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;

namespace Interpreter_WPF_3.TreeView
{    public class FileSystemObjectInfo : ViewModelBase
    {
        #region Constructors
        public FileSystemObjectInfo(FileSystemInfo info)
        {
            if (this is DummyFileSystemObjectInfo) return;
            Children = new ObservableCollection<FileSystemObjectInfo>();
            FileSystemInfo = info;
            if (info is DirectoryInfo)
            {
                ImageSource = FolderManager.GetImageSource(info.FullName, ShellManager.ItemState.Close);
                AddDummy();
            }
            else if (info is FileInfo)
            {
                ImageSource = FileManager.GetImageSource(info.FullName);
            }
            PropertyChanged += FileSystemObjectInfo_PropertyChanged;
        }

        public FileSystemObjectInfo(DriveInfo drive)
            : this(drive.RootDirectory)
        {
            Drive = drive;
        }
        #endregion

        #region Properties

        public ObservableCollection<FileSystemObjectInfo> Children { get; set; }

        public ImageSource ImageSource { get; set; }


        public bool IsExpanded { get; set; }

        public FileSystemInfo FileSystemInfo { get; set; }

        private DriveInfo Drive { get; set;}

        #endregion

        #region Methods

        public void RaisePropertiesChanged()
        {
            UpdateTree(this);
        }

        private void UpdateTree(FileSystemObjectInfo fileSystemObjectInfo)
        {
            fileSystemObjectInfo.ExploreFiles();
            fileSystemObjectInfo.ExploreDirectories();
            if(fileSystemObjectInfo.Children!=null)
            foreach (var child in fileSystemObjectInfo.Children)
            {
                child.ExploreDirectories();
                child.ExploreFiles();
                UpdateTree(child);
            }
        }

        private void AddDummy()
        {
            Children.Add(new DummyFileSystemObjectInfo());
        }

        private bool HasDummy()
        {
            return !ReferenceEquals(GetDummy(), null);
        }

        private DummyFileSystemObjectInfo GetDummy()
        {
            var list = Children.OfType<DummyFileSystemObjectInfo>().ToList();
            if (list.Count > 0) return list.First();
            return null;
        }

        private void RemoveDummy()
        {
            Children.Remove(GetDummy());
        }

        private void ExploreDirectories()
        {
            if (!ReferenceEquals(Drive, null))
            {
                if (!Drive.IsReady) return;
            }
            try
            {
                if (FileSystemInfo is DirectoryInfo)
                {
                    var directories = ((DirectoryInfo)FileSystemInfo).GetDirectories();
                    foreach (var directory in directories.OrderBy(d => d.Name))
                    {
                        if (!Equals((directory.Attributes & FileAttributes.System), FileAttributes.System) &&
                            !Equals((directory.Attributes & FileAttributes.Hidden), FileAttributes.Hidden))
                        {
                            var newDirectory = new FileSystemObjectInfo(directory);
                            var directoriesInfo = new List<string>();
                            foreach (var dir in Children)
                            {
                                directoriesInfo.Add(dir.FileSystemInfo.FullName); 
                            }
                            if(!directoriesInfo.Contains(newDirectory.FileSystemInfo.FullName))
                                Children.Add(newDirectory);
                        }
                    }
                }
            }
            catch
            {
                /*throw;*/
            }
        }

        private void ExploreFiles()
        {
            if (!ReferenceEquals(Drive, null))
            {
                if (!Drive.IsReady) return;
            }
            try
            {
                if (FileSystemInfo is DirectoryInfo)
                {
                    var files = ((DirectoryInfo)FileSystemInfo).GetFiles();
                    foreach (var file in files.OrderBy(d => d.Name))
                    {
                        if (!Equals((file.Attributes & FileAttributes.System), FileAttributes.System) &&
                            !Equals((file.Attributes & FileAttributes.Hidden), FileAttributes.Hidden))
                        {
                            if (file.Extension == ".wtf")
                            {
                                var newFile = new FileSystemObjectInfo(file);
                                var FilesInfo = new List<string>();
                                foreach (var dir in Children)
                                {
                                    FilesInfo.Add(dir.FileSystemInfo.FullName);
                                }
                                if (!FilesInfo.Contains(newFile.FileSystemInfo.FullName))
                                    Children.Add(newFile);
                            }
                        }
                    }
                }
            }
            catch
            {
                /*throw;*/
            }
        }

        #endregion

        void FileSystemObjectInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FileSystemInfo is DirectoryInfo)
            {
                if (string.Equals(e.PropertyName, "IsExpanded", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (IsExpanded)
                    {
                        ImageSource = FolderManager.GetImageSource(FileSystemInfo.FullName, ShellManager.ItemState.Open);
                        if (HasDummy())
                        {
                            RemoveDummy();
                            ExploreDirectories();
                            ExploreFiles();
                        }
                    }
                    else
                    {
                        ImageSource = FolderManager.GetImageSource(FileSystemInfo.FullName, ShellManager.ItemState.Close);
                    }
                }
            }
        }

        private class DummyFileSystemObjectInfo : FileSystemObjectInfo
        {
            public DummyFileSystemObjectInfo()
                : base(new DirectoryInfo("DummyFileSystemObjectInfo"))
            {
            }
        }
    }
}
