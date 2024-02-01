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
using Syncfusion.XForms.Buttons;
using CheckedChangedEventArgs = Xamarin.Forms.CheckedChangedEventArgs;

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
            MessagingCenter.Subscribe<AppNote>(this, "RefreshNotes", (sender) =>
            {
                _mainPageViewModel.RefreshNotes();
            });

        }



        /*protected override bool OnBackButtonPressed()
        {
            if (_mainPageViewModel.MultiSelectEnabled)
                _mainPageViewModel.ShowOrHideToolbar();

            return true;
        }*/
        /*protected override bool OnBackButtonPressed()
        {
            if (_mainPageViewModel != null && _mainPageViewModel.IsEditMode)
            {
                _mainPageViewModel.ExitEditMode();
                return true; // Handled the back button press
            }
            else
            {
                return false; // Not handled here, continue with default behavior
            }
        }*/
        



        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ParentFolderPopupViewModel>(this, "FolderUpdated");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var viewModel = this.BindingContext as MainPageViewModel;
            viewModel?.RefreshNotes();
            viewModel?.InitializeNoteCount();
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



        /*private void SelectAllCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // Assuming you have a ViewModel instance named ViewModel
            if (e.Value)
            {
                _mainPageViewModel.SelectAllNotes();
            }
            else
            {
                _mainPageViewModel.DeselectAllNotes();
            }
        }*/


        /*private void IndividualCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var note = checkBox?.BindingContext as AppNote;
            if (note != null)
            {
                _mainPageViewModel.ToggleNoteSelection(note);
            }
        }*/

        /*private void IndividualCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null)
            {
                Debug.WriteLine("Sender is not a CheckBox");
                return;
            }
        }*/
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

            // Pass the ViewModel to NewNotePage
            var newNotePage = new NewNotePage(newNoteViewModel);

            // Use Xamarin.Forms navigation to push the new page
            await Navigation.PushAsync(newNotePage);
        }

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


        private void OnSelectAllTapped(object sender, EventArgs e)
        {
            var viewModel = this.BindingContext as MainPageViewModel;
            if (viewModel != null)
            {
                viewModel.IsAllChecked = !viewModel.IsAllChecked;
            }
        }

        /*private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)

        {
            var viewModel = this.BindingContext as MainPageViewModel;
            if (sender is CheckBox checkBox && checkBox.BindingContext is AppNote note)
            {
                viewModel.ToggleNoteSelection(note);
            }
        }*/





        /*protected override bool OnBackButtonPressed()
        {
            if (_mainPageViewModel.MultiSelectEnabled)
              //  _mainPageViewModel.ShowOrHideToolbar();

            return true;
        }*/

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchKeyword = e.NewTextValue;
            Debug.WriteLine($"Search keyword entered: {searchKeyword}");
            //  _mainPageViewModel.SearchNotes(searchKeyword);

        }

        private void SelectAllCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Checkbox CheckedChanged triggered");

            if (BindingContext is MainPageViewModel viewModel)
            {
                viewModel.IsAllChecked = e.Value; // e.Value gives the boolean state of the checkbox
            }
        }
    }
}
