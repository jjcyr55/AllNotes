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
    public partial class FlyoutPage1 : FlyoutPage
    {
        public FlyoutPage1()
        {
        }

        public FlyoutPage1(MainPageViewModel mainPageViewModel)
        {
            InitializeComponent();
            /*MainPageViewModel viewModel = new MainPageViewModel();
            MenuPageViewModel menuPageViewModel = new MenuPageViewModel();
            NewNoteViewModel newNoteViewModel = new NewNoteViewModel();
            FlyoutPage1Detail flyoutPage1Detail = new FlyoutPage1Detail();
            flyoutPage1Detail.BindingContext = viewModel;
            MenuPage menuPage = new MenuPage();
            menuPage.BindingContext = menuPageViewModel;*/
            BindingContext = new MainPageViewModel();
            /*BindingContext = new MainPageViewModel();
            BindingContext = new MenuPageViewModel();
            BindingContext = new NewNoteViewModel();*/
            //Save below comments for context for knowing what to look for and add in menu
            // FlyoutPage.ListView.ItemSelected += ListView_ItemSelected;
            this.Flyout = new MenuPage(); // Your menu page
            this.Detail = new NavigationPage(new FlyoutPage1Detail());

        }

       /* private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as FlyoutPage1FlyoutMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;*/

          //  FlyoutPage.ListView.SelectedItem = null;
        }
    }
