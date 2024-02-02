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
           
            BindingContext = new MainPageViewModel();
          
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
