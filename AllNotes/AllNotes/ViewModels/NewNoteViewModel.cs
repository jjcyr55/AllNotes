using System;
using System.Windows.Input;
using Xamarin.Forms;
//using AllNotes.Services;
using AllNotes.Enum;
using AllNotes.Models;
using System.Collections.ObjectModel;
using Xamarin.CommunityToolkit.Extensions;
using AllNotes.Views.NewNote.Popups;
using System.ComponentModel;
using AllNotes.Services;
using AllNotes.Views.NewNote;
using AllNotes.Views;
using AllNotes.Interfaces;
using System.Collections.Generic;

namespace AllNotes.ViewModels
{
    public class NewNoteViewModel : INotifyPropertyChanged
    {
        public ICommand SaveNoteCommand => new Command(SaveNote);
        
        public string NewNoteTitle { get; set; }
        public string NewNoteText { get; set; }
        public string NewNoteDate { get; set; }
      

        private NoteRepository _noteRepository;
        private MainPageViewModel _mainPageViewModel;
        private Note _note;

       
        private Note selectedNote;
       // int selectedFolderId = 0;
        private int _selectedFolder;

        private IFolderRepository _folderRepository;
        private ObservableCollection<AppFolder> _folderList;
        
       // private AppFolder _selectedFolder;
        public List<AppFolder> folderList { get; set; }

       

        private int _textAlignment = 0;
        private int _selectedFolderId;




        // private int folderID;



        public NewNoteViewModel(MainPageViewModel mainPageViewModel, Note note, int selectedFolder) //int folderId)
        {
           
            _selectedFolder = selectedFolder;
            NewNoteTitle = "";
            NewNoteDate = DateTime.Now.ToString();
            _noteRepository = new NoteRepository();
            _mainPageViewModel = mainPageViewModel;
            _note = note;
           

            if (_note != null)
            {
                NewNoteTitle = note.Title;
                NewNoteText = note.Text;
                NewNoteDate = note.Date;
               
            }
            else
            {
                // Creating a new note
                NewNoteTitle = "";
                NewNoteDate = DateTime.Now.ToString();
                // Default values for other properties as needed
            }
        }



        


        private async void SaveNote()
        {
            if (!string.IsNullOrWhiteSpace(NewNoteText))
            {
                // Set to default folder ID if no folder is selected
                if (_selectedFolder == null)
                {
                    _selectedFolder = _selectedFolder = 0; // Replace with your default folder ID
                }

                var newNote = new Note
                {
                 
                    Title = NewNoteTitle,
                    Text = NewNoteText,
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                   
                };

                if (_note == null)
                {
                    await _noteRepository.CreateNote(NewNoteTitle, NewNoteText, NewNoteDate, _selectedFolder);
                }
                else
                {
                    _note.FolderId = _selectedFolder;
                    await _noteRepository.EditNote(_note.Id, NewNoteTitle, NewNoteText, NewNoteDate, _selectedFolder);
                }

                // Refresh the notes list in MainPageViewModel
                await _mainPageViewModel.Reset(new AppFolder { Id = _selectedFolderId });


                // Navigate back
                if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
                {
                    var navigationPage = mainFlyoutPage.Detail as NavigationPage;
                    await navigationPage?.PopAsync();
                }
            }
        }



        // Other methods and properties...
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    


   
        public NewNoteViewModel(Note note, int folderId)
        {

            _selectedFolder = folderId;
            selectedNote = note;
            _selectedFolder = note.folderId;

        }
    }
}