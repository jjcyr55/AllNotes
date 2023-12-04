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

namespace AllNotes.ViewModels
{
    public class MenuPageViewModel
    {
        public ICommand OpenNewFolderCommand { get; private set; }

        public MenuPageViewModel()
        {
            OpenNewFolderCommand = new Command(ExecuteOpenNewFolderCommand);
        }
        public Command<Note> NewFolderPage { get; set; }
        private async void ExecuteOpenNewFolderCommand()
        {


            NewFolderViewModel newFolderViewModel = new NewFolderViewModel();
            NewFolderPage newFolderPage = new NewFolderPage();
            // Open the new folder popup
            //  var newFolderViewModel = new NewFolderViewModel();
            // var newFolderPage = new NewFolderPage { BindingContext = newFolderViewModel };
            //  await Application.Current.MainPage.Navigation.PushAsync(newNotePage);
           
            await Application.Current.MainPage.Navigation.PushAsync(newFolderPage);
        }
    }
}