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
    public partial class EditFolderPopup : Popup
    {
        public EditFolderPopup()
        {
            InitializeComponent();
        }

      //  void OnRenameClicked(object sender, EventArgs e) => Close("rename");

      //  void OnDeleteClicked(object sender, EventArgs e) => Close("delete");

        void OnCancelClicked(object sender, EventArgs e) => Close("cancel");

        private void Close(string v)
        {
            throw new NotImplementedException();
        }
    }
}