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
    public partial class ParentFolderPopup : Popup
    {
        public ParentFolderPopup(Models.AppFolder folder)
        {
            InitializeComponent();
           var viewModel = new ParentFolderPopupViewModel(folder);
            this.BindingContext = viewModel;
        }
        void OnRenameTapped(object sender, EventArgs e) => Dismiss("rename");

        void OnDeleteTapped(object sender, EventArgs e) => Dismiss("delete");

        void OnAddSubfolderTapped(object sender, EventArgs e) => Dismiss("cancel");
    }
}