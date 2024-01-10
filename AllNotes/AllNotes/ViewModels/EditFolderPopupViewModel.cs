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
    public class EditFolderPopupViewModel //: INotifyPropertyChanged
    {
       /* public ICommand RenameCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private AppFolder _selectedFolder;

        public EditFolderPopupViewModel(AppFolder selectedFolder)
        {
           *//* _selectedFolder = selectedFolder;
            RenameCommand = new Command(async () => await RenameFolder());
            DeleteCommand = new Command(DeleteFolder);
            CancelCommand = new Command(Cancel);*//*
        }

        private async Task RenameFolder()
        {
            *//* if (_selectedFolder == null || string.IsNullOrEmpty(_selectedFolder.Name))
             {
                 // Handle the case where _selectedFolder is null or name is empty
                 return;
             }*/
           /* if (_selectedFolder == null || string.IsNullOrEmpty(_selectedFolder.Name))
            {
                // Display an alert or handle the case where no folder is selected
                await Application.Current.MainPage.DisplayAlert("Error", "No folder selected for renaming.", "OK");
                return;
            }*//*

            string result = await Application.Current.MainPage.DisplayPromptAsync(
                "Rename Folder",
                "Enter new folder name:",
                initialValue: _selectedFolder.Name,
                maxLength: 250,
                keyboard: Keyboard.Default);
           
            if (!string.IsNullOrWhiteSpace(result))
            {
                _selectedFolder.Name = result;
               AppDatabase.Instance().UpdateFolder(_selectedFolder);
                // You can send a message to refresh the UI or call a method directly if you have access
                MessagingCenter.Send(this, "FolderUpdated");
            }
        }

        private void DeleteFolder()
        {
            // Logic for deleting the folder
            // Show confirmation dialog
            // Delete the folder and refresh the UI
        }

        private void Cancel()
        {
            // Logic to close the popup
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/
    }
}