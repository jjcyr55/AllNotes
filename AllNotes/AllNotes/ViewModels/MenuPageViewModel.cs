using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.CommunityToolkit.UI.Views;
using System.Windows.Input;
using AllNotes.Views.NewNote;
using AllNotes.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AllNotes.Database;
using System.Threading.Tasks;
using AllNotes.Repositories;
using AllNotes.Interfaces;
using AllNotes.Views;
using System.Diagnostics;
using AllNotes.Services;
using SQLite;
using AllNotes.ViewModels;
using System.Linq;

namespace AllNotes.ViewModels
{
    public class MenuPageViewModel : INotifyPropertyChanged
    {
        public static MenuPage Instance { get; private set; }

        // private NoteRepository noteRepository;
        private INoteRepository _noteRepository;
        private ObservableCollection<AppFolder> _folderList;
        /*private readonly SQLiteAsyncConnection _database;
        private ObservableCollection<AppFolder> _folders;
        private FolderRepository folderRepository;
        private MainPageViewModel _mainPageViewModel;*/
        private AppFolder _selectedFolder;
        public List<AppFolder> folderList { get; set; }
        AppFolder selectedfolder = null;
        int controlMenuCount = 0;
        private int folderID;
        int selectedFolderID = 0;

        public ICommand AddFolderCommand { get; private set; }
        public ICommand FolderSelectedCommand => new Command<AppFolder>(NavigateToFlyoutPage1Detail);

        public ObservableCollection<AppFolder> FolderList
        {
            get => _folderList;
            set
            {
                _folderList = value;
                OnPropertyChanged(nameof(FolderList));
            }
        }


        public AppFolder SelectedFolder
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
        }








        public MenuPageViewModel(Views.MenuPage menuPage)
        {
            FolderList = new ObservableCollection<AppFolder>();
            selectedFolderID = folderID;
            _noteRepository = new NoteRepository(); // Initialize the repository
            FolderList = new ObservableCollection<AppFolder>();
            Reset();
            //  LoadFoldersAsync();
            // _folderRepository = new FolderRepository();
            _folderList = new ObservableCollection<AppFolder>();
            //  AddFolderCommand = new Command(async () => await AddFolderAsync());
            // EditFolderCommand = new Command(ShowEditFolderPopup);
            InitializeViewModel();



            AddFolderCommand = new Command(async () => await AddFolderAsync());

        }
        private async void InitializeViewModel()
        {
            await Reset();
            //   await InitializeDefaultFolder();
        }

        public MenuPageViewModel()
        {
        }


        private async Task Reset()
        {
            if (folderList != null) folderList.Clear();

            var foldersFromDb = await _noteRepository.GetFolderList();
            foreach (var folder in foldersFromDb)
            {
                FolderList.Add(folder);
            }
            FolderList.Add(new AppFolder("Default Folder", "folder_account_outline.png", ""));
            FolderList.Add(new AppFolder("Edit Folder", "folder_account_outline.png", ""));
            //  if (selectedfolder is null) selectedfolder = folderList[0];
        }
        /*private async Task Reset()
        {
            FolderList.Clear();  // Clear the observable collection

            var foldersFromDb = await _noteRepository.GetFolderList();
            foreach (var folder in foldersFromDb)
            {
                FolderList.Add(folder);  // Add folders from the database
            }

            // Add special folders only if they are not already in the list
            AddSpecialFolder("Default Folder", "folder_account_outline.png", "");
            AddSpecialFolder("Edit Folder", "folder_account_outline.png", "");
        }

        private void AddSpecialFolder(string name, string iconPath, string noteCount)
        {
            if (!FolderList.Any(f => f.Name == name))
            {
                FolderList.Add(new AppFolder(name, iconPath, noteCount));
            }
        }*/

        /*private async Task InitializeDefaultFolder()
        {
            var firstFolder = await _folderRepository.GetFirstFolder();
            if (firstFolder == null)
            {
                var defaultFolder = new AppFolder { Name = "Default Folder", IconPath = "folder_account_outline.png" };
                await _folderRepository.InsertFolder(defaultFolder);
            }
        }*/

        private async Task AddFolderAsync()
        {
            string newFolderName = await Application.Current.MainPage.DisplayPromptAsync("New Folder", "Enter folder name:");
            if (!string.IsNullOrEmpty(newFolderName))
            {
                var newFolder = new AppFolder(newFolderName, "folder_account_outline.png", "0");
                selectedFolderID = folderID;
                await _noteRepository.InsertFolder(newFolder);
                await Reset();
            }
        }



        private async void NavigateToFlyoutPage1Detail(AppFolder selectedFolder)
        {
            if (selectedFolder.Name == "Edit Folder")
            {
                string action = await Application.Current.MainPage.DisplayActionSheet("Edit Folder", "Cancel", null, "Rename", "Delete");

                switch (action)
                {
                    case "Rename":
                        // Implement rename logic
                        string newName = await Application.Current.MainPage.DisplayPromptAsync("Rename Folder", "Enter new folder name:", initialValue: SelectedFolder.Name);
                        if (!string.IsNullOrWhiteSpace(newName))
                        {
                            SelectedFolder.Name = newName;
                            await _noteRepository.UpdateFolder(SelectedFolder);
                            await Reset();
                        }
                        break;
                    case "Delete":
                        // Implement delete logic
                        bool confirm = await Application.Current.MainPage.DisplayAlert("Delete Folder", "Are you sure you want to delete this folder?", "Yes", "No");
                        if (confirm)
                        {
                            await _noteRepository.DeleteFolder(SelectedFolder);
                            await Reset();
                        }
                        break;
                    case "Cancel":
                        // Handle cancel action
                        break;
                }
            }
            else
            {

                MainPageViewModel mainPageViewModel =
        Application.Current.MainPage.BindingContext as MainPageViewModel;
                if (mainPageViewModel != null)
                {
                    mainPageViewModel.SelectedFolder = selectedFolder;
                    await mainPageViewModel.Reset(selectedFolder);
                    //TRYING TO REDUCE THE BELOW CODE TO SEE IF ITS CREATING FALSE NAVIGATION. NEEDS REFACTORING IMMEDIATLY OR DELETION
                    //LOOK AT FASTNOTES MENU FOR REFERNCE
                    var flyoutPage = (FlyoutPage)Application.Current.MainPage;
                    flyoutPage.IsPresented = false;


                }

                var flyoutPage1Detail = new FlyoutPage1Detail(selectedFolder);

                if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
                {
                    mainFlyoutPage.Detail = new NavigationPage(flyoutPage1Detail);
                    mainFlyoutPage.IsPresented = false;
                }
                else
                {
                    Application.Current.MainPage = new FlyoutPage
                    {
                        Detail = new NavigationPage(flyoutPage1Detail),
                        Flyout = new MenuPage()
                    };
                }
            }
        }


        /* private async void NavigateToFlyoutPage1Detail(AppFolder selectedFolder)
         {
             if (selectedFolder != null)
             {
                 MainPageViewModel mainPageViewModel = Application.Current.MainPage.BindingContext as MainPageViewModel;
                 if (mainPageViewModel != null)
                 {
                     // Update the selected folder in MainPageViewModel
                     mainPageViewModel.SelectedFolder = selectedFolder;

                     // Reset notes based on the selected folder
                     await mainPageViewModel.Reset(selectedFolder);
                 }

                 else
                 {



                 }

                     var flyoutPage1Detail = new FlyoutPage1Detail(selectedFolder);

                     if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
                     {
                         mainFlyoutPage.Detail = new NavigationPage(flyoutPage1Detail);
                         mainFlyoutPage.IsPresented = false;
                     }
                     else
                     {
                         Application.Current.MainPage = new FlyoutPage
                         {
                             Detail = new NavigationPage(flyoutPage1Detail),
                             Flyout = new MenuPage()
                         };
                     }


             }
         }*/


        private void ShowEditFolderPopup()
        {

            var editFolderPopup = new EditFolderPopup();

            // Show the EditFolderPopup
            Application.Current.MainPage.Navigation.ShowPopup(editFolderPopup);
        }


        public ICommand EditFolderCommand => new Command(ShowEditFolderPopup);




        public async Task EditFolderAsync()
        {
            // Implementation of editing a folder
            // Ensure this method is appropriate for asynchronous operations
            if (SelectedFolder != null)
            {
                // Logic to edit the selected folder
            }
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
