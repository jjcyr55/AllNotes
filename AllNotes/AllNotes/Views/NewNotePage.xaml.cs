using AllNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllNotes.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AllNotes.ViewModels;
using Syncfusion.XForms.RichTextEditor;

namespace AllNotes.Views.NewNote
{
    public partial class NewNotePage : ContentPage
    {
        public int FolderID { get; set; } // If needed for specific logic

        // Constructor for a new note in a specific folder
        public NewNotePage(int folderId)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = new NewNoteViewModel(folderId);
        }

        // Constructor for an existing note
        public NewNotePage(NewNoteViewModel newNoteViewModel)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = newNoteViewModel;
        }

        // Default constructor for a new note without specifying a folder
        public NewNotePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = new NewNoteViewModel();
        }

        // Optional: Override the back button behavior if needed
        /*protected override bool OnBackButtonPressed()
        {
            var viewModel = BindingContext as NewNoteViewModel;
            if (viewModel != null)
            {
                Task.Run(() => viewModel.SaveNote()).Wait();
            }
            return base.OnBackButtonPressed();
        }*/
    }
}