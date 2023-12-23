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

public class MainPageViewModel : INotifyPropertyChanged
{

    //  private MainPageViewModel _mainPageViewModel;
    private readonly SQLiteAsyncConnection _database;

    private readonly INoteRepository _noteRepository;
    private readonly IFolderRepository _folderRepository;
    public List<Note> noteList { get; set; }

    private ObservableCollection<object> _selectedNotes;
    //private ObservableCollection<object> _selectedFolder;
    private SelectionMode _selectionMode = SelectionMode.None;

    private bool _multiSelectEnabled = false;
    private bool _showFab = true;

    private ObservableCollection<Note> _notes;


    private AppFolder _selectedFolder; // Selected folder
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);

        return true;
    }
    //selectedFolder.id
    public async Task InitializeAsync()
    {
        var folder = await _noteRepository.GetFirstFolder(); // Assuming this is now async
        if (folder != null)
        {
            var notes = await _noteRepository.GetNotes(folder.id); //  var notes = await _noteRepository.GetNotes(folder.id)
            foreach (var note in notes)
            {
                Notes.Add(note);
            }
        }
    }

    private ObservableCollection<Note> _originalNotes; // Add this line

    public ObservableCollection<Note> Notes
    {
        get => _notes;
        set
        {
            _notes = value;
            OnPropertyChanged(nameof(Notes));
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
            FilterNotes();
        }
    }

    private void FilterNotes()
    {
        if (_originalNotes == null)
        {
            _originalNotes = new ObservableCollection<Note>(Notes);
        }

        // If the search query is empty, show all notes
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            Notes = new ObservableCollection<Note>(_originalNotes);
        }
        else
        {
            // Convert the search query to lowercase for case-insensitive comparison
            string query = SearchQuery.ToLowerInvariant();

            // Filter notes based on whether the title OR note body contains the search query
            Notes = new ObservableCollection<Note>(_originalNotes.Where(n => n.Title.ToLowerInvariant().Contains(query) || n.Text.ToLowerInvariant().Contains(query)));
        }
    }
    //  THE SEARCH FINALLY WORKS!!!!!!!!!!!!!!!!!!! if only it will update in github, third try




    public bool ShowFab
    {
        get => _showFab;
        set
        {
            _showFab = value;
            OnPropertyChanged(nameof(ShowFab));
        }
    }



    private ObservableCollection<Note> _filteredNotes;
    private object resultCount;
    private NewNoteViewModel note;

    public ObservableCollection<Note> FilteredNotes
    {
        get => _filteredNotes;
        set => SetProperty(ref _filteredNotes, value);
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

    public ObservableCollection<object> SelectedNotes
    {
        get => _selectedNotes; set
        {
            _selectedNotes = value;
            OnPropertyChanged(nameof(SelectedNotes));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MainPageViewModel()
    {
        _noteRepository = new NoteRepository();
        //  _folderRepository = new FolderRepository();
        _notes = new ObservableCollection<Note>();
        _selectedNotes = new ObservableCollection<object>();
        TapNoteCommand = new Command<Note>(TapNote);
        LongPressNoteCommand = new Command<Note>(LongPressNote);
        // GetNotesFromDb();


        _filteredNotes = new ObservableCollection<Note>();
        FilteredNotes = new ObservableCollection<Note>(_notes);

        InitializeViewModel();
        /*AppFolder folder = AppDatabase.Instance().GetFirstFolder();
        if (folder != null)
        {
            noteList = AppDatabase.Instance().GetNoteList(folder.id);
        }*/

        //BindingContext = this;


    }

    private async void InitializeViewModel()
    {
        // You might want to start with a default folder or no folder
        AppFolder initialFolder = await GetInitialFolder();
        await Reset(initialFolder);
    }

    private async Task<AppFolder> GetInitialFolder()
    {
        // Logic to determine the initial folder
        // This could be the first folder in the database, a specific "default" folder, etc.
        // For example, you might do something like this:
        return await _noteRepository.GetFirstFolder(); // Or however you get your initial folder
    }





    public AppFolder SelectedFolder
    {
        get => _selectedFolder;
        set
        {
            if (_selectedFolder != value)
            {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
                Reset(_selectedFolder); // Reset when the selected folder changes
            }
        }
    }

    /* public AppFolder SelectedFolder
     {
         get => _selectedFolder;
         set
         {
             if (_selectedFolder != value)
             {
                 _selectedFolder = value;
                 // Use the null-coalescing operator to handle null values
                 selectedFolderID = _selectedFolder?.Id ?? 0; // Default to 0 if Id is null
                 OnPropertyChanged(nameof(SelectedFolder));
                 // Add any additional logic when a folder is selected
             }
         }
     }*/
    public async Task SearchNotes(string searchKeyword)
    {
        Debug.WriteLine($"Searching notes for: {searchKeyword}");

        try
        {
            var searchResults = await _noteRepository.SearchNotesAsync(searchKeyword);

            if (searchResults != null)
            {
                SelectedNotes = new ObservableCollection<object>(_selectedNotes.Cast<object>().ToList());
                Debug.WriteLine($"Found {resultCount} notes matching the search.");

                // Assuming you have a property named FilteredNotes, update it
                FilteredNotes = new ObservableCollection<Note>(searchResults);
            }
            else
            {
                Debug.WriteLine("Search results are null.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred during search: {ex.Message}");
            // Handle the exception as needed
        }
    }




    private void LogException(Exception ex)
    {
        // You can implement logging logic here, for example, using a logging framework.
        // Example: Serilog.Logger.Error(ex, "An error occurred in SearchNotes");
    }

    private void ShowErrorMessage(string message)
    {
        // You can implement a method to show the error message to the user.
        // This might involve displaying a dialog or updating a status bar.
        // Example: MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }


    public ICommand OpenNewNoteScreenCommand => new Command(OpenNewNoteScreen);

    /*private async void OpenNewNoteScreen()
    {
        *//*var newNoteVM = new NewNoteViewModel(this, null);
        var newNotePage = new NewNotePage(newNoteVM);
        newNotePage.BindingContext = newNoteVM;
        await Application.Current.MainPage.Navigation.PushAsync(newNotePage);*//*
        
        var flyoutPage = Application.Current.MainPage as FlyoutPage;
        if (flyoutPage != null)
        {
            var navigationPage = flyoutPage.Detail as NavigationPage;
            if (navigationPage != null)
            {
                // Check if there's a currently selected folder in the MainPageViewModel
                int folderId = this.SelectedFolder?.Id ?? 0; // Default to 0 or another default value if no folder is selected

                // Create a new instance of NewNoteViewModel, passing null for a new note and the current folder's ID
                var newNoteVM = new NewNoteViewModel(this, null, folderId);
                var newNotePage = new NewNotePage(newNoteVM);
                newNotePage.BindingContext = newNoteVM;
                await navigationPage.PushAsync(newNotePage);
               
            }
        }
    }*/
    private async void OpenNewNoteScreen()
    {
        var flyoutPage = Application.Current.MainPage as FlyoutPage;
        if (flyoutPage != null)
        {
            var navigationPage = flyoutPage.Detail as NavigationPage;
            if (navigationPage != null)
            {
                int folderId = this.SelectedFolder?.Id ?? 0; // Ensure this is the correct folder ID
                var newNoteVM = new NewNoteViewModel(this, null, folderId);
                var newNotePage = new NewNotePage(newNoteVM);
                newNotePage.BindingContext = newNoteVM;
                await navigationPage.PushAsync(newNotePage);
            }
        }
    }
    /* public async void GetNotesFromDb()
     {
         Notes.Clear();
         var notes = await _noteRepository.GetNotes();//var allNotes = await _noteRepository.GetNotes(folder.id);
         foreach (Note note in notes)
         {
             Notes.Add(note);
         }
     }*/

    public Command<Note> TapNoteCommand { get; set; }

    /*private async void TapNote(Note selectedNote)
    {
        if (selectedNote != null)
        {
            if (_selectionMode == SelectionMode.None)
            {
                var newNoteVM = new NewNoteViewModel(this, selectedNote);
                var newNotePage = new NewNotePage(newNoteVM);
                newNotePage.BindingContext = newNoteVM;
                 await Application.Current.MainPage.Navigation.PushAsync(newNotePage);
               //await Navigation.PushModalAsync(new NavigationPage(new FlyoutPage1Detail(new NewNotePage(note))));
            }
        }
    }*/
    private async void TapNote(Note selectedNote)
    {
        if (selectedNote != null)
        {
            if (_selectionMode == SelectionMode.None)
            {
                var flyoutPage = Application.Current.MainPage as FlyoutPage;
                if (flyoutPage != null)
                {
                    var navigationPage = flyoutPage.Detail as NavigationPage;
                    if (navigationPage != null)
                    {
                        // Assuming selectedNote already contains the FolderId
                        var newNoteVM = new NewNoteViewModel(this, selectedNote, selectedNote.FolderId);
                        var newNotePage = new NewNotePage(newNoteVM);
                        newNotePage.BindingContext = newNoteVM;
                        await navigationPage.PushAsync(newNotePage);
                    }
                }
            }
        }
    }




    AppFolder selectedFolder;
    public async Task Reset(AppFolder folder)
    {
        selectedFolder = folder;

        // Clear existing notes
        Notes.Clear();

        if (selectedFolder != null)
        {
            // Retrieve notes for the selected folder from the database
            var notesFromDb = await _noteRepository.GetNotesInFolder(selectedFolder.Id);
            foreach (var note in notesFromDb)
            {
                Notes.Add(note);
            }
        }
        else
        {
            // If no folder is selected, you might want to fetch all notes or handle it differently
            var allNotes = await _noteRepository.GetNotes(folder.id);//var allNotes = await _noteRepository.GetNotes(selectedFolder.id);
            foreach (var note in allNotes)
            {
                Notes.Add(note);
            }
        }

        // Update any other relevant properties, like the title
        // This part depends on how you're displaying the title in your app
        // Example: CurrentFolderTitle = selectedFolder?.Name ?? "All Notes";
    }
    public Command<Note> LongPressNoteCommand { get; set; }

    private void LongPressNote(Note selectedNote)
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

    public ICommand DeleteNotesCommand => new Command(DeleteNotes);

    public object NotesCV { get; private set; }

    // public MainPageViewModel BindingContext { get; }

    private async void DeleteNotes()
    {
        foreach (Note note in SelectedNotes)
        {
            await _noteRepository.DeleteNote(note);
        }
        ShowOrHideToolbar();
        SelectionMode = SelectionMode.None;

        // Fetch the first folder (or default folder) to reset the view
        AppFolder firstFolder = await _noteRepository.GetFirstFolder();
        await Reset(firstFolder);
    }


}