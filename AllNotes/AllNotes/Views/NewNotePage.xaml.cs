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
        public NewNoteViewModel _newNoteViewModel;
        public int folderID { get; set; }
        int selectedFolderID = 0;
        public NewNotePage(NewNoteViewModel newNoteViewModel)
        {
            folderID = selectedFolderID;
            InitializeComponent();
              NavigationPage.SetHasNavigationBar(this, false);
            _newNoteViewModel = newNoteViewModel;
          //  editor.SetBinding(EditorNoUnderline.AlignmentProperty, "TextAlignment");
            BindingContext = newNoteViewModel;
        }
        public NewNotePage(int folderId)
        {
            folderID = selectedFolderID;
            InitializeComponent();
            BindingContext = new NewNoteViewModel();
        }

        public NewNotePage()
        {
        }

        /* protected override bool OnBackButtonPressed()
         {
             var viewModel = BindingContext as NewNoteViewModel;
             if (viewModel != null)
             {
                 // Run the SaveNote method and wait for it to complete
                 Task.Run(() => viewModel.SaveNote()).Wait();
             }

             return base.OnBackButtonPressed();
         }*/
       

        /*public NewNotePage(int folderId)
        {
            InitializeComponent();

            if (folderId == 0)
            {
                folderId = GetDefaultFolderId(); // Implement GetDefaultFolderId to retrieve a valid default ID
            }

            BindingContext = new NewNoteViewModel(folderId);
        }*/
    }
}