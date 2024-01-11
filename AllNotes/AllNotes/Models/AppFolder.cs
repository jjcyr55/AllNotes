using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllNotes.Models
{
    public class AppFolder : INotifyPropertyChanged
    {
        //internal int id;

        public event PropertyChangedEventHandler PropertyChanged;
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }

        //public string Title { get; set; }
       // public string IconSource { get; set; }
        //[Ignore]
        //public Type TargetPage { get; set; }
        public string Name { get; set; } = "";

        public string IconPath { get; set; } = "";

        public string noteCount { get; set; } = "0";
       // public bool IsSecure { get; set; }
        public string EncryptedPassword { get; set; }
      //  public bool LockIconVisible => IsSecure;

        public AppFolder(string FolderName)
        {
            Name = FolderName;
        }
        public AppFolder(string FolderName, string Path)
        {
            Name = FolderName;
            IconPath = Path;
        }
        public AppFolder(string FolderName, string Path, string Count)
        {
            Name = FolderName;
            IconPath = Path;
            noteCount = Count;
        }
        public AppFolder()
        {
        }
        private bool _isSecure;

        public bool IsSecure
        {
            get => _isSecure;
            set
            {
                if (_isSecure != value)
                {
                    _isSecure = value;
                    OnPropertyChanged(nameof(IsSecure));
                    OnPropertyChanged(nameof(LockIconVisible));
                }
            }
        }

        public bool LockIconVisible => IsSecure;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}