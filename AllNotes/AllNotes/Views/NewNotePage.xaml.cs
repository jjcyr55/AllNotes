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

namespace AllNotes.Views.NewNote
{
    public partial class NewNotePage : ContentPage
    {
        public NewNoteViewModel _newNoteViewModel;
        public NewNotePage(NewNoteViewModel newNoteViewModel)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            _newNoteViewModel = newNoteViewModel;
            editor.SetBinding(EditorNoUnderline.AlignmentProperty, "TextAlignment");
        }
    }
}