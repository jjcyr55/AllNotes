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

        private async Task RenameSelectedFolder()
        {
            if (_selectedFolders != null && _selectedFolders.Count == 1)
            {
                var selectedFolder = _selectedFolders.FirstOrDefault() as AppFolder;
                if (selectedFolder != null)
                {
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
