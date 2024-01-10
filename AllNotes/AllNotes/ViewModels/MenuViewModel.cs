using AllNotes.Models;
using AllNotes.Views;
using AllNotes.Views.NewNote.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllNotes.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        //  public ObservableCollection<MenuItemModel> MenuItems { get; set; }
        /* public ICommand MenuItemSelectedCommand { get; }
         public ObservableCollection<MenuItemModel> MenuItems { get; set; }
         public static NewNotePopup Instance { get; private set; }
       //  public ICommand OpenMenuCommand => new Command(OpenMenu);
         public MenuViewModel(NewNotePopup newNotePopup)
         {


             MenuItemSelectedCommand = new Command<MenuItemModel>(ExecuteMenuItem);


             //  new MenuItemModel { Title = "Share", CommandAction = () => Application.Current.MainPage.DisplayAlert("Test", "Share tapped", "OK") };

             MenuItems = new ObservableCollection<MenuItemModel>
          {
              new MenuItemModel { Title = "Color Selection", CommandAction = SelectColor },
              new MenuItemModel { Title = "Share", CommandAction = ShareNote }
          };


         }

         public MenuViewModel()
         {
             InitializeMenuItems();
         }
         private void InitializeMenuItems()
         {
             MenuItems = new ObservableCollection<MenuItemModel>
     {
         new MenuItemModel { Title = "Color Selection", CommandAction = SelectColor },
         new MenuItemModel { Title = "Share", CommandAction = ShareNote }
     };
         }
         *//*private void InitializeMenuItems()
         {
             MenuItems = new ObservableCollection<MenuItemModel>
     {
         new MenuItemModel { Title = "Color Selection", Command = new Command(SelectColor) },
         new MenuItemModel { Title = "Share", Command = new Command(ShareNote) }
         // Other items...
     };
         }*//*




         private MenuItemModel _selectedMenuItem;
         public MenuItemModel SelectedMenuItem
         {
             get => _selectedMenuItem;
             set
             {
                 if (_selectedMenuItem != value)
                 {
                     _selectedMenuItem = value;
                     OnPropertyChanged(nameof(SelectedMenuItem));
                     // Execute the command associated with the selected item
                     _selectedMenuItem?.Command?.Execute(null);
                 }
             }
         }

         public void SetNoteData(string text, string title)
         {
             NewNoteText = text;
             NewNoteTitle = title;
         }
         public async void ShareNote()
         {
             try
             {
                 // Assuming CurrentNoteContent holds the text of the note being edited or created
                 var noteContent = NewNoteText;
                 var noteTitle = NewNoteTitle;

                 Device.BeginInvokeOnMainThread(async () =>
                 {
                     await Share.RequestAsync(new ShareTextRequest
                     {
                         Text = noteContent,
                         Title = noteTitle
                     });
                 });
             }
             catch (Exception ex)
             {
                 Debug.WriteLine($"Error in sharing: {ex.Message}");
                 Device.BeginInvokeOnMainThread(async () =>
                 {
                     await Application.Current.MainPage.DisplayAlert("Error", $"Unable to share note: {ex.Message}", "OK");
                 });
             }
         }

         private ObservableCollection<MenuItemModel> _menuItems;
        *//* public ObservableCollection<MenuItemModel> MenuItems
         {
             get => _menuItems;
             set
             {
                 _menuItems = value;
                 OnPropertyChanged(nameof(MenuItems)); // Assuming INotifyPropertyChanged implementation
             }
         }*//*
         private string _newNoteText;

         public string NewNoteText
         {
             get { return _newNoteText; }
             set
             {
                 _newNoteText = value;
                 OnPropertyChanged(nameof(NewNoteText));
             }
         }
         private string _newNoteTitle;

         public string NewNoteTitle
         {
             get { return _newNoteTitle; }
             set
             {
                 _newNoteTitle = value;
                 OnPropertyChanged(nameof(NewNoteTitle));
             }
         }

         private async void SelectColor()
         {
             // Implement color selection logic here
         }
         *//*private void OpenMenu()
         {
             var newNotePopup = new NewNotePopup();

             // Assuming MenuViewModel has methods to set the note's text and title
             if (newNotePopup.BindingContext is MenuViewModel menuViewModel)
             {
                 menuViewModel.SetNoteData(NewNoteText, NewNoteTitle);
             }

             Application.Current.MainPage.Navigation.ShowPopup(newNotePopup);
         }*//*
         public ICommand OpenMenuCommand => new Command(OpenMenu);

         public object CurrentNoteContent { get; private set; }

         *//* private void OpenMenu()
          {
              // Create and navigate to the Popup
              var newNotePopup = new NewNotePopup();
              Application.Current.MainPage.Navigation.ShowPopup(newNotePopup);
          }*/
        /* private void OpenMenu()
         {
             var newNotePopup = new NewNotePopup();
             newNotePopup.BindingContext = this;
             Application.Current.MainPage.Navigation.ShowPopup(newNotePopup);
         }*//*
        public void ExecuteMenuItem(MenuItemModel item)
        {
            item?.CommandAction?.Invoke();
        }
        public void OpenMenu()
        {
            var newNotePopup = new NewNotePopup();
            newNotePopup.BindingContext = new MenuViewModel();
            Application.Current.MainPage.Navigation.ShowPopup(newNotePopup);
        }*/

    }

}