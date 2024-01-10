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
    public partial class NewNotePopup : Popup
    {

       // public CollectionView MenuContent { get { return MenuCollectionView; } }
        private MenuViewModel _viewModel1;
        private NewNotePopup _menuPage1;
        private MenuViewModel viewModel;


        public MenuViewModel MenuViewModel { get; set; }


        public NewNotePopup()
        {
            InitializeComponent();
            //  this.BindingContext = new MenuViewModel();

            /* BindingContext = new MenuViewModel();

             _viewModel1 = new MenuViewModel(this);
             BindingContext = _viewModel1;*/
        //    this.BindingContext = new MenuViewModel();


        }
    }
}