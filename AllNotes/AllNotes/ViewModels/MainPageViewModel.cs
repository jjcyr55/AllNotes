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
        /*public ObservableCollection<AppNote> Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }*/
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
                    // LoadNotesForFolder(selectedFolder);
                    RefreshNotes();
                   // SetDefaultFolder();


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
            LoadNotesForFolder(selectedFolder);
            TapNoteCommand = new Command<AppNote>(TapNote);
            LongPressNoteCommand = new Command<AppNote>(LongPressNote);
            _folderId = selectedFolder.Id;
            _navigationService = DependencyService.Get<INavigationService>();
           // SetDefaultFolder();
            OpenNewNoteScreenCommand = new Command(OpenNewNoteScreen);
            MessagingCenter.Subscribe<NewNoteViewModel>(this, "RefreshNotes", (sender) => RefreshNotes());
            RefreshNotes();
           // InitializeAsync();
         //   InitializeWithDefaultFolder();
        }

        /* private async Task InitializeAsync()
         {
             await SetDefaultFolderAsync();
         }*/


        private async void InitializeWithDefaultFolder()
        {
            var defaultFolder = await AppDatabase.Instance().GetFirstFolder();
            if (defaultFolder != null)
            {
                SelectedFolder = defaultFolder;
            }
            else
            {
                // Handle the case if there is no default folder
                // You might want to create a default folder here if it's mandatory
            }
        }


        /*private async Task SetDefaultFolderAsync()
        {
            var defaultFolder = await AppDatabase.Instance().GetFirstFolder(); // Assuming this is an async method
            if (defaultFolder != null)
            {
                SelectedFolder = defaultFolder; // This will also load notes for the default folder
            }
            else
            {
                // Handle the scenario where no folders are available
            }
        }*/
        /* public void RefreshNotes()
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
         }*/
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

        /* public void LoadNotesForFolder(AppFolder folder)
         {
             Notes.Clear();
             if (folder != null)
             {
                 var notesFromDb = AppDatabase.Instance().GetNoteList(folder.Id);
                 foreach (var note in notesFromDb)
                 {
                     Notes.Add(note);
                 }
             }
         }*/
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
        private void RefreshNotes()
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
              var  SelectedFolder = AppDatabase.Instance().GetFirstFolder();
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
        /*public async void GetNotesFromDb(AppFolder folder)
        {
            Notes.Clear();
            var notes = AppDatabase.Instance().GetNoteList(folder.Id);
            foreach (AppNote note in notes)
            {
                Notes.Add(note);
            }
        }*/


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


        /*private async void OpenNewNoteScreen()
        {
            // Navigate to NewNotePage, passing the folder ID
            await _navigationService.NavigateToNewNotePage(_folderId);
        }*/
        private async void OpenNewNoteScreen()
        {
            // Pass the current folder ID if available, otherwise pass null
            int folderId = SelectedFolder?.Id ?? 0; // 0 or any default value you consider appropriate
            await _navigationService.NavigateToNewNotePage(folderId);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}