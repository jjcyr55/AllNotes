using AllNotes.Database;
using AllNotes.Models;
using AllNotes.Services;
using AllNotes.Views;
using AllNotes.Views.NewNote.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace AllNotes.ViewModels
{
    public class ManageFoldersViewModel : BaseViewModel
    {

        //  private AppFolder _selectedFolder;
        private SelectionMode _selectionMode = SelectionMode.None;
        //  public ObservableCollection<AppFolder> FolderList { get; set; }
        public ObservableCollection<AppFolder> DisplayFolders { get; set; }

        int controlMenuCount = 0;
        private int folderID;
        int selectedFolderID = 0;
        private ObservableCollection<AppFolder> _folderList;

        public ObservableCollection<AppFolder> Folders { get; set; }


        public ObservableCollection<AppFolder> FolderList
        {
            get => _folderList;
            set
            {
                _folderList = value;
                OnPropertyChanged(nameof(FolderList));
            }
        }
        private AppFolder _selectedFolder;
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
                        HandleFolderSelection(_selectedFolder);
                    }
                }
            }
        }
        public ICommand DeleteFoldersCommand { get; set; }
        public ICommand RenameFolderCommand { get; private set; }

        // public ICommand AddSubfolderCommand { get; private set; }
        public ICommand SelectParentFolderCommand { get; }
        public ICommand SelectSubfolderCommand { get; }
        public ICommand AddSubfolderCommand { get; private set; }
        public ICommand ToggleFolderCommand { get; private set; }
        public ICommand RenameSubfolderCommand { get; private set; }

      
        public ICommand OpenSubFolderPopupCommand { get; private set; }

        private ObservableCollection<AppFolder> selectedFolder;
        //   private AppFolder _selectedFolder;
        private AppDatabase _appDatabase;

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
         private AppFolder _selectedSubfolder;
        public AppFolder SelectedSubfolder
        {
            get => _selectedSubfolder;
            set
            {
                if (_selectedSubfolder != value)
                {
                    _selectedSubfolder = value;
                    OnPropertyChanged(nameof(SelectedSubfolder));
                    // Additional logic if needed when a subfolder is selected
                }
            }
        }
        /* private AppFolder _selectedFolder;
         public AppFolder SelectedFolder
         {
             get => _selectedFolder;
             set
             {
                 if (_selectedFolder != value)
                 {
                     // Deselect the previous folder
                     if (_selectedFolder != null)
                     {
                         _selectedFolder.IsSelected = false;
                     }

                     _selectedFolder = value;
                     if (_selectedFolder != null)
                     {
                         _selectedFolder.IsSelected = true;
                     }

                     OnPropertyChanged(nameof(SelectedFolder));
                 }
             }
         }*/
        public ICommand OpenParentFolderPopupCommand { get; private set; }

        public ICommand FolderOptionsCommand { get; private set; }
        public ICommand FolderIconTappedCommand { get; private set; }
        public ICommand OpenFolderOptionsCommand { get; private set; }
        public ICommand AddFolderCommand { get; private set; }
        private readonly FolderService _folderService;

       
        public ManageFoldersViewModel(Views.ManageFolders manageFolders)
        {

            FolderIconTappedCommand = new Command<AppFolder>(HandleFolderIconTapped);

          //  OpenParentFolderPopupCommand = new Command<AppFolder>(OpenParentFolderPopup);
            MessagingCenter.Subscribe<ParentFolderPopupViewModel, AppFolder>(this, "FolderUpdated", (sender, updatedFolder) =>
            {
                // Update the folder in your FolderList
                var folderToUpdate = FolderList.FirstOrDefault(f => f.Id == updatedFolder.Id);
                if (folderToUpdate != null)
                {
                    folderToUpdate.Name = updatedFolder.Name;
                    // Update other properties as necessary

                    OnPropertyChanged(nameof(FolderList));
                }

                // Optionally, you can call a method to refresh the entire folder list
                 Reset();
            });
            
            

            SelectSubfolderCommand = new Command<AppFolder>(SelectSubfolder);

            // _folderService = folderService;
            SelectParentFolderCommand = new Command<AppFolder>(SelectParentFolder);

            
            SelectedFolder = null;
            //  ToggleFolderCommand = new Command<AppFolder>(ToggleFolder);

            //  AddSubfolderCommand = new Command<AppFolder>(AddSubfolder);

            AddSubfolderCommand = new Command(async () => await AddSubfolder());
            //   AddSubfolderCommand = new Command<AppFolder>(async (parentFolder) => await AddSubfolder(parentFolder));

            MessagingCenter.Subscribe<ManageFoldersViewModel, AppFolder>(this, "FolderUpdated", (sender, updatedFolder) =>
            {
                // Logic to update the folder list based on the updated folder
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

            FolderList = new ObservableCollection<AppFolder>(AppDatabase.Instance().GetFolderList());
            SelectedFolders = new ObservableCollection<object>();

            selectedFolderID = folderID;
            _appDatabase = new AppDatabase();
            SelectedFolders = _selectedFolders;

            DeleteFoldersCommand = new Command(async () => await DeleteSelectedFolders());
            RenameFolderCommand = new Command(async () => await RenameSelectedFolder());
            _selectedFolders = new List<object>();
            //   RefreshFolderList();
            RefreshFolders();
            //   Reset();
            // BuildFolderHierarchy();
            // BuildFolderHierarchy(allFolders);


        }


        /*public void FolderSelected(AppFolder selectedFolder)
        {
            OnFolderSelected?.Invoke(selectedFolder);
        }*/


        private List<AppNote> _notesToMove;
        // public bool IsMoveNoteMode { get; set; }
        private bool _isMoveNoteMode;
        public bool IsMoveNoteMode
        {
            get => _isMoveNoteMode;
            set
            {
                _isMoveNoteMode = value;
                OnPropertyChanged(nameof(IsMoveNoteMode));
            }
        }
        public bool IsMoveNoteContext { get; set; }
        public ManageFoldersViewModel(List<AppNote> notesToMove = null)
        {
            _notesToMove = notesToMove;
            IsMoveNoteMode = notesToMove != null;
            // Other initialization code...
        }
        public ICommand SelectFolderCommand => new Command<AppFolder>(SelectFolder);
        public Action<AppFolder> OnFolderSelected { get; set; }
        private void SelectFolder(AppFolder folder)
        {
            OnFolderSelected?.Invoke(folder);
        }


        public ICommand FolderSelectedCommand => new Command<AppFolder>(FolderSelected);

        private async void FolderSelected(AppFolder selectedFolder)
        {
            if (IsMoveNoteMode)
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Move Notes",
                    $"Are you sure you want to move the selected notes to '{selectedFolder.Name}'?",
                    "Yes",
                    "No");

                if (confirm)
                {
                    MoveNotesToFolder(selectedFolder);
                    ExitMoveNoteMode();
                }
            }
            else
            {
                // Normal folder selection logic
            }
        }

        private void ExitMoveNoteMode()
        {
            IsMoveNoteMode = false;
            _notesToMove = null;
            // Navigate back or refresh view
        }
        private void MoveNotesToFolder(AppFolder selectedFolder)
        {
            if (_notesToMove == null || selectedFolder == null)
                return;

            foreach (var note in _notesToMove)
            {
                // Update the folder ID of the note
                note.folderID = selectedFolder.Id;

                // Save the updated note to the database
                AppDatabase.Instance().UpdateNote(note);
            }

            // Optionally, send a message to update UI or refresh the notes list
            MessagingCenter.Send(this, "NotesMoved", selectedFolder.Id);
        }


        private void OpenFolderOptionsPopup(AppFolder folder)
        {
            // Logic to open the popup
            var folderOptionsPopup = new ParentFolderPopup(folder);
          
            Application.Current.MainPage.Navigation.ShowPopup(folderOptionsPopup);

            // Open the popup
        }

        private void HandleFolderIconTapped(AppFolder folder)
        {
            if (folder != null)
            {
                SelectedFolder = folder; // Set the selected folder
                OpenFolderOptionsPopup(folder); // Method to open the popup
            }
        }

       

        public ManageFoldersViewModel()
        {
        }

        

        private void SelectParentFolder(AppFolder folder)
        {
            // Handle parent folder selection
        }
        private async void SelectSubfolder(AppFolder subfolder)
        {
            // Check if the subfolder is not null
            if (subfolder != null)
            {
                // Assuming you have a page to display the contents of a folder
                var flyoutPage1Detail = new MainPageViewModel(subfolder);

                // Navigate to the folder contents page
                // await _navigationService.NavigateToMainPage(subfolder);
                //  SelectedFolder = null;
            }
        }
        

        private void ToggleFolder(AppFolder folder)
        {
            if (folder != null)
            {
                folder.IsExpanded = !folder.IsExpanded;
                // Notify UI about changes
                OnPropertyChanged(nameof(FolderList));
            }
        }

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
        public async Task Reset()
        {
            FolderList.Clear();
            var foldersFromDb = AppDatabase.Instance().GetFolderList();
            BuildFolderHierarchy(foldersFromDb);
        }
        private async Task LoadFoldersFromDatabase()
        {
            var allFolders = AppDatabase.Instance().GetFolderList();
            BuildFolderHierarchy(allFolders);
        }
        /* private void BuildFolderHierarchy(IEnumerable<AppFolder> allFolders)
         {
             var rootFolders = allFolders.Where(f => f.ParentFolderId == null).ToList();
             foreach (var folder in rootFolders)
             {
                 folder.Subfolders = new ObservableCollection<AppFolder>(
                     allFolders.Where(f => f.ParentFolderId == folder.Id));
                 FolderList.Add(folder);
             }
         }*/
        private void BuildFolderHierarchy(IEnumerable<AppFolder> allFolders)
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
        }
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



       
        private async Task AddSubfolder()
        {
            if (_selectedFolders != null && _selectedFolders.Count == 1)
            {
                var parentFolder = _selectedFolders.FirstOrDefault() as AppFolder;
                if (parentFolder != null)
                {
                    string subfolderName = await Application.Current.MainPage.DisplayPromptAsync("New Subfolder", "Enter subfolder name:");
                    if (!string.IsNullOrEmpty(subfolderName))
                    {
                        var newSubfolder = new AppFolder
                        {
                            Name = subfolderName,
                            ParentFolderId = parentFolder.Id,
                            IconPath = "folder_account_outline.png" // Set the icon path here
                        };

                        // Insert the new subfolder into the database
                        AppDatabase.Instance().InsertFolder(newSubfolder);

                        // Send a message to notify that a new subfolder has been added
                        MessagingCenter.Send(this, "SubfolderAdded", newSubfolder);
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Add Subfolder", "Please select a single folder to add a subfolder.", "OK");
            }
        }





        private void RefreshUIWithNewSubfolder(AppFolder newSubfolder, AppFolder parentFolder)
        {
            // Add the new subfolder to the parent folder's subfolder collection
            if (parentFolder.Subfolders == null)
            {
                parentFolder.Subfolders = new ObservableCollection<AppFolder>();
            }
            parentFolder.Subfolders.Add(newSubfolder);

            // Notify the UI of the changes
            OnPropertyChanged(nameof(FolderList));
        }




        private IList<object> _selectedFolders;

        public IList<object> SelectedFolders
        {
            get => _selectedFolders;
            set
            {
                _selectedFolders = value;
                OnPropertyChanged(nameof(SelectedFolders));
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
        //PROMPT GETS STUCK ON INFINITE LOOP PLEASE SELECT A SINGLE FOLDER TO RENAME IF MORE THAN ONE FOLDER IS SELECTED FOR RENAME, DELETE WORKING FOOD SO FAR
        public object LockIconVisible { get; private set; }

        private async Task RenameSelectedFolder()
        {
            if (_selectedFolders.Count == 1)
            //if (_selectedFolders != null && _selectedFolders.Count == 1)
            {
                var selectedFolder = _selectedFolders[0] as AppFolder;
                // var selectedFolder = _selectedFolders.FirstOrDefault() as AppFolder;
                if (selectedFolder != null)
                {
                    // Check if the folder is secure and prompt for a password
                    if (selectedFolder.IsSecure)
                    {
                        string inputPassword = await Application.Current.MainPage.DisplayPromptAsync("Secure Folder", "Enter password:", "Ok", "Cancel", "Password", maxLength: 20, keyboard: Keyboard.Text);

                        // Validate password
                        if (!ValidatePassword(inputPassword, selectedFolder.EncryptedPassword))
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", "Incorrect Password", "OK");
                            return; // Exit if password is incorrect
                        }
                    }

                    // Proceed with renaming after password validation
                    string newName = await Application.Current.MainPage.DisplayPromptAsync("Rename Folder", "Enter new folder name:", initialValue: selectedFolder.Name);
                    if (!string.IsNullOrEmpty(newName))
                    {
                        selectedFolder.Name = newName;
                        AppDatabase.Instance().UpdateFolder(selectedFolder);

                        RefreshFolders();
                        MessagingCenter.Send(this, "FolderRenamed");
                        MessagingCenter.Send(this, "FoldersUpdated");

                        //   MessagingCenter.Send(this, "SubfolderAdded", newSubfolder);
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Rename Folder", "Please select a single folder to rename.", "OK");
            }
        }
       


        private async Task DeleteSelectedFolders()
        {
            if (_selectedFolders != null && _selectedFolders.Any())
            {
                // Check if any selected folder is secure
                bool containsSecureFolder = _selectedFolders.Any(obj => obj is AppFolder folder && folder.IsSecure);

                if (containsSecureFolder)
                {
                    string inputPassword = await Application.Current.MainPage.DisplayPromptAsync("Secure Folder", "Enter password:", "Ok", "Cancel", "Password", maxLength: 20, keyboard: Keyboard.Text);

                    // Validate password for all secure folders
                    bool isPasswordValid = _selectedFolders.All(obj =>
                    {
                        if (obj is AppFolder folder && folder.IsSecure)
                        {
                            return ValidatePassword(inputPassword, folder.EncryptedPassword);
                        }
                        return true; // If folder is not secure, skip password check
                    });

                    if (!isPasswordValid)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Incorrect Password", "OK");
                        return; // Exit if password is incorrect
                    }
                }

                // Confirm deletion
                bool isUserSure = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Delete",
                    "Are you sure you want to delete the selected folders?",
                    "Yes",
                    "No");

                if (isUserSure)
                {
                    foreach (var obj in _selectedFolders)
                    {
                        if (obj is AppFolder folder)
                        {
                            AppDatabase.Instance().DeleteFolder(folder);
                        }
                    }

                    RefreshFolders();
                    _selectedFolders.Clear();
                    MessagingCenter.Send(this, "FoldersUpdated");
                }
            }

            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "No Folders Selected",
                    "Please select folders to delete.",
                    "OK");
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

        private string _searchQuery;
        private ManageFolders manageFolders;

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
                RefreshFolders();
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
        public void MoveNoteToFolder(AppNote noteToMove, AppFolder targetFolder)
        {
            if (noteToMove == null || targetFolder == null) return;

            // Logic to update the note's folder in the database
            noteToMove.folderID = targetFolder.Id;
            AppDatabase.Instance().UpdateNote(noteToMove);

            // Refresh views and notify users
            RefreshFolders();
            MessagingCenter.Send(this, "NoteMoved", noteToMove);
        }
        public void RefreshFolders()
        {
            var allFolders = AppDatabase.Instance().GetFolderList();
            FolderList.Clear();

            // Rebuild the hierarchy
            foreach (var folder in allFolders.Where(f => f.ParentFolderId == null))
            {
                BuildSubfolderHierarchy(folder, allFolders);
                FolderList.Add(folder);
            }

            OnPropertyChanged(nameof(FolderList));

            // Notify other view models to refresh their lists
            MessagingCenter.Send(this, "FolderListUpdated");
        }
        private void AssignNestingLevel(AppFolder folder, IEnumerable<AppFolder> allFolders, int currentLevel = 0)
        {
            folder.NestingLevel = currentLevel;
            var subfolders = allFolders.Where(f => f.ParentFolderId ==
        folder.Id);
            foreach (var subfolder in subfolders)
            {
                AssignNestingLevel(subfolder, allFolders, currentLevel + 1);
            }
        }

        private void UpdateSelectedFoldersList(AppFolder folder)
        {
            if (folder.IsSelected)
            {
                if (!_selectedFolders.Contains(folder))
                {
                    _selectedFolders.Add(folder);
                }
            }
            else
            {
                _selectedFolders.Remove(folder);
            }
            OnPropertyChanged(nameof(SelectedFolders));
        }


        private void HandleFolderSelection(AppFolder folder)
        {
            if (folder != null)
            {
                folder.IsSelected = !folder.IsSelected;

                // Update the SelectedFolders list if needed
                UpdateSelectedFoldersList(folder);
            }
        }

        public List<AppFolder> GetSelectedFolders()
        {
            return FolderList.Where(folder => folder.IsSelected).ToList();
        }
       
    }
}
