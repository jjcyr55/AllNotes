using AllNotes.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AllNotes.ViewModels
{

    // NewFolderViewModel.cs
    public class NewFolderViewModel// : INotifyPropertyChanged
    {
        /*   public event PropertyChangedEventHandler PropertyChanged;

           private NewFolderModel _newFolder;
           public NewFolderModel NewFolder
           {
               get { return _newFolder; }
               set
               {
                   _newFolder = value;
                   OnPropertyChanged();
               }
           }

           public ICommand SaveCommand { get; private set; }
           public ICommand CancelCommand { get; private set; }

           public NewFolderViewModel()
           {
               NewFolder = new NewFolderModel();

               SaveCommand = new Command(ExecuteSaveCommand);
               CancelCommand = new Command(ExecuteCancelCommand);
           }

           private void ExecuteSaveCommand()
           {
               // Implement logic to save the new folder
               // You can use your SQLite database here
               // For example: AppDatabase.Instance().InsertFolder(new AppFolder(NewFolder.FolderName, ...));

               // Close the popup or navigate back
           }

           private void ExecuteCancelCommand()
           {
               // Close the popup or navigate back
           }

           protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
           {
               PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
           }
       }*/
    }
}