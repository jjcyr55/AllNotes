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
using static AllNotes.ViewModels.ManageFoldersViewModel;
using AllNotes.Models;



namespace AllNotes.ViewModels
{

    public class MenuPageViewModel : INotifyPropertyChanged
    {

        private readonly INavigationService _navigationService;
        public ObservableCollection<AppFolder> DisplayFolders { get; set; }
        public MenuPageViewModel(FolderService instance)
        {
            _navigationService = DependencyService.Get<INavigationService>();
        }
        public ICommand NavigateToManageFoldersCommand { get; private set; }

        public ICommand EditFolderCommand { get; private set; }
        public static MenuPage Instance { get; private set; }
        public ICommand RenameFolderCommand { get; private set; }


        public ICommand CancelEditFolderCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private AppDatabase _appDatabase;
        private ObservableCollection<AppFolder> _folderList;

        private AppFolder _selectedFolder;

        int controlMenuCount = 0;
        private int folderID;
        int selectedFolderID = 0;
        public ObservableCollection<AppFolder> Folders { get; set; }
        public ICommand AddFolderCommand { get; private set; }
        private readonly FolderService _folderService;


        public ObservableCollection<AppFolder> FolderList
        {
            get => _folderList;
            set
            {
                _folderList = value;
                OnPropertyChanged(nameof(FolderList));
            }
        }


        /*public AppFolder SelectedFolder
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
        }*/
        public AppFolder SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                if (_selectedFolder != value)
                {
                    _selectedFolder = value;
                    OnPropertyChanged(nameof(SelectedFolder));

                    // Additional logic when a new folder is selected
                    if (_selectedFolder != null)
                    {
                        NavigateToFlyoutPage1Detail(_selectedFolder);
                    }
                }
            }
        }
        public ICommand SelectParentFolderCommand { get; }
        public ICommand SelectSubfolderCommand { get; }

        public ICommand AddSecureFolderCommand => new Command(ShowAddSecureFolderPopup);

        private void ShowAddSecureFolderPopup()
        {

            var secureFolderPopup = new SecureFolderPopup(this);
            Application.Current.MainPage.Navigation.ShowPopup(secureFolderPopup);


        }
        public ICommand ToggleFolderCommand { get; private set; }

        public MenuPageViewModel(Views.MenuPage menuPage)
        {
            MessagingCenter.Subscribe<MoveNotePopupViewModel, int>(this, "NotesMoved", (sender, folderId) =>
            {
                // Call method to update the UI based on the moved note
                RefreshFolderList(); // Or any other method that updates the folder list
            });
            MessagingCenter.Subscribe<MoveNotePopupViewModel, int>(this, "NotesMoved", (sender, folderId) =>
            {
                // Call method to update the UI based on the moved note
                Reset(); // Or any other method that updates the folder list
            });

            MessagingCenter.Subscribe<ParentFolderPopupViewModel, AppFolder>(this, "FolderUpdated", (sender, updatedFolder) =>
            {
                // Update logic for MenuPage's folder list
                // This might involve refreshing the folder list or updating a specific folder

                // For example:
                //  RefreshFolderList();
                Reset();
            });

            SelectSubfolderCommand = new Command<AppFolder>(SelectSubfolder);
            MessagingCenter.Subscribe<NewNoteViewModel, int>(this, "NoteUpdated", (sender, folderId) =>
            {
                UpdateNoteCountForFolder(folderId);
            });

            MessagingCenter.Subscribe<MainPageViewModel, int>(this, "NoteUpdated", (sender, folderId) =>
            {
                UpdateNoteCountForFolder(folderId);
            });

            MessagingCenter.Subscribe<ManageFoldersViewModel, AppFolder>(this, "SubfolderAdded", (sender, subfolder) =>
            {
                var parentFolder = FolderList.FirstOrDefault(f => f.Id == subfolder.ParentFolderId);
                if (parentFolder != null)
                {
                    parentFolder.Subfolders.Add(subfolder);

                    // Temporarily remove and re-add the parent folder
                    FolderList.Remove(parentFolder);
                    FolderList.Add(parentFolder);

                    // Notify changes
                    OnPropertyChanged(nameof(FolderList));
                }
            });

            


            MessagingCenter.Subscribe<ManageFoldersViewModel>(this, "FolderListUpdated", (sender) =>
            {
                RefreshFolderList();
            });
           
            MessagingCenter.Subscribe<ManageFoldersViewModel>(this, "FolderRenamed", (sender) =>
            {
                //  RefreshFolderList();
                Reset();
              // Method to update the folder list in MenuPageViewModel
            });

            MessagingCenter.Subscribe<ManageFoldersViewModel>(this, "FoldersUpdated", (sender) =>
            {
                RefreshFolderList();
                Reset();
            });

            ToggleFolderCommand = new Command<AppFolder>(ToggleFolder);


            MessagingCenter.Subscribe<ManageFoldersViewModel, AppFolder>(this, "FolderUpdated", (sender, updatedFolder) =>
            {
                UpdateFolderList(updatedFolder);
            });
            MessagingCenter.Subscribe<ManageFoldersViewModel, AppFolder>(this, "Reset", (sender, updatedFolder) =>
            {
                UpdateFolderList(updatedFolder);
            });

            MessagingCenter.Subscribe<NewNoteViewModel, int>(this, "NoteUpdated", (sender, folderId) =>
            {
                UpdateNoteCountForFolder(folderId);
            });
            MessagingCenter.Subscribe<NewNoteViewModel, int>(this, "Reset", (sender, folderId) =>
            {
                UpdateNoteCountForFolder(folderId);
            });
            MessagingCenter.Subscribe<MainPageViewModel, int>(this, "NoteUpdated", (sender, folderId) =>
            {
                UpdateNoteCountForFolder(folderId);
            });

            FolderList = new ObservableCollection<AppFolder>(AppDatabase.Instance().GetFolderList());

            NavigateToManageFoldersCommand = new Command(async () => await NavigateToManageFolders());

            selectedFolderID = folderID;
            _appDatabase = new AppDatabase(); // Initialize the repository
            FolderList = new ObservableCollection<AppFolder>();
            _navigationService = DependencyService.Get<INavigationService>();

            Reset();
           /* MessagingCenter.Subscribe<ManageFoldersViewModel>(this, "FoldersUpdated", (sender) =>
            {
                RefreshFolderList();
            });*/
            _folderList = new ObservableCollection<AppFolder>();

            InitializeViewModel();
            AddFolderCommand = new Command(async () => await AddFolderAsync());
            SelectedFolder = _selectedFolder; // Assuming 'folder' is the selected folder object

            MessagingCenter.Subscribe<ManageFoldersViewModel>(this, "FoldersUpdated", (sender) =>
            {
                RefreshFolderList();
            });
            MessagingCenter.Subscribe<NewNoteViewModel, AppNote>(this, "NoteSaved", (sender, note) =>
            {
                Reset(); // Refresh the list
            });

            MessagingCenter.Subscribe<MainPageViewModel, int>(this, "NoteUpdated", (sender, folderId) =>
            {
                UpdateNoteCountForFolder(folderId);
            });

        }
        private void HandleRenamedFolder(AppFolder updatedFolder)
        {
            // Find the folder in your FolderList and update it
            var folderToUpdate = FolderList.FirstOrDefault(f => f.Id == updatedFolder.Id);
            if (folderToUpdate != null)
            {
                folderToUpdate.Name = updatedFolder.Name;
                // Update any other properties if necessary
               
                // Refresh the UI or list
                OnPropertyChanged(nameof(FolderList));
            }
        }
        public MenuPageViewModel()
        {
        }

        /*public void UpdateNoteCountForFolder(int folderId)
        {
            Debug.WriteLine($"Updating note count for folder {folderId}");
            var folder = FolderList.FirstOrDefault(f => f.Id == folderId);
            if (folder != null)
            {
                folder.NoteCount = AppDatabase.Instance().GetNoteList(folderId).Count;
                OnPropertyChanged(nameof(FolderList));
                Reset();
            }
        }*/

        /*public void UpdateNoteCountForFolder(int folderId)
        {
            var folder = FolderList.FirstOrDefault(f => f.Id == folderId);
            if (folder != null)
            {
                folder.NoteCount = AppDatabase.Instance().GetNoteList(folderId).Count;
                foreach (var subfolder in folder.Subfolders)
                {
                    subfolder.NoteCountForSubfolders = AppDatabase.Instance().GetNoteList(subfolder.Id).Count;
                }
                OnPropertyChanged(nameof(FolderList));
                Reset();
            }
        }*/


        public void UpdateNoteCountForFolder(int folderId)
        {
            var folderToUpdate = FolderList.FirstOrDefault(f => f.Id == folderId);
            if (folderToUpdate != null)
            {
                folderToUpdate.NoteCount = AppDatabase.Instance().GetNoteList(folderId).Count;

                foreach (var subfolder in folderToUpdate.Subfolders)
                {
                    subfolder.NoteCountForSubfolders = AppDatabase.Instance().GetNoteList(subfolder.Id).Count;
                }

                OnPropertyChanged(nameof(FolderList));
                Reset();
            }

            // If the updated folder is a subfolder, find its parent and update accordingly.
            foreach (var folder in FolderList)
            {
                var subfolderToUpdate = folder.Subfolders.FirstOrDefault(sf => sf.Id == folderId);
                if (subfolderToUpdate != null)
                {
                    subfolderToUpdate.NoteCountForSubfolders = AppDatabase.Instance().GetNoteList(folderId).Count;
                    OnPropertyChanged(nameof(FolderList));
                    Reset();
                    break; // Exit the loop once the subfolder is found and updated.
                }
            }
        }

        private void HandleSubfolderAdded(AppFolder parentFolder, AppFolder newSubfolder)
        {
            var parentInList = FolderList.FirstOrDefault(f => f.Id == parentFolder.Id);
            if (parentInList != null)
            {
                if (parentInList.Subfolders == null)
                    parentInList.Subfolders = new ObservableCollection<AppFolder>();

                if (!parentInList.Subfolders.Any(sf => sf.Id == newSubfolder.Id))
                {
                    parentInList.Subfolders.Add(newSubfolder);
                    OnPropertyChanged(nameof(FolderList)); // Notify that FolderList has changed
                }
            }

            // Optionally, refresh the list UI
            RefreshUI();
        }

        private void RefreshUI()
        {
            var tempFolderList = FolderList.ToList();
            FolderList = null;
            FolderList = new ObservableCollection<AppFolder>(tempFolderList);
        }

        private void UpdateSubfolders(AppFolder parentFolder, AppFolder newSubfolder)
        {
            var parentInList = FolderList.FirstOrDefault(f => f.Id == parentFolder.Id);
            if (parentInList != null)
            {
                if (parentInList.Subfolders == null)
                    parentInList.Subfolders = new ObservableCollection<AppFolder>();

                if (!parentInList.Subfolders.Any(sf => sf.Id == newSubfolder.Id))
                {
                    parentInList.Subfolders.Add(newSubfolder);
                    OnPropertyChanged(nameof(FolderList)); // Notify that FolderList has changed
                }
            }
        }


       

        private async void SelectSubfolder(AppFolder subfolder)
        {
            // Check if the subfolder is not null
            if (subfolder != null)
            {
                // Assuming you have a page to display the contents of a folder
                var flyoutPage1Detail = new MainPageViewModel(subfolder);

                // Navigate to the folder contents page
                await _navigationService.NavigateToMainPage(subfolder);
                //  SelectedFolder = null;
            }
        }
        //UNCOMMENT THIS IF NEEDED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
        private void AddSubfoldersRecursively(AppFolder parentFolder, IEnumerable<AppFolder> allFolders)
        {
            var subfolders = allFolders.Where(f => f.ParentFolderId == parentFolder.Id);
            foreach (var subfolder in subfolders)
            {
                FolderList.Add(subfolder); // Adjust this if needed to reflect hierarchy in UI
                AddSubfoldersRecursively(subfolder, allFolders);
            }
        }


        /* private void RefreshFolderList()
         {
             FolderList.Clear();
             var allFolders = AppDatabase.Instance().GetFolderList();

             // Creating a dictionary to hold folders by their IDs for easy lookup
             var foldersById = allFolders.ToDictionary(f => f.Id);

             // First, add all top-level folders to the FolderList
             foreach (var folder in allFolders.Where(f => f.ParentFolderId == null))
             {
                 FolderList.Add(folder);
             }

             // Then, populate subfolders for each folder
             foreach (var folder in allFolders)
             {
                 if (folder.ParentFolderId.HasValue && foldersById.ContainsKey(folder.ParentFolderId.Value))
                 {
                     var parentFolder = foldersById[folder.ParentFolderId.Value];
                     if (parentFolder.Subfolders == null)
                     {
                         parentFolder.Subfolders = new ObservableCollection<AppFolder>();
                     }

                     if (!parentFolder.Subfolders.Contains(folder))
                     {
                         parentFolder.Subfolders.Add(folder);
                     }
                 }
             }

             // Notifying the UI that the FolderList has been updated
             OnPropertyChanged(nameof(FolderList));
         }*/

        /* private void RefreshFolderList()
         {
             var allFolders = AppDatabase.Instance().GetFolderList();

             // Update or add new folders
             foreach (var folder in allFolders)
             {
                 var existingFolder = FolderList.FirstOrDefault(f => f.Id == folder.Id);
                 if (existingFolder != null)
                 {
                     // Update existing folder properties if needed
                     existingFolder.Name = folder.Name;
                     existingFolder.IconPath = folder.IconPath;
                     // ... other properties ...
                 }
                 else
                 {
                     // This is a new folder, add it to the list
                     FolderList.Add(folder);
                 }
             }

             // Remove any folders that no longer exist
             for (int i = FolderList.Count - 1; i >= 0; i--)
             {
                 var folder = FolderList[i];
                 if (!allFolders.Any(f => f.Id == folder.Id))
                 {
                     FolderList.RemoveAt(i);
                 }
             }

             OnPropertyChanged(nameof(FolderList));
         }*/
        //THIS REFRESH METHOD WORKS SO UNCOMMENT IT IF OTHER METHODS DONT WORK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /*public void RefreshFolderList()
        {
            var allFolders = AppDatabase.Instance().GetFolderList();
            var folderDict = FolderList.ToDictionary(f => f.Id, f => f);

            // Refresh the top-level folders (those without a ParentFolderId)
            var topLevelFolders = allFolders.Where(f => f.ParentFolderId == null).ToList();

            // Remove top-level folders that no longer exist
            for (int i = FolderList.Count - 1; i >= 0; i--)
            {
                if (!topLevelFolders.Any(tf => tf.Id == FolderList[i].Id))
                {
                    FolderList.RemoveAt(i);
                }
            }

            // Update existing folders and add new ones
            foreach (var folder in topLevelFolders)
            {
                if (folderDict.TryGetValue(folder.Id, out var existingFolder))
                {
                    // Update existing folder
                    UpdateFolderProperties(existingFolder, folder);
                }
                else
                {
                    // This is a new folder,

                    FolderList.Add(folder);
                }
            }


            // Update subfolders for each folder
            foreach (var folder in FolderList)
            {
                UpdateSubfolders(folder, allFolders);
            }

            OnPropertyChanged(nameof(FolderList));
        }*/
       /* public void RefreshFolderList()
        {
            var allFolders = AppDatabase.Instance().GetFolderList();

            // Handle the case when FolderList is not initialized
            if (FolderList == null)
            {
                FolderList = new ObservableCollection<AppFolder>(allFolders);
                return;
            }

            // Remove folders that no longer exist
            var allFolderIds = allFolders.Select(f => f.Id).ToList();
            for (int i = FolderList.Count - 1; i >= 0; i--)
            {
                if (!allFolderIds.Contains(FolderList[i].Id))
                {
                    FolderList.RemoveAt(i);
                }
            }

            // Add new folders or update existing ones
            foreach (var folder in allFolders)
            {
                var existingFolder = FolderList.FirstOrDefault(f => f.Id == folder.Id);
                if (existingFolder != null)
                {
                    UpdateFolderProperties(existingFolder, folder);
                }
                else
                {
                    FolderList.Add(folder);
                }
            }

            // Update subfolders for each folder
            foreach (var folder in FolderList)
            {
                UpdateSubfolders(folder, allFolders);
            }

            OnPropertyChanged(nameof(FolderList));
        }*/

        //  TO DO FIX FOLDER SEARCH HIERARCHY REFRESH WHERE SEARCH BREAKS IT/ MANAGE FOLDERS SEARCH HIERACHY BUILD WORKS BETTER SO GET CODE THERE IF POSSIBLE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1

        public void RefreshFolderList()
        {
            var allFolders = AppDatabase.Instance().GetFolderList();
            foreach (var folder in allFolders)
            {
                var existingFolder = FolderList.FirstOrDefault(f => f.Id == folder.Id);
                if (existingFolder != null)
                {
                    // Update existing folder
                    existingFolder.Subfolders = new ObservableCollection<AppFolder>(
                        allFolders.Where(f => f.ParentFolderId == existingFolder.Id));
                }
            }
            OnPropertyChanged(nameof(FolderList));
        }


        private void UpdateFolderProperties(AppFolder existingFolder, AppFolder newFolderData)
        {
            existingFolder.Name = newFolderData.Name;
            existingFolder.IconPath = newFolderData.IconPath;
            // ... other properties as needed ...
        }

        private void UpdateSubfolders(AppFolder parentFolder, IEnumerable<AppFolder> allFolders)
        {
            var subfolders = allFolders.Where(f => f.ParentFolderId == parentFolder.Id).ToList();

            if (parentFolder.Subfolders == null)
                parentFolder.Subfolders = new ObservableCollection<AppFolder>();

            if (parentFolder.Subfolders == null)
            {
                parentFolder.Subfolders = new ObservableCollection<AppFolder>();
            }

            // Update or add subfolders
            foreach (var subfolder in subfolders)
            {
                var existingSubfolder = parentFolder.Subfolders.FirstOrDefault(f => f.Id == subfolder.Id);
                if (existingSubfolder == null)
                {
                    parentFolder.Subfolders.Add(subfolder);
                }
                else
                {
                    // Update existing subfolder properties
                    UpdateFolderProperties(existingSubfolder, subfolder);
                }
            }
        }





        /*private void UpdateSubfolders(AppFolder parentFolder, IEnumerable<AppFolder> allFolders)
        {
            var subfolders = allFolders.Where(f => f.ParentFolderId == parentFolder.Id).ToList();
            if (parentFolder.Subfolders == null)
                parentFolder.Subfolders = new ObservableCollection<AppFolder>();

            foreach (var subfolder in subfolders)
            {
                var existingSubfolder = parentFolder.Subfolders.FirstOrDefault(f => f.Id == subfolder.Id);
                if (existingSubfolder == null)
                {
                    parentFolder.Subfolders.Add(subfolder);
                }
                else
                {
                    // Update existing subfolder properties if needed
                    existingSubfolder.Name = subfolder.Name;
                    existingSubfolder.IconPath = subfolder.IconPath;
                    // ... other properties ...
                }
            }
        }*/


        public async Task Reset()
        {
            FolderList.Clear();
            var foldersFromDb = AppDatabase.Instance().GetFolderList();
            BuildFolderHierarchy(foldersFromDb);
        }



        /*private void BuildFolderHierarchy(IEnumerable<AppFolder> allFolders)
        {
            var rootFolders = allFolders.Where(f => f.ParentFolderId == null).ToList();
            foreach (var folder in rootFolders)
            {
                folder.Subfolders = new ObservableCollection<AppFolder>(
                    allFolders.Where(f => f.ParentFolderId == folder.Id));
                FolderList.Add(folder);
            }
        }*/


        /*private void BuildFolderHierarchy(IEnumerable<AppFolder> allFolders)
        {
            var rootFolders = allFolders.Where(f => f.ParentFolderId == null).ToList();
            foreach (var folder in rootFolders)
            {
                // Calculate note count for each folder
                folder.NoteCount = AppDatabase.Instance().GetNoteList(folder.Id).Count;

                // Recursively build subfolder hierarchy and calculate their note counts
                folder.Subfolders = new ObservableCollection<AppFolder>(
                    allFolders.Where(f => f.ParentFolderId == folder.Id)
                              .Select(subfolder => {
                                  subfolder.NoteCount = AppDatabase.Instance().GetNoteList(subfolder.Id).Count;
                                  return subfolder;
                              }));
                FolderList.Add(folder);
            }
        }*/
        private void BuildFolderHierarchy(IEnumerable<AppFolder> allFolders)
        {
            var rootFolders = allFolders.Where(f => f.ParentFolderId == null).ToList();
            foreach (var folder in rootFolders)
            {
                folder.NoteCount = AppDatabase.Instance().GetNoteList(folder.Id).Count;

                folder.Subfolders = new ObservableCollection<AppFolder>(
                    allFolders.Where(f => f.ParentFolderId == folder.Id)
                              .Select(subfolder =>
                              {
                                  subfolder.NoteCountForSubfolders = AppDatabase.Instance().GetNoteList(subfolder.Id).Count;
                                  return subfolder;
                              }));
                FolderList.Add(folder);
            }
        }




        private async Task LoadFoldersFromDatabase()
        {
            var allFolders = AppDatabase.Instance().GetFolderList();
            BuildFolderHierarchy(allFolders);
        }

        /*private async Task LoadFoldersFromDatabase()
        {
            FolderList.Clear(); // Clear the observable collection

            var allFolders = AppDatabase.Instance().GetFolderList();
            BuildFolderHierarchy(allFolders);
            foreach (var folder in allFolders)
            {
                FolderList.Add(folder); // Add folders from the database
            }
        }*/



        private void BuildSubfolderHierarchy(AppFolder folder, IEnumerable<AppFolder> allFolders)
        {
            var subfolders = allFolders.Where(f => f.ParentFolderId == folder.Id);
            foreach (var subfolder in subfolders)
            {
                BuildSubfolderHierarchy(subfolder, allFolders); // Recursive call for deeper levels
                folder.Subfolders.Add(subfolder);
            }
        }





        private void UpdateFolderList(AppFolder updatedFolder)
        {
            // Find and update the folder in FolderList
            var folderToUpdate = FolderList.FirstOrDefault(f => f.Id == updatedFolder.Id);
            if (folderToUpdate != null)
            {
                // Update the properties of the folder
                folderToUpdate.Name = updatedFolder.Name;
                // Update other properties as necessary

                // If the updated folder has subfolders, handle them accordingly
                // This part depends on how you manage subfolders in your app

                OnPropertyChanged(nameof(FolderList));
            }
            else
            {
                // If the folder is not found, it might be a new folder or a deeper level subfolder
                // Handle accordingly, e.g., adding a new folder or updating subfolders
            }
        }
        private void ToggleFolder(AppFolder folder)
        {
            folder.IsExpanded = !folder.IsExpanded;
            OnPropertyChanged(nameof(FolderList));
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
        //CREATE POPUPS IN FOR SECURE FOLDER SELECT IN AN EFFORT TO HIDE PASSWORD, SAME FOR MANAGE FOLDERS DELETE AND RENAME
        //GET MORE ROBUST PASSWORD ENCRYPTION AND POSSIBLY SET UP USERNAME AND PASSWORD RETRIEVE METHODS. ALSO CREATE METHODS TO DENY FOLDER DELETION ON SECURE FOLDER WITHOUT PASSWORD. ALSO SHOW LOCK FOLDER ICON FOR SECURE FOLDERS
        public async void NavigateToFlyoutPage1Detail(AppFolder selectedFolder)
        {
            if (selectedFolder != null)
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
        /*private void ShowEditFolderPopup()
        {
            _selectedFolder = SelectedFolder;
            var editFolderPopup = new EditFolderPopup(this);
            Application.Current.MainPage.Navigation.ShowPopup(editFolderPopup);
        }*/



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}