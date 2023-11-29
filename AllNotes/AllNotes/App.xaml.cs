using System;
using SQLite;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using AllNotes.Data;
//using AllNotes.ViewModels;
using SQLite;
using AllNotes.Data;
using AllNotes.Services;

namespace AllNotes
{
    public partial class App : Application
    {
        private static SQLiteAsyncConnection database;
        public static object INoteRepository { get; internal set; }


        public static SQLiteAsyncConnection Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database()._database;
                }

                return database;
            }
        }

       // public static object INoteRepository { get; set; }

        public App()
        {
            InitializeComponent();
            var mainPageViewModel = new MainPageViewModel();
            MainPage = new NavigationPage(new MainPage(mainPageViewModel));
            MainPage.BindingContext = mainPageViewModel;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}