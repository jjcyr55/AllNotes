using AllNotes.Database;
using AllNotes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AllNotes.ViewModels
{
    public class ManageFoldersViewModel : BaseViewModel
    {
        public ObservableCollection<AppFolder> FolderList { get; set; }
        public ICommand DeleteFoldersCommand { get; set; }
        public ICommand RenameFolderCommand { get; private set; }

        private ObservableCollection<AppFolder> selectedFolder;
        private AppFolder _selectedFolder;
        private AppDatabase _appDatabase;
      
        private ObservableCollection<AppFolder> _folderList;

       
        int controlMenuCount = 0;
        private int folderID;
        int selectedFolderID = 0;
        
        public ManageFoldersViewModel()
        {
            FolderList = new ObservableCollection<AppFolder>(AppDatabase.Instance().GetFolderList());

           
            selectedFolderID = folderID;
            _appDatabase = new AppDatabase(); 
            SelectedFolders = _selectedFolders;
          
            DeleteFoldersCommand = new Command(async () => await DeleteSelectedFolders());
            RenameFolderCommand = new Command(async () => await RenameSelectedFolder());
            _selectedFolders = new List<object>();
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
            if (_selectedFolders != null && _selectedFolders.Count == 1)
            {
                var selectedFolder = _selectedFolders.FirstOrDefault() as AppFolder;
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
                        MessagingCenter.Send(this, "FoldersUpdated");
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

        private void RefreshFolders()
        {
            FolderList.Clear();
            var allFolders = AppDatabase.Instance().GetFolderList();
            foreach (var folder in allFolders)
            {
                FolderList.Add(folder);
            }
        }
    }
    }
