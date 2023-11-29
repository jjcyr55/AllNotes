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

public class MainPageViewModel : INotifyPropertyChanged
{
 
    private MainPageViewModel _mainPageViewModel;
    private readonly SQLiteAsyncConnection _database;

    private readonly INoteRepository _noteRepository;

 
    private ObservableCollection<object> _selectedNotes;
    private SelectionMode _selectionMode = SelectionMode.None;

    private bool _multiSelectEnabled = false;
    private bool _showFab = true;

  
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);

        return true;
    }

    private ObservableCollection<Note> _notes;
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


    /* private void FilterNotes()
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

             // Filter notes based on whether the title contains the search query
             Notes = new ObservableCollection<Note>(_originalNotes.Where(n => n.Title.ToLowerInvariant().Contains(query)));
         }
     }
    */



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
        _notes = new ObservableCollection<Note>();
        _selectedNotes = new ObservableCollection<object>();
        TapNoteCommand = new Command<Note>(TapNote);
        LongPressNoteCommand = new Command<Note>(LongPressNote);
        GetNotesFromDb();

        _filteredNotes = new ObservableCollection<Note>();
        FilteredNotes = new ObservableCollection<Note>(_notes);
    }

    
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

    private async void OpenNewNoteScreen()
    {
        var newNoteVM = new NewNoteViewModel(this, null);
        var newNotePage = new NewNotePage(newNoteVM);
        newNotePage.BindingContext = newNoteVM;
        await Application.Current.MainPage.Navigation.PushAsync(newNotePage);
    }

    public async void GetNotesFromDb()
    {
        Notes.Clear();
        var notes = await _noteRepository.GetNotes();
        foreach (Note note in notes)
        {
            Notes.Add(note);
        }
    }

    public Command<Note> TapNoteCommand { get; set; }

    private async void TapNote(Note selectedNote)
    {
        if (selectedNote != null)
        {
            if (_selectionMode == SelectionMode.None)
            {
                var newNoteVM = new NewNoteViewModel(this, selectedNote);
                var newNotePage = new NewNotePage(newNoteVM);
                newNotePage.BindingContext = newNoteVM;
                await Application.Current.MainPage.Navigation.PushAsync(newNotePage);
            }
        }
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

    private async void DeleteNotes()
    {
        foreach (Note note in SelectedNotes)
        {
            await _noteRepository.DeleteNote(note);
        }
        ShowOrHideToolbar();
        SelectionMode = SelectionMode.None;
        GetNotesFromDb();
    }

}

