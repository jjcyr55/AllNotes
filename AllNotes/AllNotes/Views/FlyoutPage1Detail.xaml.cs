using AllNotes.Database;
using AllNotes.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllNotes.Database;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using AllNotes.Repositories;
using SQLite;
using AllNotes.Services;
using AllNotes.Interfaces;
using AllNotes.ViewModels;

namespace AllNotes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlyoutPage1Detail : ContentPage
    {


        // private NoteRepository _noteRepository;
        private readonly SQLiteAsyncConnection _database;


        // public static FlyoutPage1Detail instance = null;

        private List<AppNote> listNotes;

        private string SearchKeyword;
        private Entry SearchBox;

        private MainPageViewModel _mainPageViewModel;
        private object notes;
        public List<AppNote> noteList { get; set; }
        private AppFolder _currentFolder;
        private AppFolder selectedFolder;
        private AppFolder _selectedFolder;
        private FlyoutPage1Detail _flyoutPage1Detail;
        private ObservableCollection<AppNote> _notes = new ObservableCollection<AppNote>();
      //  private NoteRepository _noteRepository = new NoteRepository();
        private FolderRepository _folderRepository = new FolderRepository();

        /*protected override async void OnAppearing()
        {
            base.OnAppearing();


            // listNotes.Reverse();
            if (listNotes != null)
            {
                listNotes.Reverse();
            }

            // NotesCV.ItemsSource = null;

            if (selectedFolder != null)
            {
                var notes = await _noteRepository.GetNotes(selectedFolder.Id);
                NotesCV.ItemsSource = notes;
                this.Title = selectedFolder.Name;
            }
        }*/
        /*protected override async void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = BindingContext as MainPageViewModel;
            if (viewModel != null)
            {
                await viewModel.RefreshData();
            }
        }*/
        /*  else
         {
             selectedFolder = await FolderRepository.Instance.GetFirstFolder();
             if (selectedFolder != null)
             {
                 var notes = await _noteRepository.GetNotesInFolder(selectedFolder.Id);
                 NotesCV.ItemsSource = notes;
                 this.Title = selectedFolder.Name;
             }
         }
     }
    }*/

        /* public async Task Reset(AppFolder folder)
         {
             selectedFolder = folder ?? await NoteRepository.Instance.GetFirstFolder();

             if (selectedFolder != null)
             {
                 var notes = await _noteRepository.GetNotesInFolder(selectedFolder.Id);
                 NotesCV.ItemsSource = notes;
                 this.Title = selectedFolder.Name;
             }
         }*/


        /* public AppFolder CurrentFolder
         {
             get => _currentFolder;
             set
             {
                 _currentFolder = value;
                 // Add logic here if you need to update the UI based on the new folder
             }
         }*/


        public NavigationPage Detail { get; internal set; }


        /* public FlyoutPage1Detail(AppFolder folder)
         {
             _currentFolder = folder;
             // Initialize the page with _currentFolder
         }
 */




       /* private async void LoadNotesForFolder()
        {
            if (_selectedFolder != null)
            {
                var notes = await _noteRepository.GetNotes(_selectedFolder.Id);
                _notes.Clear();
                foreach (var note in notes)
                {
                    _notes.Add(note);
                }

                NotesCV.ItemsSource = _notes;
            }
        }*/
        //  FOOL AROUND WITH THIS PAGE BECAUSE THERES BEEN SOME MAJOR CHANGES FOR THE POSITIVE, LOOK FOR ID PASSING POSSIBILITIES AND WHY NOTES ARE POPULATING TO ALL FOLDERS NOW

        public FlyoutPage1Detail()
        {

            // Set the flyout menu
            // this.Flyout = new MenuPage(); // Replace with your menu page

            // Set the detail page
            // this.Detail = new NavigationPage(new FlyoutPage1Detail()); // Replace with your initial detail page


            // This is supposed to go in the parameter:MainPageViewModel mainPageViewModel
            //instance = this;
           /* _mainPageViewModel = new MainPageViewModel();
            listNotes = new List<Note>();
            BindingContext = _mainPageViewModel;
            _selectedFolder = selectedFolder;*/
          
            // Additional initialization
            _notes = new ObservableCollection<AppNote>();

            InitializeComponent();
            this.BindingContext = new MainPageViewModel();
            // NavigationPage.SetHasNavigationBar(this, false);
            _mainPageViewModel = new MainPageViewModel();
           // listNotes = new List<Note>();
            BindingContext = _mainPageViewModel;
            _currentFolder = selectedFolder;


        }



        //MAY NEED TO REMOVE POSSIBLY CONFLICTING CODE: _notes = new ObservableCollection<AppNote>(); AND private ObservableCollection<AppNote> _notes;
        //  private ObservableCollection<AppNote> _notes;
        public FlyoutPage1Detail(AppFolder selectedFolder)
        {

            if (selectedFolder == null)
            {
                throw new ArgumentNullException(nameof(selectedFolder));
            }

            _selectedFolder = selectedFolder;
            InitializeComponent();
            this.BindingContext = new MainPageViewModel();

            //  LoadNotesForFolder();
            //  _currentFolder = folder;

        }



        protected override bool OnBackButtonPressed()
        {
            if (this.Detail.Navigation.NavigationStack.Count > 1)
            {
                this.Detail.Navigation.PopAsync(); // Go back to the previous page in the stack
                return true; // Prevent default back button behavior
            }

            return base.OnBackButtonPressed();
            /*if (_mainPageViewModel.MultiSelectEnabled)
                _mainPageViewModel.ShowOrHideToolbar();

            return true;*/
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchKeyword = e.NewTextValue;
            Debug.WriteLine($"Search keyword entered: {searchKeyword}");
          //  _mainPageViewModel.SearchNotes(searchKeyword);
           
        }
    

}
}