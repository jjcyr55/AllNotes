using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AllNotes.Models;
using AllNotes.ViewModels;
using System.Runtime.CompilerServices;
using AllNotes.ViewModels;

namespace AllNotes
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel _mainPageViewModel;

        public MainPage()
        {
        }

        public MainPage(MainPageViewModel mainPageViewModel)
        {
            InitializeComponent();
           NavigationPage.SetHasNavigationBar(this, false);
            _mainPageViewModel = new MainPageViewModel();
            
        }
      

        protected override bool OnBackButtonPressed()
        {
            if (_mainPageViewModel.MultiSelectEnabled)
                _mainPageViewModel.ShowOrHideToolbar();

            return true;
        }
    }
}