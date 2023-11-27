using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using AllNotes.Models;
//using AllNotes.Services;
using AllNotes.Views.NewNote;
using AllNotes.Repositories;
using AllNotes.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Note> Notes { get; set; }
    private ObservableCollection<object> _selectedNotes;
    private NoteRepository _noteRepository;
    private SelectionMode _selectionMode = SelectionMode.None;

    private bool _multiSelectEnabled = false;
    private bool _showFab = true;

    public bool ShowFab

    {
        get => _showFab;
        set
        {
            _showFab = value;
            OnPropertyChanged(nameof(ShowFab));
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

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MainPageViewModel()
    {
        _noteRepository = new NoteRepository();
        Notes = new ObservableCollection<Note>();
        _selectedNotes = new ObservableCollection<object>();
        TapNoteCommand = new Command<Note>(TapNote);
        LongPressNoteCommand = new Command<Note>(LongPressNote);
        GetNotesFromDb();
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

