using System;
using SQLite;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using AllNotes.Data;
//using AllNotes.ViewModels;
using SQLite;
using AllNotes.Data;
using AllNotes.Services;
using AllNotes.Views;
using AllNotes.ViewModels;
using AllNotes.Repositories;
using AllNotes.Interfaces;
using AllNotes.Database;

using System.Threading.Tasks;
using AllNotes.Models;
using Xamarin.Essentials;

namespace AllNotes
{
   
        public partial class App : Application
        {

        public App()
        {
            InitializeComponent();
            // MainPage = new NavigationPage(new FlyoutPage1());
          //  Ngo9BigBOggjHTQxAR8 / V1NAaF5cWWJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWX5feXRcR2NeU0NyXEQ =
             AppDatabase.Instance().Init();
            DependencyService.Register<INavigationService, NavigationService>();


            /* var mainPage = new FlyoutPage1
             {
                 Flyout = new MenuPage(), // Replace with your actual menu page
                 Detail = new NavigationPage(new MenuPage()) // Replace with your actual detail page
             };

             MainPage = mainPage;*/
            var mainPageViewModel = new MainPageViewModel();
            var detailPage = new FlyoutPage1Detail
            {
                BindingContext = mainPageViewModel
            };

            MainPage = new FlyoutPage
            {
                Detail = new NavigationPage(detailPage),
                Flyout = new MenuPage()
            };
        }
        protected override async void OnStart()
        {
            
        }
        private async void PromptUserToOpenMenu()
        {
            var userResponse = await Application.Current.MainPage.DisplayAlert(
                "Select Folder",
                "Please select a folder to continue.",
                "Go to Menu",
                "Cancel");

            if (userResponse)
            {
                OpenMenu();
            }
        }

        private void OpenMenu()
        {
            if (Application.Current.MainPage is FlyoutPage mainPage)
            {
                mainPage.IsPresented = true; // This will open the flyout menu
            }
        }
        private async Task InitializeApplicationAsync()
        {
            /*int defaultFolderId = await GetDefaultFolderId();

            AppFolder defaultFolder = AppDatabase.Instance().GetFolder(defaultFolderId);
            if (defaultFolder != null)
            {
                MainPageViewModel mainPageViewModel = new MainPageViewModel();
                mainPageViewModel.LoadNotesForFolder(defaultFolder);
                MainPage = new FlyoutPage1Detail(mainPageViewModel);
            }
            else
            {
                // Handle the case where the default folder is not found
            }*/
        }

        private async Task<int> GetDefaultFolderId()
        {
            var defaultFolder = await AppDatabase.Instance().GetFirstFolder();
            return defaultFolder?.Id ?? 0; // Assuming 0 is a safe default ID
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            var menuPageViewModel = new MenuPageViewModel();
   menuPageViewModel.RefreshFolderList();
        }
    }
}