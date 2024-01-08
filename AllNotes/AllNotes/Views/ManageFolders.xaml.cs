using AllNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllNotes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageFolders : ContentPage
    {
        private ManageFoldersViewModel _manageFoldersViewModel;
        public ManageFolders(ManageFoldersViewModel manageFoldersViewModel)
        {
            InitializeComponent();
            _manageFoldersViewModel = manageFoldersViewModel;
            this.BindingContext = _manageFoldersViewModel;
        }
    }
}