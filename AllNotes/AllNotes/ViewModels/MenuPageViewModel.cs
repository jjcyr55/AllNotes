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
        public ICommand NavigateToManageFoldersCommand { get; private set; }

        public ICommand EditFolderCommand { get; private set; }
        public static MenuPage Instance { get; private set; }
        public ICommand RenameFolderCommand { get; private set; }

       // public ICommand DeleteFolderCommand { get; private set; }
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
        //  private AppFolder _selectedFolder;

        public ICommand AddSecureFolderCommand => new Command(ShowAddSecureFolderPopup);

        private void ShowAddSecureFolderPopup()
        {

            var secureFolderPopup = new SecureFolderPopup(this);
            Application.Current.MainPage.Navigation.ShowPopup(secureFolderPopup);

           
        }


        public MenuPageViewModel(Views.MenuPage menuPage)
        {
           // EditFolderCommand = new Command<AppFolder>((folder) => HandleEditFolder(folder));
            FolderList = new ObservableCollection<AppFolder>(AppDatabase.Instance().GetFolderList());
            //    DeleteFolderCommand = new Command(async () => await DeleteFolderAsync());
            // NavigateToManageFoldersCommand = new Command(NavigateToManageFolders);
            NavigateToManageFoldersCommand = new Command(async () => await NavigateToManageFolders());

            selectedFolderID = folderID;
            _appDatabase = new AppDatabase(); // Initialize the repository
            FolderList = new ObservableCollection<AppFolder>();
            _navigationService = DependencyService.Get<INavigationService>();

            Reset();
            MessagingCenter.Subscribe<ManageFoldersViewModel>(this, "FoldersUpdated", (sender) =>
            {
                RefreshFolderList();
            });
            _folderList = new ObservableCollection<AppFolder>();

            InitializeViewModel();
            AddFolderCommand = new Command(async () => await AddFolderAsync());
            SelectedFolder = _selectedFolder; // Assuming 'folder' is the selected folder object

            MessagingCenter.Subscribe<ManageFoldersViewModel>(this, "FoldersUpdated", (sender) => {
                RefreshFolderList();
            });
            MessagingCenter.Subscribe<NewNoteViewModel, AppNote>(this, "NoteSaved", (sender, note) =>
            {
                Reset(); // Refresh the list
            });
            //  RenameFolderCommand = new Command(async () => await RenameFolderAsync(selectedfolder));
           // DeleteFolderCommand = new Command<AppFolder>(async (folder) => await DeleteFolderAsync(folder));

            CancelCommand = new Command(CancelOperation);
        }
        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                PerformSearch(); // Call the search method when the query changes
            }
        }
        public void PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                // If the search query is empty, show all notes
                RefreshFolderList();
            }
            else
            {
                // Filter the notes based on the query
                var filteredFolders = AppDatabase.Instance().GetFolderList()
    .Where(folder => folder.Name.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   folder.Name.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
    .ToList();

                // Update the Notes collection with the search results
                FolderList.Clear();
                foreach (var folder in filteredFolders)
                {
                    FolderList.Add(folder);
                }
            }
        }



        public void CreateSecureFolder(string folderName, string password)
        {
            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(password))
            {
                // Handle validation error
                return;
            }

            var encryptedPassword = EncryptPassword(password);

            var iconPath = "folder_locker.png";

            var newFolder = new AppFolder
            {
                Name = folderName,
                IsSecure = true,
                EncryptedPassword = encryptedPassword,
                IconPath = iconPath,
                // other properties...
            };

            AppDatabase.Instance().InsertFolder(newFolder);
            RefreshFolderList(); // Update your folder list
        }
       
        private string EncryptPassword(string password)
        {
            // This is a placeholder for actual encryption logic
            // For demonstration purposes only
            // In production, use a strong encryption method
            byte[] data = System.Text.Encoding.UTF8.GetBytes(password);
            string encryptedPassword = Convert.ToBase64String(data);
            return encryptedPassword;
        }







        private async Task NavigateToManageFolders()
        {
            // Create an instance of ManageFoldersViewModel
            var manageFoldersViewModel = new ManageFoldersViewModel();

            // Create a new instance of ManageFolders page
            var manageFoldersPage = new ManageFolders(manageFoldersViewModel);

            // Navigate using Xamarin.Forms built-in navigation
            if (Application.Current.MainPage is NavigationPage navigationPage)
            {
                await navigationPage.PushAsync(manageFoldersPage);
            }
            else if (Application.Current.MainPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage)
            {
                await ((NavigationPage)flyoutPage.Detail).PushAsync(manageFoldersPage);
                flyoutPage.IsPresented = false;
            }
            else
            {
                // Handle other cases or throw an exception
            }
        }


        private bool _isDeleteModeEnabled;
        public bool IsDeleteModeEnabled
        {
            get => _isDeleteModeEnabled;
            set
            {
                _isDeleteModeEnabled = value;
                // Update UI elements (buttons, selection visuals) based on the mode
            }
        }
      
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
         //   AddSpecialFolder("Edit Folder", "edit_folder_icon.png");
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



       



        private async void InitializeViewModel()
        {
            await LoadFoldersFromDatabase();
         //   AddSpecialFolders();
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

        private bool _isSecure;

        public bool IsSecure
        {
            get => _isSecure;
            set
            {
                if (_isSecure != value)
                {
                    _isSecure = value;
                    OnPropertyChanged(nameof(IsSecure));
                    OnPropertyChanged(nameof(LockIconVisible));
                }
            }
        }
        public bool LockIconVisible => IsSecure;


        public ICommand FolderSelectedCommand => new Command<AppFolder>(NavigateToFlyoutPage1Detail);


        /* public async void NavigateToFlyoutPage1Detail(AppFolder selectedFolder)
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
         }*/
        //GET MORE ROBUST PASSWORD ENCRYPTION AND POSSIBLY SET UP USERNAME AND PASSWORD RETRIEVE METHODS. ALSO CREATE METHODS TO DENY FOLDER DELETION ON SECURE FOLDER WITHOUT PASSWORD. ALSO SHOW LOCK FOLDER ICON FOR SECURE FOLDERS
        public async void NavigateToFlyoutPage1Detail(AppFolder selectedFolder)
        {
            if (selectedFolder.Name == "Edit Folder")
            {
                // Your existing logic for "Edit Folder"...
            }
            else
            {
                // Check if the folder is secure
                if (selectedFolder.IsSecure)
                {
                    // Prompt for password
                    string inputPassword = await Application.Current.MainPage.DisplayPromptAsync("Secure Folder", "Enter password:", "Ok", "Cancel", "Password", maxLength: 20, keyboard: Keyboard.Text);

                    if (inputPassword != null && ValidatePassword(inputPassword, selectedFolder.EncryptedPassword))
                    {
                        // Password is correct, proceed to open the folder
                        await OpenFolder(selectedFolder);
                    }
                    else
                    {
                        // Handle incorrect password
                        await Application.Current.MainPage.DisplayAlert("Error", "Incorrect Password", "OK");
                    }
                }
                else
                {
                    // For non-secure folders, just open them as usual
                    await OpenFolder(selectedFolder);
                }
            }
        }

        private bool ValidatePassword(string inputPassword, string encryptedPassword)
        {
            // Decrypt the encryptedPassword and compare with inputPassword
            // For demonstration, assuming simple base64 decryption
            try
            {
                string decryptedPassword = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedPassword));
                return decryptedPassword == inputPassword;
            }
            catch
            {
                return false;
            }
        }

        private async Task OpenFolder(AppFolder folder)
        {
            // Your existing logic to open the folder
            // Replace with your actual method to open the folder
            await _navigationService.NavigateToMainPage(folder);
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