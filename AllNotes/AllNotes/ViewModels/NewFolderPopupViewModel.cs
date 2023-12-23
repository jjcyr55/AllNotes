using AllNotes.Views;
using AllNotes.Views.NewNote.Popups;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace AllNotes.ViewModels
{
    public class NewFolderPopupViewModel : BaseViewModel
    {
       /* private string _newFolderName;
        private MenuPageViewModel _viewModel;


        public string NewFolderName
        {
            get => _newFolderName;
            set => SetProperty(ref _newFolderName, value);
        }

        public ICommand OkCommand => new Command(AddNewFolder);

        public NewFolderPopupViewModel(MenuPageViewModel viewModel)
        {
            _viewModel = viewModel;

        }

        public NewFolderPopupViewModel()
        {
        }


        //  public ICommand AddNewFolderCommand => new Command(AddNewFolder);
        private async void AddNewFolder()
        {
            if (_viewModel != null)
            {
                // Call the AddNewFolder method in MenuPageViewModel
                _viewModel.AddNewFolderAsync(NewFolderName);

                // Close the popup or perform other actions if needed
                // ...
                var newFolderPopup = new NewFolderPopup();
                newFolderPopup.BindingContext = this;
                //  Application.Current.MainPage.Navigation.ShowPopup(newFolderPopup);
                //  await App.Current.MainPage.Navigation.ShowPopupAsync(newFolderPopup);
                var result = await App.Current.MainPage.Navigation.ShowPopupAsync(newFolderPopup);
            }
        }

        //private Popup<object> popup;

        public ICommand CancelCommand => new Command(OnCancel);
        private async void OnCancel()
        {
            var newFolderPopup = new NewFolderPopup();
            //var popup = new Popup();
            // var result = await Navigation.ShowPopupAsync(new NewFolderPopupViewModel());
            // Close the XCT popup
            //await Application.Current.MainPage.Navigation.PopAsync();
            // await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
            var result = await App.Current.MainPage.Navigation.ShowPopupAsync(newFolderPopup);
        }



        public ICommand OnOkCommand => new Command(OnOk);
        private async void OnOk()
        {
            if (_viewModel != null)
            {
                var newFolderPopup = new NewFolderPopup();
                newFolderPopup.BindingContext = this;

                await App.Current.MainPage.Navigation.ShowPopupAsync(newFolderPopup);
            }
        }*/
    }
}
