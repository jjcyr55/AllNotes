using AllNotes.Models;
using AllNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllNotes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageFolders : ContentPage
    {
        private ManageFoldersViewModel _manageFoldersViewModel;
        private ManageFoldersViewModel manageFoldersViewModel;
        private ManageFolders _manageFolders;

        public ManageFoldersViewModel ManageFoldersViewModel { get; set; }
        public ManageFolders(ManageFoldersViewModel manageFoldersViewModel)
        {
            InitializeComponent();
            BindingContext = new ManageFoldersViewModel();

            _manageFoldersViewModel = new ManageFoldersViewModel(this);
            BindingContext = _manageFoldersViewModel;
            

        }
        private void ToggleFolderDirectly(object sender
, EventArgs e)
        {
            if (sender is Image image && image.BindingContext is AppFolder folder)
            {
                folder.IsExpanded = !folder.IsExpanded;

           
                    // This line is necessary to refresh the ListView
        foldersListView.ItemsSource = null;
                foldersListView.ItemsSource = _manageFoldersViewModel.FolderList;
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Subscribe to messages when the page appears
            MessagingCenter.Subscribe<ManageFoldersViewModel, AppFolder>(this, "SubfolderAddedInMenuPage", (sender, newSubfolder) =>
            {
                // Handle the message
            });
            var viewModel = BindingContext as ManageFoldersViewModel;

            if (viewModel != null && viewModel.IsMoveNoteContext)
            {
                // Show alert only in the context of moving notes
                DisplayAlert("Move Notes", "Please select a folder to move the notes to.", "OK");
            }
        }
       
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Unsubscribe from messages when the page disappears
            MessagingCenter.Unsubscribe<ManageFoldersViewModel, AppFolder>(this, "SubfolderAddedInMenuPage");
        }
        /*private void OnFolderSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is AppFolder selectedFolder)
            {
                ((ManageFoldersViewModel)BindingContext).SelectedFolder = selectedFolder;
            }
    // Optionally, you can deselect the item immediately
    ((ListView)sender).SelectedItem = null;
        }
    }*/

        private void OnFolderSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                ((ListView)sender).SelectedItem = null; // Manually deselect the item
            }
        }
    }
    }