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
        public ICommand OpenToolBarMenuCommand => new Command(OpenToolbarMenu);

        
        public bool IsNotEditMode => !IsEditMode;
        private bool isFirstNoteAfterRestart = true;



        








        private string _favoriteActionText;
        public string FavoriteActionText
        {
            get => _favoriteActionText;
            set
            {
                _favoriteActionText = value;
                OnPropertyChanged(nameof(FavoriteActionText));
              //  UpdateFavoriteActionText();
            }
        }


      
        private void UpdateFavoriteActionText()
        {
            if (!SelectedNotes.Any())
            {
                FavoriteActionText = "No Notes Selected";
                return;
            }

            var favoriteCount = SelectedNotes.Count(note => note.IsFavorite);
            var nonFavoriteCount = SelectedNotes.Count - favoriteCount;

            if (favoriteCount == SelectedNotes.Count)
            {
                // All selected notes are favorites
                FavoriteActionText = "Unfavorite";
            }
            else if (nonFavoriteCount == SelectedNotes.Count)
            {
                // No selected notes are favorites
                FavoriteActionText = "Favorite";
            }
            else
            {
                // Mix of favorite and non-favorite notes
                FavoriteActionText = "Favorite All";
            }
        }

        private void ExecuteFavoriteAction()
        {
            switch (FavoriteActionText)
            {
                case "Favorite":
                     AddSelectedNotesToFavorites();
                  // ToggleFavoriteStatusForSelectedNotes();
                    break;
                case "Unfavorite":
                    UnfavoriteSelectedNotes();
                    break;
                case "Favorite All":
                    FavoriteAllSelectedNotes();
                    break;
            }
            RefreshNotes(); 
        
            ResetSelectionStates();
        }
        private void ResetSelectionStates()
        {
            if (Notes != null && SelectedNotes != null)
            {
                // Create a temporary list to avoid modifying the collection while iterating
                var tempSelectedNotes = SelectedNotes.ToList();

                foreach (var note in tempSelectedNotes)
                {
                    note.IsSelected = false;
                    note.IsChecked = false;
                }

                SelectedNotes.Clear();
                UpdateFavoriteActionText();
            }
        }
        
        private void AddSelectedNotesToFavorites()
        {
            foreach (var note in SelectedNotes)
            {
                note.IsFavorite = true;
                UpdateNoteInDatabase(note);
            }
            UpdateFavoriteActionText();
        }

        private void UnfavoriteSelectedNotes()
        {
            foreach (var note in SelectedNotes.Where(n => n.IsFavorite))
            {
                note.IsFavorite = false;
                UpdateNoteInDatabase(note);
            }
            UpdateFavoriteActionText();
        }

        private void ToggleFavoriteStatusForSelectedNotes()
        {
            foreach (var note in SelectedNotes)
            {
                note.IsFavorite = !note.IsFavorite;
                UpdateNoteInDatabase(note);
            }
            UpdateFavoriteActionText();
        }
        private void FavoriteAllSelectedNotes()
        {
            foreach (var note in SelectedNotes)
            {
                if (!note.IsFavorite) // Check if the note is not already a favorite
                {
                    note.IsFavorite = true; // Set as favorite
                    UpdateNoteInDatabase(note);
                }
            }
            UpdateFavoriteActionText();
        }

      
        private void UpdateNoteInDatabase(AppNote note)
        {
            var dbNote = AppDatabase.Instance().GetNoteById(note.id);
            if (dbNote != null)
            {
                // Update only the fields that need to be persistent
                dbNote.IsFavorite = note.IsFavorite;
                // ... other fields if necessary

                AppDatabase.Instance().UpdateNote(dbNote);
            }
        }



        public void OnSelectedNotesChanged()
        {
            foreach (var note in SelectedNotes)
            {
                // Debug log to check if a note is favorited
                Debug.WriteLine($"Note {note.Title} is {(note.IsFavorite ? "a favorite" : "not a favorite")}");
            }

            UpdateFavoriteActionText();
        }
       /* public void OnSelectedNotesChanged()
        {
            UpdateFavoriteActionText(); // Call this method whenever the selection changes
          
        }*/
        private void RefreshFavoriteStatusOfSelectedNotes()
        {
            foreach (var note in SelectedNotes)
            {
                var refreshedNote = AppDatabase.Instance().GetNoteById(note.id); // Assuming you have such a method
                note.IsFavorite = refreshedNote?.IsFavorite ?? note.IsFavorite;
            }
        }
        
       
        private void ToggleEditMode()
        {
            if (IsEditMode)
            {
                ExitEditMode(); // Call the unified method to exit edit mode
            }
            else
            {
                EnterEditMode(); // Call the unified method to enter edit mode
            }
        }
        public Command<AppNote> LongPressNoteCommand { get; set; }

        public bool IsEditModeOrLongPressMode => IsEditMode;
        private void LongPressNote(AppNote selectedNote)
        {
            IsEditMode = true;
           
            OnSelectedNotesChanged();
           
        }
        private void EnterEditMode()
        {
            IsEditMode = true;
          
            OnSelectedNotesChanged();
          
            // Other logic specific to entering edit mode
        }
        public void ExitEditMode()
        {
            IsEditMode = false;
            foreach (var note in Notes)
            {
                note.IsSelected = false;
            }
            SelectedNotes.Clear();
              ResetEditModeState();
          //  RefreshNotes(); // Refresh the UI to reflect changes
            UpdateFavoriteActionText(); // Reset favorite action text
            // Reset any other temporary states
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

        private bool _isLongPressMode;
        private object selectedNote;

        public bool IsLongPressMode
        {
            get => _isLongPressMode;
            set
            {
                if (_isLongPressMode != value)
                {
                    _isLongPressMode = value;
                    OnSelectedNotesChanged();
                    ResetEditModeState();
                    OnPropertyChanged(nameof(IsLongPressMode));
                    // Additional logic if needed
                }
            }
        }
       



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
                    OnSelectedNotesChanged();
                    // RefreshNotes();
                    UpdateFavoriteActionText();
                 //   ClearSelectionState();
                    if (_isSelected)
                        MessagingCenter.Send(this, "AddToSelectedNotes", this);
                    else
                        MessagingCenter.Send(this, "RemoveFromSelectedNotes", this);
                }
            }
        }
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                    OnSelectedNotesChanged();
                    RefreshCollectionView();
                 //   ResetEditModeState();
                    //  RefreshNotes();
                    UpdateFavoriteActionText();
                  //  ClearSelectionState();
                    OnIsCheckedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler OnIsCheckedChanged;


       
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
                    RefreshCollectionView();//this must stay for all notes to be checked
                    OnSelectedNotesChanged();
                  //  ClearSelectionState();
                    //  ResetEditModeState();
                    // RefreshNotes();
                    // Only refresh the collection view if there is a need to reflect changes in UI
                    // RefreshCollectionView(); // Consider if this is really necessary
                    UpdateFavoriteActionText(); // Update if needed based on the current state
                }
            }
        }
        private void OpenToolbarMenu()
        {
            var toolbarMorePopup = new ToolbarMorePopup();
            toolbarMorePopup.BindingContext = this;
            //  RefreshFavoriteStatusOfSelectedNotes();
            //  RefreshFavoriteStatusOfSelectedNotes();
            //  UpdateFavoriteStatusOfSelectedNotes();
            //   OnSelectedNotesChanged();
           
            //   RefreshCollectionView();
            OnPropertyChanged(nameof(Notes));
           // ResetEditModeState();
            UpdateFavoriteActionText();
            Application.Current.MainPage.Navigation.ShowPopup(toolbarMorePopup);
        }
        //MAKE A REFERENCE CALL TO THIS METHOD WHEN TOGGLING EDIT MODE AND POSSIBLY ELSEWHERE
        public void ResetEditModeState()
        {
            // Reset the IsSelected property for all notes
            foreach (var note in Notes)
            {
                note.IsSelected = false;
            }

            // Clear the selected notes collection
            SelectedNotes.Clear();

            // Optionally, refresh the notes collection view
            RefreshCollectionView();

            // Reset any other flags or properties related to edit mode
            IsEditMode = false;
            IsAllChecked = false;

            // Reset the FavoriteActionText to its default state
            FavoriteActionText = "No Notes Selected";

            // Any other UI state reset if necessary
            // ...
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
          //  RefreshCollectionView();
        }


        public ICommand ToggleSelectAllCommand { get; private set; }

        private void ToggleSelectAll()
        {
            IsAllChecked = !IsAllChecked;
            foreach (var note in Notes)
            {
                note.IsSelected = IsAllChecked;
            }
            OnSelectedNotesChanged();
           
            // ResetEditModeState();
        }






        private void UpdateFavoriteStatusOfSelectedNotes()
        {
            bool anyFavoriteChange = false; // Flag to check if any favorite status changed

            foreach (var note in SelectedNotes)
            {
                // Assuming you have a method to toggle favorite status in your note model or service
                // Toggle the favorite status
                note.IsFavorite = !note.IsFavorite;

                // Update the note in the database
                // AppDatabase.Instance().UpdateNoteFavoriteStatus(note.id, note.IsFavorite);

                anyFavoriteChange = true;
            }

            if (anyFavoriteChange)
            {
                // If any favorite status changed, update the FavoriteActionText and refresh the notes
                UpdateFavoriteActionText();
                RefreshNotes(); // Assuming this will update the UI with the new favorite status
            }
        }


        private ObservableCollection<AppNote> _selectedNotes = new ObservableCollection<AppNote>();
       

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

       
        
        private void LogSelectedNotes()
        {
            Debug.WriteLine($"Testing {SelectedNotes.Count} selected notes.");
            foreach (var note in SelectedNotes)
            {
                Debug.WriteLine($"Selected Note: {note.Title}");
            }
        }
        //POSSIBLE RESET CALLS HERE OR OTHER METHOD CALLS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
        //POSSIBLE RESET CALLS HERE OR OTHER METHOD CALLS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                _isFavorite = value;
               
                OnPropertyChanged(nameof(IsFavorite));
                MessagingCenter.Send(this, "NoteFavoriteStatusChanged", this);
            }
        }




        private void NoteOnIsCheckedChanged(object sender, EventArgs e)
        {
            OnSelectedNotesChanged();
        }

        private void ClearSelectionState()
        {
            if (Notes != null)
            {
                foreach (var note in Notes)
                {
                    note.IsSelected = false;
                }
                SelectedNotes.Clear(); // Clear the selected notes collection
                UpdateFavoriteActionText(); // Update the favorite action text
                OnPropertyChanged(nameof(SelectedNotes)); // Notify that the selection has changed
            }
        }

        public ICommand ExitEditModeCommand { get; private set; }
        public ICommand FavoriteActionCommand { get; private set; }
        public ICommand MoveToDifferentFolderCommand { get; private set; }


        private AppNote _selectedNoteForMoving;
        public AppNote SelectedNoteForMoving
        {
            get => _selectedNoteForMoving;
            set
            {
                _selectedNoteForMoving = value;
                OnPropertyChanged(nameof(SelectedNoteForMoving));
            }
        }

        /*private async void OpenManageFoldersPage()
        {
            // Assuming 'SelectedNotes' is a collection in your ViewModel that holds the selected notes.
            var selectedNotes = this.SelectedNotes.ToList();

            // Check if there are any selected notes
            if (selectedNotes.Count == 0)
            {
                // Optionally, show a message to the user indicating that no notes are selected
                await Application.Current.MainPage.DisplayAlert("No Notes Selected", "Please select at least one note to move.", "OK");
                return;
            }

            // Create an instance of ManageFoldersViewModel with the selected notes
            var manageFoldersViewModel = new ManageFoldersViewModel(selectedNotes);
            var manageFoldersPage = new ManageFolders(manageFoldersViewModel);

            // Navigate to the ManageFolders page
            if (Application.Current.MainPage is FlyoutPage flyoutPage)
            {
                if (flyoutPage.Detail is NavigationPage navigationPage)
                {
                    await navigationPage.Navigation.PushAsync(manageFoldersPage);
                }
                else
                {
                    // Handle other cases or log an error
                }
            }
            else
            {
                // Handle other cases or log an error
            }
        }*/


        /*public async Task OpenMoveNotePopupAsync()
        {
            if (SelectedNotes == null || !SelectedNotes.Any())
            {
                // Optionally handle the case where no notes are selected
                await Application.Current.MainPage.DisplayAlert("No Notes Selected", "Please select at least one note to move.", "OK");
                return;
            }

            // Initialize MoveNotePopupViewModel with the selected notes
            var moveNotePopupViewModel = new MoveNotePopupViewModel(SelectedNotes.ToList());

            // Create MoveNotePopup with its ViewModel
            var moveNotePopupPage = new MoveNotePopup(moveNotePopupViewModel);

            // Open MoveNotePopup as a modal page
            await Application.Current.MainPage.Navigation.PushModalAsync(moveNotePopupPage);
        }*/
        /*private async Task OpenMoveNotePopupAsync()
        {
            try
            {
                // Simplified logic to open the modal
                var moveNotePopupPage = new MoveNotePopup(); // Open with default constructor for testing

                if (Application.Current.MainPage is FlyoutPage flyoutPage)
                {
                    if (flyoutPage.Detail is NavigationPage navigationPage)
                    {
                        System.Diagnostics.Debug.WriteLine("Attempting to open modal...");
                        await navigationPage.Navigation.PushModalAsync(moveNotePopupPage);
                        System.Diagnostics.Debug.WriteLine("Modal opened.");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Current MainPage is not a NavigationPage.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in OpenMoveNotePopupAsync: " + ex.Message);
            }
        }*/
        private async Task OpenMoveNotePopupAsync()
        {
            try
            {
                if (SelectedNotes == null || !SelectedNotes.Any())
                {
                    await Application.Current.MainPage.DisplayAlert("No Notes Selected", "Please select at least one note to move.", "OK");
                    return;
                }

                var moveNotePopupViewModel = new MoveNotePopupViewModel(SelectedNotes.ToList());
                var moveNotePopupPage = new MoveNotePopup(moveNotePopupViewModel);
                moveNotePopupViewModel.SetNotesToMove(SelectedNotes.ToList());

                if (Application.Current.MainPage is FlyoutPage flyoutPage)
                {
                    if (flyoutPage.Detail is NavigationPage navigationPage)
                    {
                        await navigationPage.Navigation.PushModalAsync(moveNotePopupPage);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Current MainPage is not a FlyoutPage.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in OpenMoveNotePopupAsync: " + ex.Message);
            }
        }
        /*public async Task OpenMoveNotePopupAsync()
        {
            if (SelectedNotes.Any())
            {
                var moveNotePopupViewModel = new MoveNotePopupViewModel();
                moveNotePopupViewModel.LoadFolders(); // Explicitly call LoadFolders if needed
                moveNotePopupViewModel.SetNotesToMove(SelectedNotes.ToList());

                var moveNotePopupPage = new MoveNotePopup();
                moveNotePopupPage.BindingContext = moveNotePopupViewModel;

                await Application.Current.MainPage.Navigation.PushModalAsync(moveNotePopupPage);
            }
        }*/

        /* public async Task OpenMoveNotePopupAsync()
         {
             if (SelectedNotes.Any())
             {
                 //  var moveNotePopupViewModel = new MoveNotePopupViewModel(SelectedNotes.ToList());
                 //  var moveNotePopupPage = new MoveNotePopup(moveNotePopupViewModel);
                 // Create an instance of MoveNotePopupViewModel, passing 'this' as MainPageViewModel
                 var moveNotePopupViewModel = new MoveNotePopupViewModel(this);
                 var moveNotePopupPage = new MoveNotePopup(); // Assuming this is your page class
                 moveNotePopupPage.BindingContext = moveNotePopupViewModel;
                 await Application.Current.MainPage.Navigation.PushModalAsync(moveNotePopupPage);
                 // Check if the MainPage is a FlyoutPage and navigate accordingly
                 *//*if (Application.Current.MainPage is FlyoutPage flyoutPage)
                 {
                     if (flyoutPage.Detail is NavigationPage navigationPage)
                     {
                         await navigationPage.Navigation.PushModalAsync(moveNotePopupPage);
                     }
                 }
                 else
                 {
                     System.Diagnostics.Debug.WriteLine("Current MainPage is not a FlyoutPage.");
                 }
             }
             *//*
             }
         }*/

        private void MoveNotesToFolder(List<AppNote> notesToMove, AppFolder targetFolder)
{
            // Implement the logic to move notesToMove to targetFolder
            // Update the database, etc.

            // Refresh the UI or navigate back as needed
        }
        public ICommand OpenMoveNotePopupCommand { get; private set; }

        

        // Other initialization code...
        public ICommand OpenManageFoldersCommand { get; private set; }


        public ICommand TestSelectedNotesCommand { get; private set; }
        public MainPageViewModel(AppFolder selectedFolder)
        {
            MessagingCenter.Subscribe<MoveNotePopupViewModel, AppFolder>(this, "NotesMoved", (sender, folderId) =>
            {
                // Refresh the notes list for the affected folder
                LoadNotesForFolder(selectedFolder);
            });
            OpenMoveNotePopupCommand = new Command(async () => await OpenMoveNotePopupAsync());
            // OpenManageFoldersCommand = new Command(OpenManageFoldersPage);
            //  MoveToDifferentFolderCommand = new Command(ShowMoveNotePopup);
            MessagingCenter.Subscribe<MoveNotePopupViewModel, AppNote>(this, "NoteMoved", (sender, movedNote) =>
            {
                // Logic to handle the UI update after a note has been moved
                RefreshNotes(); // Update your notes list
            });

            MessagingCenter.Subscribe<App>(this, "BackButtonPressed", (sender) =>
            {
                if (IsEditMode)
                {
                    // Logic to exit edit mode
                    ExitEditMode();
                }
                else
                {
                    // Handle other back button logic as needed
                }
            });


            ExitEditModeCommand = new Command(ExitEditMode);

            SelectedNotes = new ObservableCollection<AppNote>();
          
            ClearSelectionState();
            MessagingCenter.Subscribe<AppNote>(this, "NoteSelectionChanged", note =>
            {
                OnSelectedNotesChanged();
              
            });

            MessagingCenter.Subscribe<AppNote>(this, "AddToSelectedNotes", note =>
            {
                OnSelectedNotesChanged();

            });
            MessagingCenter.Subscribe<AppNote>(this, "RemoveFromSelectedNotes", note =>
            {
                OnSelectedNotesChanged();

            });
          

            FavoriteActionCommand = new Command(ExecuteFavoriteAction);

            

            MessagingCenter.Subscribe<AppNote>(this, "AddToSelectedNotes", note =>
            {
                if (!SelectedNotes.Contains(note))
                    SelectedNotes.Add(note);
                OnSelectedNotesChanged(); // Call when a note is selected
            });

            MessagingCenter.Subscribe<AppNote>(this, "RemoveFromSelectedNotes", note =>
            {
                if (SelectedNotes.Contains(note))
                    SelectedNotes.Remove(note);
                OnSelectedNotesChanged(); // Call when a note is deselected
            });

            SelectedNotes = new ObservableCollection<AppNote>();


            
            UpdateFavoriteActionText();

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

            //THIS AND 2ND LOADNOTESFORFOLDER MAY NEED TO BE UNCOMMENTED IF THERES ISSUES WITH FEATURES
           /* MessagingCenter.Subscribe<object, int>(this, "RefreshMainPage", (sender, arg) =>
            {
                LoadNotesForFolder(arg);
            });*/

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




        /*public void LoadNotesForFolder(int folderId)
        {
            foreach (var note in Notes)
            {
                note.OnIsCheckedChanged += NoteOnIsCheckedChanged;
            }
            var notesFromDb = AppDatabase.Instance().GetNoteList(selectedFolder.Id);
            foreach (var note in notesFromDb)
            {
                // Retrieve notes from the database based on folderId
                note.SelectionChanged += Note_SelectionChanged;
                Notes.Add(note);
                var notes = AppDatabase.Instance().GetNoteList(folderId);

                // Update your UI elements to display the retrieved notes
            }
        }*/
        /*public void LoadNotesForFolder(AppFolder folder)
        {
            selectedFolder = folder;
            Notes.Clear();
            ResetEditModeState();

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
        }*/
        public void LoadNotesForFolder(AppFolder folder)
        {
            selectedFolder = folder;
            Notes.Clear();
            ResetEditModeState();
            ClearSelectionState();

            if (selectedFolder != null)
            {
                var notesFromDb = AppDatabase.Instance().GetNoteList(selectedFolder.Id);
                foreach (var note in notesFromDb)
                {
                    note.IsChecked = false;
                    note.IsSelected = false; // Reset selection state
                    Notes.Add(note);
                }
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




       

        private void SetLastUsedFolderId(int folderId)
        {
            // Save the last used folder ID to preferences
            Preferences.Set("LastUsedFolderId", folderId);
        }





        public ObservableCollection<AppNote> SelectedNotes { get; set; }
        public MainPageViewModel()
        {
           
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
             //  var notesFromDb = AppDatabase.Instance().GetNoteList(SelectedFolder?.Id ?? 0);
               
                  var notesFromDb = AppDatabase.Instance().GetNoteList(SelectedFolder.Id); // Use AppDatabase.Instance() to access your database
               // Notes = new ObservableCollection<AppNote>(notesFromDb.OrderByDescending(n => n.IsFavorite).ThenBy(n => n.Date));


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