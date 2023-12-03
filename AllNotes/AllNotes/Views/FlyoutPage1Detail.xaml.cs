using AllNotes.Database;
using AllNotes.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllNotes.Database;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllNotes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlyoutPage1Detail : ContentPage
    {
        public static FlyoutPage1Detail instance = null;

        private List<Note> listNotes;

        private string SearchKeyword;
        private Entry SearchBox;

        private MainPageViewModel _mainPageViewModel;
        private object notes;
        public List<AppNote> noteList { get; set; }


       // AppFolder selectedFolder;
      

        protected override async void OnAppearing()
        {
            base.OnAppearing();
           

            listNotes.Reverse();


            //NotesCV.ItemsSource = null;


            //NotesCV.ItemsSource = noteList;

        }
        
        public FlyoutPage1Detail()
        {
            // This is supposed to go in the parameter:MainPageViewModel mainPageViewModel
            //instance = this;


            InitializeComponent();

           // NavigationPage.SetHasNavigationBar(this, false);
            _mainPageViewModel = new MainPageViewModel();
            listNotes = new List<Note>();
            BindingContext = _mainPageViewModel;

           

        }


        protected override bool OnBackButtonPressed()
        {
            if (_mainPageViewModel.MultiSelectEnabled)
                _mainPageViewModel.ShowOrHideToolbar();

            return true;
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchKeyword = e.NewTextValue;
            Debug.WriteLine($"Search keyword entered: {searchKeyword}");
            _mainPageViewModel.SearchNotes(searchKeyword);
           
        }
    

}
}