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
using System.Runtime.CompilerServices;

namespace AllNotes.ViewModels
{

    public class MenuPageViewModel : INotifyPropertyChanged
    {

        private readonly INavigationService _navigationService;

        public MenuPageViewModel()
        {
            _navigationService = DependencyService.Get<INavigationService>();
        }
        public ICommand EditFolderCommand { get; private set; }
        public static MenuPage Instance { get; private set; }
        public ICommand RenameFolderCommand { get; private set; }

        public ICommand DeleteFolderCommand { get; private set; }
        public ICommand CancelEditFolderCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private AppDatabase _appDatabase;
        private ObservableCollection<AppFolder> _folderList;

        private AppFolder _selectedFolder;
        //  public List<AppFolder> folderList { get; set; }
        //  AppFolder _selectedfolder = null;
        int controlMenuCount = 0;
        private int folderID;
        int selectedFolderID = 0;
        // AppFolder selectedFolder;
        // AppFolder selectedfolder = null;
        public ICommand AddFolderCommand { get; private set; }
     


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
                    OnPropertyChanged(nameof(SelectedFolder));
                    NavigateToFlyoutPage1Detail(_selectedFolder);



                    // Send the message only when the selected folder changes
                    MessagingCenter.Send(this, "FolderSelected", _selectedFolder);
                }
            }
        }








        public MenuPageViewModel(Views.MenuPage menuPage)
        {
           // EditFolderCommand = new Command<AppFolder>((folder) => HandleEditFolder(folder));
            FolderList = new ObservableCollection<AppFolder>(AppDatabase.Instance().GetFolderList());
        //    DeleteFolderCommand = new Command(async () => await DeleteFolderAsync());


            selectedFolderID = folderID;
            _appDatabase = new AppDatabase(); // Initialize the repository
            FolderList = new ObservableCollection<AppFolder>();
            _navigationService = DependencyService.Get<INavigationService>();

            Reset();

            _folderList = new ObservableCollection<AppFolder>();

            InitializeViewModel();
            AddFolderCommand = new Command(async () => await AddFolderAsync());
            SelectedFolder = _selectedFolder; // Assuming 'folder' is the selected folder object


            MessagingCenter.Subscribe<NewNoteViewModel, AppNote>(this, "NoteSaved", (sender, note) =>
            {
                Reset(); // Refresh the list
            });
            //  RenameFolderCommand = new Command(async () => await RenameFolderAsync(selectedfolder));
          //  DeleteFolderCommand = new Command(async () => await DeleteFolderAsync());
            CancelCommand = new Command(CancelOperation);
        }




      

       

       
       

        /*private void RefreshFolderList()
        {
            FolderList.Clear();
            var folders = AppDatabase.Instance().GetFolderList();
            foreach (var folder in folders)
            {
                FolderList.Add(folder);
            }
        }*/
        private void RefreshFolderList()
        {
            FolderList.Clear();
            var folders = AppDatabase.Instance().GetFolderList();
            foreach (var folder in folders)
            {
                FolderList.Add(folder);
            }
        }


        private void CancelOperation()
        {
            // Logic to handle cancellation
        }
       

        private void CancelEditFolder()
        {
            // Logic to close the popup or cancel the edit operation
        }

      
        

     //   private const string DefaultFolderName = "Default Folder";


        /* public async Task Reset()
         {
             FolderList.Clear(); // Clear the observable collection

             // Get all folders from the database
             var foldersFromDb = AppDatabase.Instance().GetFolderList();

             // Define the default folder details
             string defaultFolderName = "Default Folder";
             string defaultFolderIcon = "folder_account_outline.png"; // Replace with the actual icon file name
             string editFolderName = "Edit Folder";
             string editFolderIcon = "folder_account_outline.png";

             AddSpecialFolder(defaultFolderName, "default_folder_icon.png");
             AddSpecialFolder(editFolderName, "edit_folder_icon.png");
             // Check if the "Default Folder" exists in the database
             var defaultFolder = foldersFromDb.FirstOrDefault(f => f.Name == defaultFolderName);
             if (defaultFolder == null)
             {
                 // Create and add the "Default Folder" if it doesn't exist
                 defaultFolder = new AppFolder(defaultFolderName, defaultFolderIcon, "0");
                 AppDatabase.Instance().InsertFolder(defaultFolder);
             }

             // Add the "Default Folder" to the list first
             FolderList.Add(defaultFolder);

             // Then add the rest of the folders, excluding the "Default Folder"
             foreach (var folder in foldersFromDb)
             {
                 if (folder.Name != defaultFolderName)
                 {
                     FolderList.Add(folder);
                 }
             }

             // "Edit Folder" functionality is managed separately through your UI components
         }*/
        public async Task Reset()
        {
            FolderList.Clear();
            var foldersFromDb = AppDatabase.Instance().GetFolderList();

            // Add regular folders
            foreach (var folder in foldersFromDb)
            {
                FolderList.Add(folder);
            }

            // Add "Edit Folder" as a special item
            AddSpecialFolder("Edit Folder", "edit_folder_icon.png");
        }

        /*public async Task Reset()
        {
            FolderList.Clear();

            var foldersFromDb = AppDatabase.Instance().GetFolderList();
            string defaultFolderName = "Default Folder";
            string editFolderName = "Edit Folder";

            // Add special folders
            AddSpecialFolder(defaultFolderName, "default_folder_icon.png");
            AddSpecialFolder(editFolderName, "edit_folder_icon.png");

            // Add other folders from the database
            foreach (var folder in foldersFromDb)
            {
                if (folder.Name != defaultFolderName && folder.Name != editFolderName)
                {
                    FolderList.Add(folder);
                }
            }
        }*/



        private void AddSpecialFolder(string name, string iconPath)
        {
            if (!FolderList.Any(f => f.Name == name))
            {
                FolderList.Add(new AppFolder(name, iconPath, ""));
            }
        }

        private void AddSpecialFolderIfNeeded(string name, string iconPath)
        {
            if (!FolderList.Any(f => f.Name == name))
            {
                FolderList.Add(new AppFolder(name, iconPath, ""));
            }
        }


        private void AddSpecialFolderIfMissing(string name, string iconPath)
        {
            if (!FolderList.Any(f => f.Name == name))
            {
                FolderList.Add(new AppFolder(name, iconPath, ""));
            }
        }




        private void AddSpecialFolders()
        {
           // AddSpecialFolder("Default Folder", "folder_account_outline.png", "");
            AddSpecialFolder("Edit Folder", "folder_account_outline.png", "");
        }

        private void AddSpecialFolder(string name, string iconPath, string noteCount)
        {
            if (!FolderList.Any(f => f.Name == name))
            {
                FolderList.Add(new AppFolder(name, iconPath, noteCount));
            }
        }



        private async void InitializeViewModel()
        {
            await LoadFoldersFromDatabase();
            AddSpecialFolders();
            // AddSpecialFolderIfNeeded("New Folder", "folder_plus1.png");
            // AddSpecialFolderIfNeeded("Edit Folder", "folder_account_outline.png");
        }
        private async Task LoadFoldersFromDatabase()
        {
            FolderList.Clear(); // Clear the observable collection

            var foldersFromDb = AppDatabase.Instance().GetFolderList();
            foreach (var folder in foldersFromDb)
            {
                FolderList.Add(folder); // Add folders from the database
            }
        }





        

        private async Task AddFolderAsync()
        {
            string newFolderName = await Application.Current.MainPage.DisplayPromptAsync("New Folder", "Enter folder name:");
            if (!string.IsNullOrEmpty(newFolderName))
            {
                var newFolder = new AppFolder(newFolderName, "folder_account_outline.png", "0");
                selectedFolderID = folderID;
                AppDatabase.Instance().InsertFolder(newFolder);
                await Reset();
            }
        }


        


           public ICommand FolderSelectedCommand => new Command<AppFolder>(NavigateToFlyoutPage1Detail);


        public async void NavigateToFlyoutPage1Detail(AppFolder selectedFolder)
        {
            if (selectedFolder.Name == "Edit Folder")
            {
                // Your existing logic for "Edit Folder"...
            }
            else
            {
                // If no folder is selected, use the default folder
              //  var folderToNavigate = selectedFolder ?? FolderList.FirstOrDefault(f => f.Name == DefaultFolderName);
              //  await _navigationService.NavigateToMainPage(folderToNavigate);
                await _navigationService.NavigateToMainPage(selectedFolder);
            }
        }
       


       

        //  public ICommand EditFolderCommand => new Command(ShowEditFolderPopup);
        private void ShowEditFolderPopup()
        {
            _selectedFolder = SelectedFolder;
            var editFolderPopup = new EditFolderPopup(this);
            Application.Current.MainPage.Navigation.ShowPopup(editFolderPopup);
        }
       


            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }