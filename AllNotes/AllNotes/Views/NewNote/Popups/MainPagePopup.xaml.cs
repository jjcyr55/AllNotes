using AllNotes.Models;
using AllNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllNotes.Views.NewNote.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPagePopup : Popup
    {


        

        public MainPagePopup()
        {
            InitializeComponent();
            // Create an instance of MainPagePopupViewModel
            var viewModel = new MainPagePopupViewModel();

            // Set the BindingContext of the Popup to the ViewModel
            this.BindingContext = viewModel;

        }

       /* void OnRenameClicked(object sender, EventArgs e) => Dismiss("rename");

        void OnDeleteClicked(object sender, EventArgs e) => Dismiss("delete");

        void OnCancelClicked(object sender, EventArgs e) => Dismiss("cancel");*/
    }
}