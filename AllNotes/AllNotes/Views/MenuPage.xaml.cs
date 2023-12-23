using AllNotes.Models;
using AllNotes.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllNotes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public CollectionView MenuContent { get { return menuContent; } }
        private MenuPageViewModel _viewModel;
        private MenuPage _menuPage;
        private MenuPageViewModel viewModel;


        public MenuPageViewModel MenuPageViewModel { get; set; }


        public MenuPage()
        {
            InitializeComponent();
            BindingContext = new MenuPageViewModel();

            _viewModel = new MenuPageViewModel(this);
            BindingContext = _viewModel;
            UpdateMenu();

        }



        public void UpdateMenu()
        {
            // Update the menu UI as needed
            // For example, you can reassign the ItemsSource of the CollectionView
            // to reflect the updated FolderList from the ViewModel.
            menuContent.ItemsSource = _viewModel.FolderList;

        }
        private async void OnGoToFlyoutPage1DetailClicked(object sender, EventArgs e)
        {


            // Assuming FlyoutPage1Detail is a ContentPage
            // var flyoutPage1Detail = new FlyoutPage1Detail();
            // await Navigation.PushAsync(flyoutPage1Detail);
        }
        /*private async void menuItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count != 0)
            {
                // Get the note model
                var folder = (AppFolder)e.CurrentSelection[0];

                // Your existing logic...
            }*/
        // }

    }
}
/*private MenuPageViewModel _viewModel;
private FlyoutPage2Flyout _menuPage;
private MenuPageViewModel viewModel;
public MenuPageViewModel MenuPageViewModel { get; set; }
public FlyoutPage2Flyout()
{
    InitializeComponent();
    _viewModel = new MenuPageViewModel(this);
    BindingContext = _viewModel;
    UpdateMenu();
   
}


public void UpdateMenu()
{
    // Update the menu UI as needed
    // For example, you can reassign the ItemsSource of the CollectionView
    // to reflect the updated FolderList from the ViewModel.
    menuContent.ItemsSource = _viewModel.FolderList;
}
private async void menuItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
{
    if (e.CurrentSelection.Count != 0)
    {
        // Get the note model
        var folder = (AppFolder)e.CurrentSelection[0];

        // Your existing logic...
    }
}

    }
}*/