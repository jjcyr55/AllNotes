using AllNotes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AllNotes.ViewModels
{
    public class MainPagePopupViewModel : BaseViewModel
    {
        public ObservableCollection<MenuItemModel> MenuItems { get; set; }
        public ICommand ItemSelectedCommand { get; private set; }


        
        public ICommand EditCommand => new Command(() =>
        {
            MessagingCenter.Send(this, "ToggleEdit");
        });



        public MainPagePopupViewModel()
        {
            // Initialize the command
            ItemSelectedCommand = new Command<MenuItemModel>(ExecuteMenuItem);

            // Initialize and populate the MenuItems collection
           /* MenuItems = new ObservableCollection<MenuItemModel>
        {
            new MenuItemModel { Title = "Edit", CommandAction = EditNotes},
            new MenuItemModel { Title = "Create Folder", CommandAction = CreateNewFolder },
            new MenuItemModel { Title = "Note Template", CommandAction = NewNoteTemplate },
            new MenuItemModel { Title = "Unfavorite", CommandAction = UnfavoriteNote },
            // ... other menu items ...
        };*/
        }

        private void UnfavoriteNote()
        {
            throw new NotImplementedException();
        }

        private void NewNoteTemplate()
        {
            throw new NotImplementedException();
        }

        private void CreateNewFolder()
        {
            throw new NotImplementedException();
        }

        private void EditNotes()
        {
            throw new NotImplementedException();
        }

        // MenuItemSelectedCommand = new Command<MenuItemModel>(ExecuteMenuItem);
        //   MenuItems = new ObservableCollection<MenuItemModel>
        private void ExecuteMenuItem(MenuItemModel item)
        {
            item?.CommandAction?.Invoke();
        }

        // Implement INotifyPropertyChanged

        public class MenuItem
        {
            public string Title { get; set; }
            // You can add more properties if needed
        }
    }
}