using AllNotes.Database;
using AllNotes.Interfaces;
using AllNotes.Models;
using AllNotes.ViewModels;
using AllNotes.Views;
using AllNotes.Views.NewNote;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AllNotes.Services
{
    public class NavigationService : INavigationService
    {
        private SelectionMode _selectionMode;

        public bool IsEditMode { get; private set; }

        public async Task NavigateToMainPage(AppFolder selectedFolder)
        {
            var mainPageViewModel = new MainPageViewModel(selectedFolder);
            var flyoutPage1Detail = new FlyoutPage1Detail { BindingContext = mainPageViewModel };

            if (Application.Current.MainPage is FlyoutPage flyoutPage)
            {
                flyoutPage.Detail = new NavigationPage(flyoutPage1Detail);
                flyoutPage.IsPresented = false; // Close the menu if it's open
            }
            else
            {
                // Create a new FlyoutPage and set both Flyout and Detail
                var newFlyoutPage = new FlyoutPage
                {
                    Detail = new NavigationPage(flyoutPage1Detail),
                    Flyout = new MenuPage() // Assuming MenuPage is your flyout menu
                };
                Application.Current.MainPage = newFlyoutPage;
            }
        }

       /* public async Task NavigateToNewNotePage(AppNote selectedNote)
        {
            if (selectedNote != null)
            {
                var newNoteVM = new NewNoteViewModel(this, selectedNote);
                var newNotePage = new NewNotePage(newNoteVM);
                newNotePage.BindingContext = newNoteVM;
                if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
                {
                    var navigationPage = mainFlyoutPage.Detail as NavigationPage;
                    await navigationPage?.PushAsync(newNotePage);
                }
            }
        }*/


        public async Task NavigateToNewNotePage(AppNote selectedNote)
        {
            if (!IsEditMode && selectedNote != null)
            {
                if (selectedNote != null)
                {
                    if (_selectionMode == SelectionMode.None)
                    {
                        var newNoteVM = new NewNoteViewModel(this, selectedNote);
                        var newNotePage = new NewNotePage(newNoteVM);
                        newNotePage.BindingContext = newNoteVM;
                        if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
                        {
                            var navigationPage = mainFlyoutPage.Detail as NavigationPage;
                            await navigationPage?.PushAsync(newNotePage);

                        }
                    }
                }
            }
        }

        public async Task NavigateTo(ManageFolders manageFoldersPage)
        {
            if (!(Application.Current.MainPage is NavigationPage))
            {
                Application.Current.MainPage = new NavigationPage(Application.Current.MainPage);
            }

            await Application.Current.MainPage.Navigation.PushAsync(manageFoldersPage);
        }

    }
}