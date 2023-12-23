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
                        database = new IDatabase()._database;
                    }

                    return database;
                }
            }

            // public static object INoteRepository { get; set; }

            public App()
        {
            InitializeComponent();

            //DependencyService.Register<Database>();
              
            MainPage = new NavigationPage(new FlyoutPage1());
            // MainPage = new FlyoutPage1Detail();
          //  MainPage = new FlyoutPage1Detail();
        }


        protected override async void OnStart()
        {
            base.OnStart();
            INoteRepository noteRepository = new NoteRepository();
            await noteRepository.InitializeDefaultFolder();
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