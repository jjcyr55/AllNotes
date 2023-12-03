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
        internal int id;

        public event PropertyChangedEventHandler PropertyChanged;
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public string name { get; set; }
        public string iconPath { get; set; }
        public string noteCount { get; set; }
        public string Title { get; set; }
        public string IconSource { get; set; }
        public Type TargetPage { get; set; }
        public string Folder { get; set; }
        public string Name { get; set; }
        public string IconPath { get; set; }
        public string NoteCount { get; set; }

        public AppFolder()
        {
            // Set default values or leave properties as null/empty
        }

        // Constructor with required parameters
        public AppFolder(string folderName)
        {
            Name = folderName;
            // Set default values for other properties or leave them as null/empty
        }

        public AppFolder(string folderName, string path)
        {
            Name = folderName;
            IconPath = path;
            // Set default values for other properties or leave them as null/empty
        }

        public AppFolder(string folderName, string path, string count)
        {
            Name = folderName;
            IconPath = path;
            NoteCount = count;
        }
    }
}