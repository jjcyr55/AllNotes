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
using AllNotes.Repositories;
using AllNotes.Interfaces;
using AllNotes.Database;
using AllNotes.ViewModels;

namespace AllNotes
{
   
        public partial class App : Application
        {

        public App()
        {
            InitializeComponent();


            AppDatabase.Instance().Init();
            DependencyService.Register<INavigationService, NavigationService>();

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
          //  base.OnStart();
           // INoteRepository noteRepository = new NoteRepository();
        //    await noteRepository.InitializeDefaultFolder();
            // Other startup code...
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}