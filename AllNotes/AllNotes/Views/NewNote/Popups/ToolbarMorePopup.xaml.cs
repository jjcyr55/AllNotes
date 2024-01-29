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
    public partial class ToolbarMorePopup : Popup
    {
        public ToolbarMorePopup()
        {
            InitializeComponent();
        }
    }
}