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

        public static MenuPage Instance { get; private set; }


            private AppDatabase _appDatabase;
            private ObservableCollection<AppFolder> _folderList;

            private AppFolder _selectedFolder;
            public List<AppFolder> folderList { get; set; }
            AppFolder _selectedfolder = null;
            int controlMenuCount = 0;
            private int folderID;
            int selectedFolderID = 0;
        AppFolder selectedFolder;
        AppFolder selectedfolder = null;
        public ICommand AddFolderCommand { get; private set; }
           // public ICommand FolderSelectedCommand => new Command<AppFolder>(NavigateToFlyoutPage1Detail);

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

                selectedFolderID = folderID;
                _appDatabase = new AppDatabase(); // Initialize the repository
                FolderList = new ObservableCollection<AppFolder>();
            _navigationService = DependencyService.Get<INavigationService>();

            Reset();
                //  LoadFoldersAsync();
                // _folderRepository = new FolderRepository();
                _folderList = new ObservableCollection<AppFolder>();
            //  AddFolderCommand = new Command(async () => await AddFolderAsync());
            // EditFolderCommand = new Command(ShowEditFolderPopup);
            // InitializeViewModel();
            InitializeViewModel();
            AddFolderCommand = new Command(async () => await AddFolderAsync());
            SelectedFolder = _selectedFolder; // Assuming 'folder' is the selected folder object

            // Send a message with the selected folder
            // MessagingCenter.Send(this, "FolderSelected", SelectedFolder);
            MessagingCenter.Subscribe<NewNoteViewModel, AppNote>(this, "NoteSaved", (sender, note) =>
            {
                Reset(); // Refresh the list
            });

        }



        /*public async Task Reset()
        {
           // if (folderList != null) //NOTICE THIS LINE IS BUGGY AND DOUBLES FOLDERS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                FolderList.Clear();  // Clear the observable collection

            var foldersFromDb = AppDatabase.Instance().GetFolderList();
            foreach (var folder in foldersFromDb)
            {
                FolderList.Add(folder);  // Add folders from the database
            }

            // Add special folders only if they are not already in the list
            AddSpecialFolder("Default Folder", "folder_account_outline.png", "");
            AddSpecialFolder("Edit Folder", "folder_account_outline.png", "");

            //if (selectedfolder is null) selectedfolder = folderList[0];
        }*/

        private const string DefaultFolderName = "Default Folder";

        /* public async Task Reset()
         {
             FolderList.Clear();
             var foldersFromDb = AppDatabase.Instance().GetFolderList();

             // Add Default Folder at the beginning
             var defaultFolder = foldersFromDb.FirstOrDefault(f => f.Name == DefaultFolderName);
             if (defaultFolder != null)
             {
                 FolderList.Add(defaultFolder);
                 foldersFromDb.Remove(defaultFolder);
             }

             foreach (var folder in foldersFromDb)
             {
                 FolderList.Add(folder);
             }

             AddSpecialFolder("Edit Folder", "folder_account_outline.png", "");
         }*/
        /*public async Task Reset()
        {
            FolderList.Clear(); // Clear the observable collection

            // Get all folders from the database
            var foldersFromDb = AppDatabase.Instance().GetFolderList();

            // Prioritize adding the "Default Folder" and "Edit Folder" first
            var defaultFolder = foldersFromDb.FirstOrDefault(f => f.Name == "Default Folder");
            var editFolder = foldersFromDb.FirstOrDefault(f => f.Name == "Edit Folder");

            // Ensure "Default Folder" is always first, even if retrieved after "Edit Folder"
            if (defaultFolder != null)
            {
                defaultFolder.IconPath = "folder_account_outline.png";
                FolderList.Insert(0, defaultFolder); // Insert at index 0 for guaranteed top position
                foldersFromDb.Remove(defaultFolder); // Remove it from the subsequent loop
            }

            if (editFolder != null)
            {
                FolderList.Add(editFolder); // Add "Edit Folder" after "Default Folder"
                foldersFromDb.Remove(editFolder); // Remove it from the subsequent loop
            }

            // Add the remaining folders from the database
            foreach (var folder in foldersFromDb)
            {
                FolderList.Add(folder);
            }

            // Add any remaining special folders that weren't already added
         //   AddSpecialFolderIfMissing("New Folder", "folder_plus1.png");

            // ... rest of the code remains unchanged
        }*/


        /*public async Task Reset()
        {
            FolderList.Clear(); // Clear the observable collection

            // Get all folders from the database
            var foldersFromDb = AppDatabase.Instance().GetFolderList();

            // Prioritize adding the "Default Folder" and "Edit Folder" first, with icons
            var defaultFolder = foldersFromDb.FirstOrDefault(f => f.Name == "Default Folder");
            var editFolder = foldersFromDb.FirstOrDefault(f => f.Name == "Edit Folder");

            // Ensure "Default Folder" is always first, with the desired icon
            if (defaultFolder != null)
            {
                // Set the icon for "Default Folder"
                defaultFolder.IconPath = "folder_account_outline.png"; // Replace with the correct icon path

                FolderList.Insert(0, defaultFolder); // Insert at index 0 for guaranteed top position
                foldersFromDb.Remove(defaultFolder); // Remove it from the subsequent loop
            }

            if (editFolder != null)
            {
                FolderList.Add(editFolder); // Add "Edit Folder" after "Default Folder"
                foldersFromDb.Remove(editFolder); // Remove it from the subsequent loop
            }

            // Add the remaining folders from the database
            foreach (var folder in foldersFromDb)
            {
                FolderList.Add(folder);
            }

            // Remove the "New Folder" special folder
            //  FolderList.RemoveAll(f => f.Name == "New Folder");

            // ... rest of the code remains unchanged
        }
*/
        /*public async Task Reset()
        {
            FolderList.Clear(); // Clear the observable collection

            // Get all folders from the database
            var foldersFromDb = AppDatabase.Instance().GetFolderList();

            // Prioritize adding the "Default Folder" and "Edit Folder" first, with icons
            var defaultFolder = foldersFromDb.FirstOrDefault(f => f.Name == "Default Folder");
            var editFolder = foldersFromDb.FirstOrDefault(f => f.Name == "Edit Folder");

            // Ensure "Default Folder" is always first, with the desired icon
            if (defaultFolder != null)
            {
                // Set the icon for "Default Folder"
                defaultFolder.IconPath = "folder_account_outline.png"; // Replace with the correct icon path

                FolderList.Insert(0, defaultFolder); // Insert at index 0 for guaranteed top position
                foldersFromDb.Remove(defaultFolder); // Remove it from the subsequent loop
            }

            if (editFolder != null)
            {
                FolderList.Add(editFolder); // Add "Edit Folder" after "Default Folder"
                foldersFromDb.Remove(editFolder); // Remove it from the subsequent loop
            }

            // Add the remaining folders from the database
            foreach (var folder in foldersFromDb)
            {
                FolderList.Add(folder);
            }

            // Remove the "New Folder" special folder
          //  FolderList.RemoveAll(f => f.Name == "New Folder");

            // ... rest of the code remains unchanged
        }*/
        /*public async Task Reset()
        {
            FolderList.Clear(); // Clear the observable collection

            // Get all folders from the database
            var foldersFromDb = AppDatabase.Instance().GetFolderList();

            // Prioritize adding the "Default Folder" and "Edit Folder" first, with icons
            var defaultFolder = foldersFromDb.FirstOrDefault(f => f.Name == "Default Folder");
            var editFolder = foldersFromDb.FirstOrDefault(f => f.Name == "Edit Folder");

            // Ensure "Default Folder" is always first, with the desired icon
            if (defaultFolder != null)
            {
                // Set the icon for "Default Folder"
                defaultFolder.IconPath = "path/to/folder_account_outline.png"; // Replace with the correct icon path
                FolderList.Insert(0, defaultFolder); // Insert at index 0 for guaranteed top position
            }

            // Add "Edit Folder" after "Default Folder"
            if (editFolder != null)
            {
                FolderList.Add(editFolder);
            }

            // Add the remaining folders from the database
            foreach (var folder in foldersFromDb)
            {
                // Avoid duplicates of "Default Folder" and "Edit Folder"
                if (folder.Name != "Default Folder" && folder.Name != "Edit Folder")
                {
                    FolderList.Add(folder);
                }
            }

            // ... rest of the code remains unchanged
        }*/
        /*public async Task Reset()
        {
            FolderList.Clear(); // Clear the observable collection

            // Get all folders from the database
            var foldersFromDb = AppDatabase.Instance().GetFolderList();

            // First, add the "Default Folder" if it exists
            var defaultFolder = foldersFromDb.FirstOrDefault(f => f.Name == "Default Folder");
            if (defaultFolder != null)
            {
                FolderList.Add(defaultFolder);
            }

            // Then add the rest of the folders, excluding the "Default Folder"
            foreach (var folder in foldersFromDb)
            {
                if (folder.Name != "Default Folder")
                {
                    FolderList.Add(folder);
                }
            }

            // Add special folders if needed, like "New Folder", "Edit Folder"
            // Ensure they are not already present in the list
            AddSpecialFolderIfNeeded("New Folder", "folder_plus1.png");
            AddSpecialFolderIfNeeded("Edit Folder", "folder_account_outline.png");
        }*/
        public async Task Reset()
        {
            FolderList.Clear(); // Clear the observable collection

            // Get all folders from the database
            var foldersFromDb = AppDatabase.Instance().GetFolderList();

            // Define the default folder details
            string defaultFolderName = "Default Folder";
            string defaultFolderIcon = "default_folder_icon.png"; // Replace with the actual icon file name

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
            AddSpecialFolder("Default Folder", "folder_account_outline.png", "");
            //AddSpecialFolder("Edit Folder", "folder_account_outline.png", "");
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

        



           /* private async Task Reset()
            {
            await LoadFoldersFromDatabase();
            AddSpecialFolders();
            FolderList.Clear(); // Clear the observable collection

                var foldersFromDb = AppDatabase.Instance().GetFolderList();
                foreach (var folder in foldersFromDb)
                {
                    FolderList.Add(folder); // Add folders from the database
                }
            }

       
        private void AddSpecialFolders()
            {
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


        /* public ICommand FolderSelectedCommand => new Command<AppFolder>(NavigateToFlyoutPage1Detail);
         public async void NavigateToFlyoutPage1Detail(AppFolder selectedFolder)
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
                                 AppDatabase.Instance().UpdateFolder(SelectedFolder);
                                 await Reset();
                             }
                             break;
                         case "Delete":
                             // Implement delete logic
                             bool confirm = await Application.Current.MainPage.DisplayAlert("Delete Folder", "Are you sure you want to delete this folder?", "Yes", "No");
                             if (confirm)
                             {
                                 AppDatabase.Instance().DeleteFolder(SelectedFolder);
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




                     *//* var flyoutPage = (FlyoutPage)Application.Current.MainPage;
                      flyoutPage.IsPresented = false;*//*

                     await _navigationService.NavigateToMainPage(selectedFolder);
                 }

                    *//* var flyoutPage1Detail = new FlyoutPage1Detail(selectedFolder);

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
                     }*//*
                 }
             }*/
        /*private async void NavigateToFlyoutPage1Detail(AppFolder _selectedFolder)
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
        }*/
        public ICommand FolderSelectedCommand => new Command<AppFolder>(NavigateToFlyoutPage1Detail);

        /* public async void NavigateToFlyoutPage1Detail(AppFolder selectedFolder)
         {
             if (selectedFolder.Name == "Edit Folder")
             {
                 // Your existing logic for "Edit Folder"...
             }
             else
             {
                 // Use the navigation service for other folders
                 await _navigationService.NavigateToMainPage(selectedFolder);
             }
         }*/
         public async void NavigateToFlyoutPage1Detail(AppFolder selectedFolder)
         {
             if (selectedFolder.Name == "Edit Folder")
             {
                 // Your existing logic for "Edit Folder"...
             }
             else
             {
                // If no folder is selected, use the default folder
                var folderToNavigate = selectedFolder ?? FolderList.FirstOrDefault(f => f.Name == DefaultFolderName);
                await _navigationService.NavigateToMainPage(folderToNavigate);
            }
         }





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