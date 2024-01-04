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
using AllNotes.Views.NewNote;

namespace AllNotes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlyoutPage1Detail : ContentPage
    {


        private readonly SQLiteAsyncConnection _database;

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
      
        private FolderRepository _folderRepository = new FolderRepository();

        

        

        public NavigationPage Detail { get; internal set; }


       

        public FlyoutPage1Detail()
        {


            _notes = new ObservableCollection<AppNote>();

            InitializeComponent();
            this.BindingContext = new MainPageViewModel();
            // NavigationPage.SetHasNavigationBar(this, false);
            _mainPageViewModel = new MainPageViewModel();
           
            BindingContext = _mainPageViewModel;
            _currentFolder = selectedFolder;


        }
       
    
   
    public FlyoutPage1Detail(AppFolder selectedFolder)
        {

            if (selectedFolder == null)
            {
                throw new ArgumentNullException(nameof(selectedFolder));
            }

            _selectedFolder = selectedFolder;
            InitializeComponent();
            this.BindingContext = new MainPageViewModel();


        }
        private async void GoToNewNotePage()
        {
            // Create an instance of NewNoteViewModel
            var newNoteViewModel = new NewNoteViewModel();

            // Pass the ViewModel to NewNotePage
            var newNotePage = new NewNotePage(newNoteViewModel);

            // Use Xamarin.Forms navigation to push the new page
            await Navigation.PushAsync(newNotePage);
        }

        // Event handler for a UI action, like a button click
        private void Button_Clicked(object sender, EventArgs e)
        {
            GoToNewNotePage();
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