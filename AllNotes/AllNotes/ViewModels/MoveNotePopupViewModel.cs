using AllNotes.Database;
using AllNotes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Xamarin.Forms;

namespace AllNotes.ViewModels
{
    public class MoveNotePopupViewModel : INotifyPropertyChanged
    {
        /*private AppNote note;
        public AppNote Note;*/
      
        private ObservableCollection<AppFolder> _folderList = new ObservableCollection<AppFolder>();
        private ObservableCollection<AppNote> _notesToMove = new ObservableCollection<AppNote>();
        private MainPageViewModel _mainPageViewModel;
        private AppFolder _selectedFolder;

        public ObservableCollection<AppFolder> FolderList
        {
            get => _folderList;
            set
            {
                _folderList = value;
                OnPropertyChanged(nameof(FolderList));
            }
        }

        public ObservableCollection<AppNote> NotesToMove
        {
            get => _notesToMove;
            set
            {
                _notesToMove = value;
                OnPropertyChanged(nameof(NotesToMove));
            }
        }

        public AppFolder SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
                ShowConfirmationPrompt();
            }
        }

        public ICommand MoveNotesCommand { get; private set; }
        public ICommand TestCommand { get; private set; }

       /* public MoveNotePopupViewModel(MainPageViewModel mainPageViewModel)
        {
            _mainPageViewModel = mainPageViewModel;
            MoveNotesCommand = new Command(MoveNotesToFolder);
            TestCommand = new Command(TestMethod);

            LoadFolders();
        }*/
        public MoveNotePopupViewModel(List<AppNote> notesToMove)
        {
            NotesToMove = new ObservableCollection<AppNote>(notesToMove);
            // Further setup...
            NotesToMove = new ObservableCollection<AppNote>();
            //  NotesToMove = new ObservableCollection<AppNote>(notesToMove);
            //   FolderList = LoadFolders(); // Implement this method to load folders
            MoveNotesCommand = new Command(MoveNotesToFolder);
            TestCommand = new Command(TestMethod);
            
            LoadFolders();

        }


        public void SetNotesToMove(List<AppNote> notesToMove)
        {
            NotesToMove = new ObservableCollection<AppNote>(notesToMove);
        }

        public MoveNotePopupViewModel(Views.NewNote.Popups.MoveNotePopup moveNotePopup)
        {
        }

        public MoveNotePopupViewModel(MainPageViewModel mainPageViewModel)
        {
        }

        public MoveNotePopupViewModel()
        {
        }

        private void TestMethod()
        {
            if (NotesToMove != null && NotesToMove.Any())
            {
                foreach (var note in NotesToMove)
                {
                    Debug.WriteLine($"Note Title: {note.Title}, Note ID: {note.id}");
                }
            }
            else
            {
                Debug.WriteLine("No notes to move or NotesToMove is null.");
            }
        }

        public void LoadFolders()
        {
            var allFolders = AppDatabase.Instance().GetFolderList();
            FolderList.Clear();
            foreach (var folder in allFolders)
            {
                FolderList.Add(folder);
            }
        }
        private async void MoveNotesToFolder()
        {
            if (SelectedFolder != null)
            {
                if (NotesToMove != null)
                {
                    try
                    {
                        foreach (var note in NotesToMove)
                        {
                            // Assuming AppNote has a property named 'FolderID' or similar to represent its folder
                            note.folderID = SelectedFolder.Id;

                            // Assuming you have a method in your database service to update a note
                            await AppDatabase.Instance().UpdateNote(note);
                         //   MessagingCenter.Send(this, "NotesUpdated", note.folderID);
                        }

                        // Inform other parts of your app that notes have been moved
                        MessagingCenter.Send(this, "NotesMoved", SelectedFolder);
                        await Application.Current.MainPage.Navigation.PopModalAsync();
                        MessagingCenter.Send(this, "CloseModal");
                       
                        // MessagingCenter.Send(this, "CloseModal");
                        // Optionally, refresh the main page to reflect changes
                        // This depends on how your main page is set up to listen for updates
                    }
                    catch (Exception ex)
                    {
                        // Handle exception (e.g., log the error)
                        Debug.WriteLine($"Error updating notes: {ex.Message}");
                    }
                }
            }
        }

        /*private async void ShowConfirmationPrompt()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Move",
                $"Move selected notes to folder '{SelectedFolder.Name}'?",
                "Yes", "No");

            if (confirm)
            {
                MoveNotesToFolder();
            }
            else
            {
                SelectedFolder = null; // Reset the selection if the user cancels
            }
        }*/
        private async void ShowConfirmationPrompt()
        {
            if (SelectedFolder != null)
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Move",
                    $"Move selected notes to folder '{SelectedFolder.Name}'?",
                    "Yes", "No");

                if (confirm)
                {
                    MoveNotesToFolder();
                    MessagingCenter.Send(this, "CloseModal");
                }
                else
                {
                    SelectedFolder = null; // Reset the selection if the user cancels
                }
            }
            else
            {
                // Handle the case where SelectedFolder is null
                // This could involve displaying an error message or logging the issue
            }
        }

        // Other methods...

        /* private void ShowConfirmationPrompt()
         {
             // Implementation...
         }

         private void MoveNotesToFolder()
         {
             // Implementation...
         }*/


        // Implement INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}