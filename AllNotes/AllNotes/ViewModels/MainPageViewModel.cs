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
using System.Text.RegularExpressions;

namespace AllNotes.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<object> _selectedNotes;
        public ObservableCollection<AppNote> Notes { get; set; }

        private INavigationService _navigationService;
        private int _folderId;
        private bool _showFab = true;
        private SelectionMode _selectionMode = SelectionMode.None;
        private AppDatabase _database;
        AppFolder selectedFolder;
        private bool _multiSelectEnabled = false;
        public ICommand OpenNewNoteScreenCommand { get; private set; }

        private bool isFirstNoteAfterRestart = true;

        //!!!!!!!!!!!!!!!!!!!!!!
        public ObservableCollection<object> SelectedNotes
        {
            get => _selectedNotes; set
            {
                _selectedNotes = value;
                OnPropertyChanged(nameof(SelectedNotes));
            }
        }

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
        private bool _isInMultiSelectMode;

        public bool IsInMultiSelectMode
        {
            get => _isInMultiSelectMode;
            set
            {
                if (_isInMultiSelectMode != value)
                {
                    _isInMultiSelectMode = value;
                    OnPropertyChanged(nameof(IsInMultiSelectMode));
                }
            }
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                PerformSearch(); // Call the search method when the query changes
            }
        }
        /*public void PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                // If the search query is empty, show all notes
                RefreshNotes();
            }
            else
            {
                // Filter the notes based on the query
                var filteredNotes = AppDatabase.Instance().GetNoteList(selectedFolder.Id)
    .Where(note => note.Title.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   note.Text.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
    .ToList();

                // Update the Notes collection with the search results
                Notes.Clear();
                foreach (var note in filteredNotes)
                {
                    Notes.Add(note);
                }
            }
        }*/


        public void PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                RefreshNotes();
            }
            else
            {
                // Ensure selectedFolder is not null
                if (selectedFolder == null)
                {
                    return;
                }

                var noteList = AppDatabase.Instance().GetNoteList(selectedFolder.Id);
                if (noteList == null)
                {
                    return;
                }

                // Filter the notes based on the query
                var filteredNotes = noteList
                    .Where(note => (note.Title != null && note.Title.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                   (note.Text != null && note.Text.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();

                Notes.Clear();
                foreach (var note in filteredNotes)
                {
                    Notes.Add(note);
                }
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


            MessagingCenter.Subscribe<NewNoteViewModel>(this, "RefreshNotes", (sender) => RefreshNotes());
            MessagingCenter.Subscribe<NewNoteViewModel, int>(this, "RefreshMainPage", (sender, folderId) =>
            {
                LoadNotesForFolder(selectedFolder);
            });
            MessagingCenter.Subscribe<object, int>(this, "RefreshMainPage", (sender, arg) =>
            {
                LoadNotesForFolder(arg);
            });

            _navigationService = DependencyService.Get<INavigationService>();

            RefreshNotes();
            InitializeWithDefaultFolder();
            LoadNotesForFolder(selectedFolder);
        }
        public void LoadNotesForFolder(int folderId)
        {
            // Retrieve notes from the database based on folderId
            var notes = AppDatabase.Instance().GetNoteList(folderId);

            // Update your UI elements to display the retrieved notes
        }

        private async Task GetDefaultFolder()
        {
            await AppDatabase.Instance().GetFirstFolder();
        }

        public async Task InitializeWithDefaultFolder()
        {
            if (SelectedFolder == null)
            {
                var defaultFolder = await AppDatabase.Instance().GetFirstFolder();
                if (defaultFolder != null)
                {
                    SelectedFolder = defaultFolder; // Set only if no folder is currently selected
                }
            }
        }

        public void OnNoteCreated(AppNote newNote)
        {
            if (isFirstNoteAfterRestart)
            {
                LoadDefaultFolderContents();
                isFirstNoteAfterRestart = false;
            }

            // Logic to save the new note...
            LoadNotesForFolder(selectedFolder);
        }
        private void LoadDefaultFolderContents()
        {
            int defaultFolderId = GetDefaultFolderId();
            AppFolder defaultFolder = AppDatabase.Instance().GetFolder(defaultFolderId);
            if (defaultFolder != null)
            {
                LoadNotesForFolder(defaultFolder);
            }
            else
            {
                // Handle the case where the default folder is not found
            }
        }
        private int GetDefaultFolderId()
        {
            // Retrieve the default folder ID from the database
            var defaultFolder = AppDatabase.Instance().GetFirstFolder();
            return defaultFolder?.Id ?? 0; // Assuming 0 is a safe default ID
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

            if (Notes != null)
            {
                Notes.Clear();

                if (selectedFolder != null)
                {
                    var notesFromDb = AppDatabase.Instance().GetNoteList(selectedFolder.Id);
                    foreach (var note in notesFromDb)
                    {
                        Notes.Add(note);
                    }
                }
            }
        }

       


        public void RefreshNotes()
        {
            if (SelectedFolder != null)
            {
                Notes.Clear();
                var notesFromDb = AppDatabase.Instance().GetNoteList(SelectedFolder.Id); // Use AppDatabase.Instance() to access your database
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
        }
        public static string StripHtml(string htmlContent)
        {
            return Regex.Replace(htmlContent, "<.*?>", string.Empty);
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
                    if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
                    {
                        var navigationPage = mainFlyoutPage.Detail as NavigationPage;
                        await navigationPage?.PushAsync(newNotePage);
                        //  await newNoteVM.OpenTEditor(); // Open TEditor with the selected note
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
        /*public void ShowOrHideToolbar()
        {
            MultiSelectEnabled = !MultiSelectEnabled;
            ShowFab = !ShowFab;
            if (MultiSelectEnabled)
                SelectionMode = SelectionMode.Multiple;
            else
                SelectionMode = SelectionMode.None;
        }*/
        public void ShowOrHideToolbar()
        {
            MultiSelectEnabled = !MultiSelectEnabled;
            ShowFab = !ShowFab;
            IsInMultiSelectMode = MultiSelectEnabled;

            // Rest of your method...
        }

        public ICommand DeleteNotesCommand => new Command(DeleteNotes);

        private async void DeleteNotes()
        {
            try
            {
                var db = AppDatabase.Instance(); // Get a reference to the database

                foreach (AppNote note in SelectedNotes)
                {
                    if (note is AppNote) // Ensure only AppNote objects are deleted
                    {
                        db.DeleteNote(note); // Use async version for database operations
                    }
                }

                RefreshNotes(); // Refresh the notes list after deletion
                SelectedNotes.Clear(); // Clear the selection
                ShowOrHideToolbar(); // Reset the UI state
                SelectionMode = SelectionMode.None;
            }
            catch (Exception ex)
            {
                // Handle errors gracefully
                await Application.Current.MainPage.DisplayAlert("Error Deleting Notes", "An error occurred while deleting notes. Please try again.", "OK");
                Debug.WriteLine("Error deleting notes: " + ex.Message);
            }
        }




        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}