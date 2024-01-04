using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using AllNotes.Models;
using AllNotes.Services;
using AllNotes.Views.NewNote;
//using AllNotes.Repositories;
using AllNotes.ViewModels;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using SQLite;
using AllNotes.Data;
using System.Threading.Tasks;
using AllNotes.Interfaces;
using AllNotes.Views;
using AllNotes.Database;
using AllNotes.Repositories;
using Xamarin.Essentials;

namespace AllNotes.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<object> _selectedNotes;
        public ObservableCollection<AppNote> Notes { get; set; }
       // private ObservableCollection<AppNote> _notes;
       // private AppFolder _selectedFolder;
        private INavigationService _navigationService;
        private int _folderId; // The ID of the currently selected folder
        private bool _showFab = true;
        private SelectionMode _selectionMode = SelectionMode.None;
        private AppDatabase _database;
        AppFolder selectedFolder;
        private bool _multiSelectEnabled = false;
        public ICommand OpenNewNoteScreenCommand { get; private set; }
       
        public ObservableCollection<object> SelectedNotes
        {
            get => _selectedNotes; set
            {
                _selectedNotes = value;
                OnPropertyChanged(nameof(SelectedNotes));
            }
        }
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public AppFolder SelectedFolder
        {
            get => selectedFolder;
            set
            {
                if (selectedFolder != value)
                {
                    selectedFolder = value;
                    OnPropertyChanged(nameof(SelectedFolder));
                    RefreshNotes(); // Refresh notes when selected folder changes

                    // Save the last used folder ID
                    if (selectedFolder != null)
                    {
                        SetLastUsedFolderId(selectedFolder.Id);
                    }
                }
            }
        }

        public bool ShowFab
        {
            get => _showFab;
            set
            {
                _showFab = value;
                OnPropertyChanged(nameof(ShowFab));
            }
        }
        public SelectionMode SelectionMode
        {
            get => _selectionMode;
            set
            {
                if (_selectionMode != value)
                {
                    _selectionMode = value;
                    OnPropertyChanged(nameof(SelectionMode));
                }
            }
        }
        public bool MultiSelectEnabled

        {
            get => _multiSelectEnabled;
            set
            {
                _multiSelectEnabled = value;
                OnPropertyChanged(nameof(MultiSelectEnabled));
            }
        }
       
        public MainPageViewModel(AppFolder selectedFolder)
        {
            selectedFolder = selectedFolder;
            _selectedNotes = new ObservableCollection<object>();
            Notes = new ObservableCollection<AppNote>();
           
            TapNoteCommand = new Command<AppNote>(TapNote);
            LongPressNoteCommand = new Command<AppNote>(LongPressNote);
            _folderId = selectedFolder.Id;
            _navigationService = DependencyService.Get<INavigationService>();
           // SetDefaultFolder();
          //  OpenNewNoteScreenCommand = new Command(OpenNewNoteScreen);
            MessagingCenter.Subscribe<NewNoteViewModel>(this, "RefreshNotes", (sender) => RefreshNotes());
           
            
            _navigationService = DependencyService.Get<INavigationService>();
           
           // RefreshNotes();
         //   InitializeWithDefaultFolder();
            LoadNotesForFolder(selectedFolder);
        }

        private async Task GetDefaultFolder()
        {
           await AppDatabase.Instance().GetFirstFolder();
        }
        private async void InitializeWithDefaultFolder()
        {
            var defaultFolder = await AppDatabase.Instance().GetFirstFolder();
            if (defaultFolder != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SelectedFolder = defaultFolder; // Set the default folder
                    LoadNotesForFolder(selectedFolder); // Refresh notes for the default folder
                });
            }
        }


       /* private async void InitializeWithDefaultFolder()
        {
            if (SelectedFolder == null)
            {
                var defaultFolder = await AppDatabase.Instance().GetFirstFolder();
                if (defaultFolder != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        SelectedFolder = defaultFolder; // Set only if no folder is currently selected
                    });
                }
                else
                {
                    // Handle the case if there is no default folder
                    // Create a default folder or choose an appropriate action
                }
            }
        }*/


        private int GetDefaultFolderId()
        {
            // Use a synchronous method to avoid Task-related issues
            var defaultFolder = AppDatabase.Instance().GetFirstFolder();
            return defaultFolder?.Id ?? 0; // Default to 0 if no folders exist
        }
        

        private int GetLastUsedFolderId()
        {
            // Retrieve the last used folder ID from preferences
            return Preferences.Get("LastUsedFolderId", 0); // Default to 0 if not set
        }

        private void SetLastUsedFolderId(int folderId)
        {
            // Save the last used folder ID to preferences
            Preferences.Set("LastUsedFolderId", folderId);
        }

        
       

        private int GetLastSelectedFolderId()
        {
            // Retrieve the last selected folder ID from local settings
            // Example: return Preferences.Get("LastFolderId", 0);
            // Implement this method based on how you store local settings
            return 0; // Default value if no folder ID is stored
        }
        
       
        public MainPageViewModel()
        {
            
            //  _selectedNotes = new ObservableCollection<AppNote>();

            // Subscribe to the FolderSelected message
            MessagingCenter.Subscribe<MenuPageViewModel, AppFolder>(this, "FolderSelected", (sender, folder) =>
            {
                SelectedFolder = folder; // Correctly assign the received folder to SelectedFolder
            });
            _navigationService = DependencyService.Get<INavigationService>();
        }


        public void LoadNotesForFolder(AppFolder folder)
        {
            selectedFolder = folder;
            Notes.Clear();
            if (selectedFolder != null)
            {
                var notesFromDb = AppDatabase.Instance().GetNoteList(selectedFolder.Id);
                foreach (var note in notesFromDb)
                {
                    Notes.Add(note);
                }

            }
            else
            {

                var notesFromDB = AppDatabase.Instance().GetNoteList(0);


            }
        }
       
        /*private void RefreshNotes()
        {
            if (SelectedFolder != null)
            {
                Notes.Clear();
                var notesFromDb = AppDatabase.Instance().GetNoteList(SelectedFolder.Id);
                foreach (var note in notesFromDb)
                {
                    Notes.Add(note);
                }
            }
            else
            {
                var SelectedFolder = AppDatabase.Instance().GetFirstFolder();
                if (SelectedFolder != null)
                {
                    Notes.Clear();
                    var notesFromDb = AppDatabase.Instance().GetNoteList(SelectedFolder.Id);
                    foreach (var note in notesFromDb)
                    {
                        Notes.Add(note);
                    }
                }
            }
        }*/
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  ABOVE METHOD WORKS MAY NEED TO BE UNCOMMENTED

        private void RefreshNotes()
        {
            if (Notes == null)
            {
                Notes = new ObservableCollection<AppNote>();
            }

            if (SelectedFolder != null)
            {
                Notes.Clear();
                var notesFromDb = AppDatabase.Instance().GetNoteList(SelectedFolder.Id);
                foreach (var note in notesFromDb)
                {
                    Notes.Add(note);
                }
            }
            else
            {
                // Handle the case where SelectedFolder is null
                // Perhaps load a default folder or show a message
            }
        }



        public Command<AppNote> TapNoteCommand { get; set; }
        private async void TapNote(AppNote selectedNote)
        {
            if (selectedNote != null)
            {
                if (_selectionMode == SelectionMode.None)
                {
                    var newNoteVM = new NewNoteViewModel(this, selectedNote);
                    var newNotePage = new NewNotePage(newNoteVM);
                    newNotePage.BindingContext = newNoteVM;
                    //   await Application.Current.MainPage.Navigation.PushAsync(newNotePage);
                    if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
                    {
                        var navigationPage = mainFlyoutPage.Detail as NavigationPage;
                        await navigationPage?.PushAsync(newNotePage);
                    }
                }
            }
        }
        public Command<AppNote> LongPressNoteCommand { get; set; }
        private void LongPressNote(AppNote selectedNote)
        {
            if (selectedNote != null)
            {
                if (_selectionMode == SelectionMode.None)
                {
                    ShowOrHideToolbar();
                    SelectionMode = SelectionMode.Multiple;
                    SelectedNotes.Add(selectedNote);
                }
            }
        }
        public void ShowOrHideToolbar()
        {
            MultiSelectEnabled = !MultiSelectEnabled;
            ShowFab = !ShowFab;
            if (MultiSelectEnabled)
                SelectionMode = SelectionMode.Multiple;
            else
                SelectionMode = SelectionMode.None;
        }


       

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}