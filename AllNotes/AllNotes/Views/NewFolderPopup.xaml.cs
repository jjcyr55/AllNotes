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

namespace AllNotes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewFolderPopup : Popup
    {
        public NewFolderPopup()
        {
            InitializeComponent();
            BindingContext = new NewFolderPopupViewModel();
        }

        /* private void Button_Clicked(object sender, EventArgs e)
         {

         }*/
      //  void OnCancelClicked(object sender, EventArgs e) => Close("cancel");

        //  void OnRenameClicked(object sender, EventArgs e) => Close("rename");

        //  void OnDeleteClicked(object sender, EventArgs e) => Close("delete");

        //  void OnCancelClicked(object sender, EventArgs e) => Close("cancel");
    }
}