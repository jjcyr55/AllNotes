using AllNotes.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllNotes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SecureFolderPopup : Popup
    {
        private MenuPageViewModel _viewModel;
        public SecureFolderPopup(ViewModels.MenuPageViewModel menuPageViewModel)
        {
            InitializeComponent();
            _viewModel = menuPageViewModel;
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            var folderName = folderNameEntry.Text;
            var password = passwordEntry.Text;

            _viewModel.CreateSecureFolder(folderName, password);

            // Close the popup
            Dismiss("save");
        }

        void OnCancelClicked(object sender, EventArgs e) => Dismiss("cancel");

       
    }
}