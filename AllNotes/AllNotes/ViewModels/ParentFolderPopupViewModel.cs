using AllNotes.Database;
using AllNotes.Models;
using AllNotes.Views;
using AllNotes.Views.NewNote.Popups;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace AllNotes.ViewModels
{
    public class ParentFolderPopupViewModel : BaseViewModel
    {

        public ICommand DeleteFoldersCommand { get; set; }
        public ICommand RenameFolderCommand { get; private set; }
      //   public ICommand RenameFolderCommand { get; private set; }
       //   public ICommand DeleteFolderCommand { get; private set; }
        public AppFolder CurrentFolder { get; set; }

        public ICommand AddSubfolderCommand { get; }

        private Models.AppFolder _selectedFolder;
        private Models.AppFolder _currentFolder;
        public ParentFolderPopupViewModel(Models.AppFolder folder)
        {
            MessagingCenter.Subscribe<ParentFolderPopup>(this, "FolderDeleted", (sender) =>
            {
                // Logic to refresh the folder list
            });
            CurrentFolder = folder;
           //  DeleteFolderCommand = new Command(async () => await DeleteSelectedFolders(CurrentFolder));
               DeleteFoldersCommand = new Command(async () => await DeleteSelectedFolders());
             RenameFolderCommand = new Command(async () => await RenameSelectedFolder());
            _currentFolder = folder;
            _selectedFolder = folder;
            AddSubfolderCommand = new Command(ExecuteAddSubfolder);
            // RenameFolderCommand = new Command(async () => await RenameFolder());


        }

        private async Task RenameSelectedFolder()
        {
            if (_currentFolder != null)
            {
                // Check if the folder is secure and prompt for a password
                if (_currentFolder.IsSecure)
                {
                    string inputPassword = await Application.Current.MainPage.DisplayPromptAsync("Secure Folder", "Enter password:", "Ok", "Cancel", "Password", maxLength: 20, keyboard: Keyboard.Text);

                    // Validate password
                    if (!ValidatePassword(inputPassword, _currentFolder.EncryptedPassword))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Incorrect Password", "OK");
                        return; // Exit if password is incorrect
                    }
                }

                // Proceed with renaming after password validation
                string newName = await Application.Current.MainPage.DisplayPromptAsync("Rename Folder", "Enter new folder name:", initialValue: _currentFolder.Name);
                if (!string.IsNullOrEmpty(newName))
                {
                    _currentFolder.Name = newName;
                    AppDatabase.Instance().UpdateFolder(_currentFolder);

                    // Update necessary elements or send messages to notify other parts of the application
                    MessagingCenter.Send(this, "FolderUpdated", _currentFolder);
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No folder selected for renaming.", "OK");
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

        private async Task DeleteSelectedFolders()
        {
            if (_currentFolder != null)
            {
                // Check if the folder is secure and prompt for a password
                if (_currentFolder.IsSecure)
                {
                    string inputPassword = await Application.Current.MainPage.DisplayPromptAsync("Secure Folder", "Enter password:", "Ok", "Cancel", "Password", maxLength: 20, keyboard: Keyboard.Text);

                    // Validate password
                    if (!ValidatePassword(inputPassword, _currentFolder.EncryptedPassword))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Incorrect Password", "OK");
                        return; // Exit if password is incorrect
                    }
                }

                // Confirm deletion
                bool isUserSure = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Delete",
                    $"Are you sure you want to delete the folder '{_currentFolder.Name}'?",
                    "Yes",
                    "No");

                if (isUserSure)
                {
                    AppDatabase.Instance().DeleteFolder(_currentFolder);

                    // Notify the application about the deletion
                    MessagingCenter.Send(this, "FolderUpdated", _currentFolder);
                }
            }
        }

        private void ExecuteAddSubfolder()
        {
            // Add subfolder logic
        }
    }
}