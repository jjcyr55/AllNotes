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
        private MenuPageViewModel _menuPageViewModel;

        private FolderRepository _folderRepository = new FolderRepository();

        

        

        public NavigationPage Detail { get; internal set; }


       

        public FlyoutPage1Detail(MainPageViewModel mainPageViewModel)
        {


            _notes = new ObservableCollection<AppNote>();

            InitializeComponent();
         
            _mainPageViewModel = new MainPageViewModel();
           
            BindingContext = _mainPageViewModel;
            _currentFolder = selectedFolder;
            _menuPageViewModel = new MenuPageViewModel();
            BindingContext = _menuPageViewModel;
            MessagingCenter.Subscribe<ParentFolderPopupViewModel>(this, "FolderUpdated", (sender) => _menuPageViewModel.Reset());

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ParentFolderPopupViewModel>(this, "FolderUpdated");
        }

        public FlyoutPage1Detail()
        {
            InitializeComponent();
            // Additional initialization code, if needed
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
            int folderId = GetSelectedFolderId();
            if (folderId == 0)
            {
                // No folder is selected, prompt the user
                PromptUserToSelectFolder();
                if (folderId == 0)
                {
                    // User didn't select a folder, return without navigating
                    return;
                }
            }

            // Create an instance of NewNoteViewModel with the selected folder ID
            var newNoteViewModel = new NewNoteViewModel(folderId);
            MessagingCenter.Send(this, "OpenFullScreenEditor");

            // Instead of navigating to NewNotePage, open TEditor directly
            await newNoteViewModel.OpenTEditor(); // Open TEditor for a new note
        }


        /* private async void GoToNewNotePage()
         {
             int folderId = GetSelectedFolderId();
             if (folderId == 0)
             {
                 // No folder is selected, prompt the user
                 PromptUserToSelectFolder();
                 if (folderId == 0)
                 {
                     // User didn't select a folder, return without navigating
                     return;
                 }
             }

             // Create an instance of NewNoteViewModel with the selected folder ID
           //  var newNoteViewModel = new NewNoteViewModel(folderId);
             MessagingCenter.Send(this, "OpenFullScreenEditor");

             var editorPage = new FullScreenEditorPage("<p>Initial content</p>");
             await Navigation.PushModalAsync(editorPage);
         }*/

        // Method to get the currently selected folder ID
        private int GetSelectedFolderId()
        {
            var mainPageViewModel = BindingContext as MainPageViewModel;
            if (mainPageViewModel != null && mainPageViewModel.SelectedFolder != null)
            {
                return mainPageViewModel.SelectedFolder.Id;
            }
            return 0;
        }

       
        private async void PromptUserToSelectFolder()
        {
            var userResponse = await Application.Current.MainPage.DisplayAlert(
                "Select Folder",
                "Please select a folder to continue.",
                "Go to Menu",
                "Cancel");

            if (userResponse)
            {
                OpenMenu();
            }
        }


        private void OpenMenu()
        {
            if (Application.Current.MainPage is FlyoutPage mainPage)
            {
                mainPage.IsPresented = true; // This will open the flyout menu
            }
        }

        // Event handler for a UI action, like a button click
        private void Button_Clicked(object sender, EventArgs e)
        {
            GoToNewNotePage();
        }





        /* protected override bool OnBackButtonPressed()
         {
             if (this.Detail.Navigation.NavigationStack.Count > 1)
             {
                 this.Detail.Navigation.PopAsync(); // Go back to the previous page in the stack
                 return true; // Prevent default back button behavior
             }

             return base.OnBackButtonPressed();
             *//*if (_mainPageViewModel.MultiSelectEnabled)
                 _mainPageViewModel.ShowOrHideToolbar();

             return true;*//*
         }*/
        /*protected override bool OnBackButtonPressed()
        {
            if (_mainPageViewModel.MultiSelectEnabled)
                _mainPageViewModel.ShowOrHideToolbar();
            if (this.Detail.Navigation.NavigationStack.Count > 1)
            {
                this.Detail.Navigation.PopAsync(); // Go back to the previous page in the stack
                return true; // Prevent default back button behavior
            }

            return base.OnBackButtonPressed();
            

           // return true; 
         }*/
        /* protected override bool OnBackButtonPressed()
         {
             // Check if multi-select mode is enabled
             if (_mainPageViewModel != null && _mainPageViewModel.MultiSelectEnabled)
             {
                 // Disable multi-select mode and update UI accordingly
                 _mainPageViewModel.MultiSelectEnabled = false;
                 _mainPageViewModel.ShowOrHideToolbar();

                 // Stay on the current page
                 return true; // This intercepts the back button press, preventing the default behavior
             }

             // If not in multi-select mode, proceed with the default back button behavior
             return base.OnBackButtonPressed();
         }*/
        //BACK CAUSING CRASH ON LONGPRESS!!!!!!!!!!!!!!!!!FIX!!!!!!!!!!!
        protected override bool OnBackButtonPressed()
        {
            // Check if the app is in multi-select mode
            if (_mainPageViewModel.IsInMultiSelectMode)
            {
                // Disable multi-select mode
                _mainPageViewModel.ShowOrHideToolbar();

                // Intercept the back button press to prevent app exit
                return true;
            }

            // Allow default back button behavior
            return base.OnBackButtonPressed();
        }
        /* protected override bool OnBackButtonPressed()
         {
             if (_mainPageViewModel != null && _mainPageViewModel.MultiSelectEnabled)
                 _mainPageViewModel.MultiSelectEnabled = false;
           //  _mainPageViewModel.ShowOrHideToolbar();

             return true;
         }*/

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchKeyword = e.NewTextValue;
            Debug.WriteLine($"Search keyword entered: {searchKeyword}");
            //  _mainPageViewModel.SearchNotes(searchKeyword);

        }

        
    }
}