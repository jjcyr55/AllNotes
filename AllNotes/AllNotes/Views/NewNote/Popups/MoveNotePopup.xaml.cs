using AllNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllNotes.Views.NewNote.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MoveNotePopup : ContentPage 
    {
        private MoveNotePopupViewModel _moveNotePopupViewModel;

        public MoveNotePopup()
        {
        }

        /* public MoveNotePopup()
{
}*/
        /* BindingContext = new ManageFoldersViewModel();

         _manageFoldersViewModel = new ManageFoldersViewModel(this);
         BindingContext = _manageFoldersViewModel;*/

        /* public MoveNotePopup(MoveNotePopupViewModel moveNotePopupViewModel)
         {
             InitializeComponent();
             BindingContext = new MoveNotePopupViewModel();
             _moveNotePopupViewModel = new MoveNotePopupViewModel(this);
             BindingContext = _moveNotePopupViewModel;
             MessagingCenter.Subscribe<MoveNotePopupViewModel>(this, "CloseModal", async (sender) =>
             {
                 await Navigation.PopModalAsync();
             });

             Task.Run(async () =>
             {
                 await Task.Delay(500); // Ensure UI is ready
                 await Application.Current.MainPage.DisplayAlert("Select Folder", "Please select a folder to move the note(s) to.", "OK");
             });
         }*/
        public MoveNotePopup(MoveNotePopupViewModel moveNotePopupViewModel)
        {
            InitializeComponent();
            BindingContext = new MoveNotePopupViewModel();
            _moveNotePopupViewModel = new MoveNotePopupViewModel(this);
            BindingContext = _moveNotePopupViewModel;
            BindingContext = moveNotePopupViewModel;
            /* BindingContext = new MoveNotePopupViewModel();

             _moveNotePopupViewModel = new MoveNotePopupViewModel(this);
             BindingContext = _moveNotePopupViewModel;*/
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Unsubscribe to prevent memory leaks
            MessagingCenter.Unsubscribe<MoveNotePopupViewModel>(this, "CloseModal");
        }
        /* public MoveNotePopup(MoveNotePopupViewModel viewModel)
         {
             InitializeComponent();
             BindingContext = viewModel;
         }*/
    }
}