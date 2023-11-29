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
using AllNotes.Services;
using AllNotes.Interfaces;
using System.Diagnostics;

namespace AllNotes
{
    public partial class MainPage : ContentPage
    {
        private List<Note> listNotes;

        private string SearchKeyword;
        private Entry SearchBox;
       
        private MainPageViewModel _mainPageViewModel;
        private object notes;

        // List<Note> listNotes;
        public MainPage()
        {
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
         //   stackTop.IsVisible = true;
            //  IEnumerable<Note> notes = await App.Database.GetNotes();
            // listNotes = notes.ToList();
           
            listNotes.Reverse();
         
        }
        public MainPage(MainPageViewModel mainPageViewModel)
        {
            InitializeComponent();
           NavigationPage.SetHasNavigationBar(this, false);
            _mainPageViewModel = new MainPageViewModel();
            listNotes = new List<Note>();
            BindingContext = _mainPageViewModel;
          



            // string dbPath = DependencyService.Get<IFileHelper>().GetLocalFilePath("MyData.db");
            // viewModel = new MainViewModel(dbPath);
            // BindingContext = viewModel;
            // When you want to show all notes
            //  NotesCV.ItemsSource = _mainPageViewModel.Notes;

            // When you want to show filtered notes
            //   NotesCV.ItemsSource = _mainPageViewModel.FilteredNotes;

        }

       /* public void searchBarElement_TextChanged(object sender, TextChangedEventArgs e)
        {
            base.OnAppearing();

          //  stackTop.IsVisible = false;
          //  frameTop.IsVisible = false;
            SearchBar searchBar = (SearchBar)sender;
          

          //  _mainPageViewModel.SearchKeyword = e.NewTextValue;
          //  _mainPageViewModel.SearchNotes();

            // Set the ItemsSource of the CollectionView to the filtered notes
            NotesCV.ItemsSource = _mainPageViewModel.FilteredNotes;
           
        }
       */

        protected override bool OnBackButtonPressed()
        {
            if (_mainPageViewModel.MultiSelectEnabled)
                _mainPageViewModel.ShowOrHideToolbar();

            return true;
        }


        /*  private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
          {
              base.OnAppearing();
              stackTop.IsVisible = false;
              frameTop.IsVisible=false;
              SearchBar searchBar = (SearchBar)sender;
             // listNotes = await App.Database.SearchNoteAsync(searchBar.Text);
             // listViewNotes.ItemsSource = listNotes;
          }*/
        private void SearchBar_Tapped(object sender, EventArgs e)
        {
         //   stackTop.IsVisible = false;
         //   frameTop.IsVisible = false;
        }

        private void BtnOption_Clicked(object sender, EventArgs e)
        {

        }

       /* private void searchBarElement_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
*/
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchKeyword = e.NewTextValue;
            Debug.WriteLine($"Search keyword entered: {searchKeyword}");
            _mainPageViewModel.SearchNotes(searchKeyword);
            // base.OnAppearing();

            // stackTop.IsVisible = false;
            //   frameTop.IsVisible = false;
            // SearchBar searchBar = (SearchBar)sender;
            //  await _mainPageViewModel.SearchNotes(searchBar.Text);

            //  _mainPageViewModel.SearchKeyword = e.NewTextValue;
            //  _mainPageViewModel.SearchNotes();

            // Set the ItemsSource of the CollectionView to the filtered notes
            //  NotesCV.ItemsSource = _mainPageViewModel.FilteredNotes;
        }
    }
}