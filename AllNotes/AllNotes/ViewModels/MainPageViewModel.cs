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
using AllNotes.Views.NewNote.Popups;
using Xamarin.CommunityToolkit.Extensions;
using System.Net.Http;
using System.Xml.Linq;

namespace AllNotes.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        // private ObservableCollection<object> _selectedNotes;
        public ObservableCollection<AppNote> Notes { get; set; }

        private INavigationService _navigationService;
        private int _folderId;
        private bool _showFab = true;
        private SelectionMode _selectionMode = SelectionMode.None;
        private AppDatabase _database;
        AppFolder selectedFolder;
        private bool _multiSelectEnabled = false;
        public ICommand OpenNewNoteScreenCommand { get; private set; }
        // public ICommand SelectAllCommand { get; private set; }
        public ICommand OpenMenu2Command => new Command(OpenMenu);

        //  public ICommand ToggleSelectionCommand { get; private set; }

        // public bool IsEditMode { get; set; } // Set this based on your logic
        public bool IsNotEditMode => !IsEditMode;
        private bool isFirstNoteAfterRestart = true;
        //  public bool IsEditMode { get; set; }
        //  public ICommand SelectAllCommand { get; set; }





























        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));

                    if (_isSelected)
                        MessagingCenter.Send(this, "AddToSelectedNotes", this);
                    else
                        MessagingCenter.Send(this, "RemoveFromSelectedNotes", this);
                }
            }
        }















        /* private bool _isAllChecked;
         public bool IsAllChecked
         {
             get => _isAllChecked;
             set
             {
                 _isAllChecked = value;
                 OnPropertyChanged(nameof(IsAllChecked));
                 CheckUncheckAll(value);
             }
         }*/


        private bool _isAllChecked;
        public bool IsAllChecked
        {
            get => _isAllChecked;
            set
            {
                if (_isAllChecked != value)
                {
                    _isAllChecked = value;
                    OnPropertyChanged(nameof(IsAllChecked));
                    UpdateAllNotesCheckState(value);
                    RefreshCollectionView();
                    foreach (var note in Notes)
                    {
                        note.IsSelected = _isAllChecked;
                    }
                }
            }
        }

        private void RefreshCollectionView()
        {
            var currentNotes = Notes.ToList();
            Notes = null;
            Notes = new ObservableCollection<AppNote>(currentNotes);
            OnPropertyChanged(nameof(Notes));
        }
        private void UpdateAllNotesCheckState(bool isSelected)
        {
            foreach (var note in Notes)
            {
                note.IsSelected = isSelected;
            }
            OnPropertyChanged(nameof(Notes)); // Notify UI to refresh
        }



        private void CheckUncheckAll(bool check)
        {
            foreach (var note in Notes)
            {
                note.IsSelected = check;
            }
        }










        public ICommand ToggleSelectAllCommand { get; private set; }

        private void ToggleSelectAll()
        {
            IsAllChecked = !IsAllChecked;
            foreach (var note in Notes)
            {
                note.IsSelected = IsAllChecked;
            }
        }









        /*private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }*/
















        private ObservableCollection<AppNote> _selectedNotes = new ObservableCollection<AppNote>();
        /*public ObservableCollection<AppNote> SelectedNotes
        {
            get => _selectedNotes;
            set
            {
                _selectedNotes = value;
                OnPropertyChanged(nameof(SelectedNotes));
            }
        }*/

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
        private int _noteCount;
        public int NoteCount
        {
            get => _noteCount;
            set
            {
                _noteCount = value;
                OnPropertyChanged(nameof(NoteCount));
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







        private void OpenMenu()
        {
            var mainPagePopup = new MainPagePopup();
            mainPagePopup.BindingContext = this;
            //  var mainPagePopup = new MainPagePopup(this);
            Application.Current.MainPage.Navigation.ShowPopup(mainPagePopup);
        }
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


        public static string StripHtmlTags(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;

            var array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }


        private ICommand _editCommand;
        public ICommand EditCommand
        {
            get => _editCommand;
            set
            {
                _editCommand = value;
                OnPropertyChanged(nameof(EditCommand));
            }
        }

        /*private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;

            // Additional logic to handle when the edit mode is toggled
            // For example, clearing selected items when exiting edit mode
            if (!IsEditMode)
            {
                // Assuming you have a method or logic to clear selected notes
                RefreshNotes();
            }
        }*/
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;

            foreach (var note in Notes)
            {
                note.UpdateCheckboxVisibility(IsEditMode);
            }

            // Additional logic as required...
        }

        private void LogSelectedNotes()
        {
            Debug.WriteLine($"Testing {SelectedNotes.Count} selected notes.");
            foreach (var note in SelectedNotes)
            {
                Debug.WriteLine($"Selected Note: {note.Title}");
            }
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
                // Update selection mode based on edit mode state
                SelectionMode = _isEditMode ? SelectionMode.Multiple : SelectionMode.None;
            }
        }
        /* public void ToggleNoteSelection(AppNote note)
         {
             if (!IsEditMode)
                 return;

             if (note.IsSelected)
                 DeselectNote(note);
             else
                 SelectNote(note);
         }*/

        public void ToggleNoteSelection(AppNote selectedNote)
        {
            if (!IsEditMode)
                return;
            if (selectedNote != null)
            {
                if (_isEditMode == IsEditMode)
                {
                    ShowOrHideToolbar();
                    SelectionMode = SelectionMode.Multiple;
                    SelectedNotes.Add(selectedNote);
                }
            }

        }
        

        /* private void LongPressNote(Note selectedNote)
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
         }*/










        public void SelectNote(AppNote note)
        {
            if (note != null && !SelectedNotes.Contains(note))
            {
                note.IsSelected = true;
                SelectedNotes.Add(note);
            }
        }

        public void DeselectNote(AppNote note)
        {
            if (note != null && SelectedNotes.Contains(note))
            {
                note.IsSelected = false;
                SelectedNotes.Remove(note);
            }
        }


        public void DeleteSelectedNotes()
        {
            if (!IsEditMode)
                return;

            foreach (var note in SelectedNotes.ToList())
            {
                // Delete logic here
                Debug.WriteLine($"Deleting note: {note.Title}");
            }

            SelectedNotes.Clear();
            RefreshNotes(); // Update UI
        }













        public ICommand TestSelectedNotesCommand { get; private set; }
        public MainPageViewModel(AppFolder selectedFolder)
        {

            MessagingCenter.Subscribe<AppNote, AppNote>(this, "AddToSelectedNotes", (sender, note) =>
            {
                if (!SelectedNotes.Contains(note))
                    SelectedNotes.Add(note);
            });

            MessagingCenter.Subscribe<AppNote, AppNote>(this, "RemoveFromSelectedNotes", (sender, note) =>
            {
                if (SelectedNotes.Contains(note))
                    SelectedNotes.Remove(note);
            });





            TestSelectedNotesCommand = new Command(LogSelectedNotes);
            SelectedNotes = new ObservableCollection<AppNote>();
            Notes = new ObservableCollection<AppNote>();

            // Example of creating AppNote instances
            SelectedNotes = new ObservableCollection<AppNote>();
            // Load notes...
            foreach (var note in Notes)
            {
                note.PropertyChanged += Note_PropertyChanged;
            }

            MessagingCenter.Subscribe<AppNote>(this, "AddToSelectedNotes", note =>
            {
                if (!SelectedNotes.Contains(note))
                    SelectedNotes.Add(note);
            });

            MessagingCenter.Subscribe<AppNote>(this, "RemoveFromSelectedNotes", note =>
            {
                if (SelectedNotes.Contains(note))
                    SelectedNotes.Remove(note);
            });


            ToggleSelectAllCommand = new Command(ToggleSelectAll);
            EditCommand = new Command(ToggleEditMode);


            Notes = new ObservableCollection<AppNote>();



            /* MessagingCenter.Subscribe<MainPagePopupViewModel>(this, "ToggleEdit", (sender) =>
             {
                 IsSelectAllVisible = !IsSelectAllVisible;
             });*/

            MessagingCenter.Subscribe<NewNoteViewModel, int>(this, "NoteUpdated", (sender, folderId) =>
            {
                UpdateNoteCount();
            });
            InitializeNoteCount();

            selectedFolder = selectedFolder;

            Notes = new ObservableCollection<AppNote>();
            SelectedNotes = new ObservableCollection<AppNote>();
            TapNoteCommand = new Command<AppNote>(TapNote);
            LongPressNoteCommand = new Command<AppNote>(LongPressNote);
            _folderId = selectedFolder.Id;
            _navigationService = DependencyService.Get<INavigationService>();
            MessagingCenter.Subscribe<AppNote>(this, "RefreshNotes", (sender) => {
                RefreshNotes();
            });

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
        private void Note_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppNote.IsSelected))
            {
                var note = sender as AppNote;
                if (note != null)
                {
                    if (note.IsSelected && !SelectedNotes.Contains(note))
                    {
                        SelectedNotes.Add(note);
                        Debug.WriteLine($"Added to SelectedNotes: {note.Title}");
                    }
                    else if (!note.IsSelected && SelectedNotes.Contains(note))
                    {
                        SelectedNotes.Remove(note);
                        Debug.WriteLine($"Removed from SelectedNotes: {note.Title}");
                    }
                }
            }
        }
        private void LoadNotes()
        {
            var notesFromDb = AppDatabase.Instance().GetNoteList(selectedFolder.Id); // Replace with your actual data fetching logic

            foreach (var noteData in notesFromDb)
            {
                var note = new AppNote(); // Use parameterless constructor
                                          // Set properties of note from noteData...
                note.Initialize(SelectedNotes, RefreshNotes); // Initialize the note
                note.PropertyChanged += Note_PropertyChanged;
                Notes.Add(note);
            }
        }



        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }





        public void LoadNotesForFolder(int folderId)
        {
            var notesFromDb = AppDatabase.Instance().GetNoteList(selectedFolder.Id);
            foreach (var note in notesFromDb)
            {
                // Retrieve notes from the database based on folderId
                note.SelectionChanged += Note_SelectionChanged;
                Notes.Add(note);
                var notes = AppDatabase.Instance().GetNoteList(folderId);

                // Update your UI elements to display the retrieved notes
            }
        }
        private void Note_SelectionChanged(object sender, EventArgs e)
        {
            var note = sender as AppNote;
            if (note != null)
            {
                if (note.IsSelected && !SelectedNotes.Contains(note))
                {
                    SelectedNotes.Add(note);
                }
                else if (!note.IsSelected && SelectedNotes.Contains(note))
                {
                    SelectedNotes.Remove(note);
                }
            }
        }
        public void InitializeNoteCount()
        {
            // Assuming you have a method to get the total note count
            if (SelectedFolder != null)
            {
                NoteCount = AppDatabase.Instance().GetNoteList(SelectedFolder.Id).Count;
            }
        }

        private void UpdateNoteCount()
        {
            // Logic to update the note count
            // Assuming SelectedFolder is the currently selected folder in MainPage
            if (SelectedFolder != null)
            {
                NoteCount = AppDatabase.Instance().GetNoteList(SelectedFolder.Id).Count;
            }
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

        public ObservableCollection<AppNote> SelectedNotes { get; set; }
        public MainPageViewModel()
        {
            //may have to remove it was a test!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1

            /*foreach (var note in Notes)
            {
                note.PropertyChanged += Note_PropertyChanged;
            }*/

            MessagingCenter.Subscribe<AppNote>(this, "AddToSelectedNotes", note =>
            {
                if (!SelectedNotes.Contains(note))
                    SelectedNotes.Add(note);
            });

            MessagingCenter.Subscribe<AppNote>(this, "RemoveFromSelectedNotes", note =>
            {
                if (SelectedNotes.Contains(note))
                    SelectedNotes.Remove(note);
            });

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
        



        /* public void RefreshNotes()
         {
             if (SelectedFolder != null)
             {
                 Notes.Clear();
                 var notesFromDb = AppDatabase.Instance().GetNoteList(SelectedFolder.Id); // Use AppDatabase.Instance() to access your database

                 foreach (var note in notesFromDb)
                 {
                     note.PreviewText = StripHtmlTags(note.Text);
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
                         note.PreviewText = StripHtmlTags(note.Text);
                         Notes.Add(note);
                     }
                 }
             }
         }*/


        public void RefreshNotes()
        {
            if (SelectedFolder != null)
            {

                var notesFromDb = AppDatabase.Instance().GetNoteList(SelectedFolder.Id); // Use AppDatabase.Instance() to access your database

                var sortedNotes = notesFromDb.OrderByDescending(n => n.IsFavorite).ThenBy(n => n.Date).ToList();
                Notes.Clear();
                foreach (var note in sortedNotes) // Use orderedNotes here
                {
                    note.PreviewText = StripHtmlTags(note.Text);
                    Notes.Add(note);
                }
            }
            else
            {
                var SelectedFolder = AppDatabase.Instance().GetFirstFolder(); // Make sure this variable is not redeclared if already declared in the class
                if (SelectedFolder != null)
                {
                    if (Notes == null)
                        Notes = new ObservableCollection<AppNote>();
                    Notes.Clear();
                    var notesFromDb = AppDatabase.Instance().GetNoteList(SelectedFolder.Id);
                    var orderedNotes = notesFromDb.OrderByDescending(n => n.IsFavorite).ThenBy(n => n.Date).ToList(); // Add this line
                    foreach (var note in orderedNotes) // Use orderedNotes here
                    {
                        note.PreviewText = StripHtmlTags(note.Text);
                        Notes.Add(note);
                        OnPropertyChanged(nameof(Notes));
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

            if (!IsEditMode && selectedNote != null)
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

                        }
                    }
                }
            }
        }
       


        private bool _isLongPressMode;
        public bool IsLongPressMode
        {
            get => _isLongPressMode;
            set
            {
                if (_isLongPressMode != value)
                {
                    _isLongPressMode = value;
                    OnPropertyChanged(nameof(IsLongPressMode));
                    // Additional logic if needed
                }
            }
        }





        public Command<AppNote> LongPressNoteCommand { get; set; }
       
        public bool IsEditModeOrLongPressMode => IsEditMode || IsLongPressMode;
        private void LongPressNote(AppNote selectedNote)
        {
            if (selectedNote != null)
            {
                selectedNote.IsLongPressed = !selectedNote.IsLongPressed;
                if (_selectionMode == SelectionMode.None)
                {
                    IsEditMode = true; // Assuming IsEditMode controls the toolbar visibility
                    SelectionMode = SelectionMode.Multiple;
                    SelectedNotes.Add(selectedNote);
                }
            }
        }
        public void ShowOrHideToolbar()
        {
            MultiSelectEnabled = !MultiSelectEnabled;
            ShowFab = !ShowFab; // Assuming this controls the visibility of a floating action button

            if (MultiSelectEnabled)
            {
                SelectionMode = SelectionMode.Multiple;
                IsLongPressMode = true; // Enable long press mode
            }
            else
            {
                SelectionMode = SelectionMode.None;
                IsLongPressMode = false; // Disable long press mode
            }
        }

        public ICommand DeleteNotesCommand => new Command(DeleteNotes);


        public async void DeleteNotes()
        {
            if (!IsEditMode)
                return;
            if (!SelectedNotes.Any(n => n.IsSelected))
            {
                Debug.WriteLine("No notes selected for deletion.");
                return; // Exit if no notes are selected
            }

            try
            {
                var db = AppDatabase.Instance(); // Get a reference to the database

                int? folderId = null; // Variable to store folder ID

                var notesToDelete = SelectedNotes.ToList();
                foreach (AppNote note in SelectedNotes)
                {
                    Debug.WriteLine($"Note: {note.Title}, IsSelected: {note.IsSelected}");
                    if (note is AppNote) // Ensure only AppNote objects are deleted
                    {
                        db.DeleteNote(note); // Use async version for database operations
                        folderId = note.folderID; // Store the folder ID
                        Debug.WriteLine($"Deleting note: {note.Title}");
                    }
                }

                foreach (AppNote note in SelectedNotes.ToList())
                {
                    Debug.WriteLine($"Note: {note.Title}, IsSelected: {note.IsSelected}");
                    if (note is AppNote) // Ensure only AppNote objects are deleted
                    {
                        db.DeleteNote(note); // Use async version for database operations
                        folderId = note.folderID; // Store the folder ID
                        Debug.WriteLine($"Deleting note: {note.Title}");
                    }
                }
                // Check if folder ID was set and send message
                if (folderId.HasValue)
                {
                    MessagingCenter.Send<MainPageViewModel, int>(this, "NoteUpdated", folderId.Value);
                }


                SelectedNotes.Clear(); // Clear the selection
                RefreshNotes(); // Refresh the notes list after deletion
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
       



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(SelectedNotes))
            {
                Debug.WriteLine("SelectedNotes collection changed.");
            }
        }
    }
}