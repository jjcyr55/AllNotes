using AllNotes.Database;
using AllNotes.Models;
using AllNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
//using TEditor;
//using TEditor.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllNotes.Views
{
	/*[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FullScreenEditorPage : ContentPage
	{



        public string Date { get; set; }
        public int folderID { get; set; }
        public ObservableCollection<int> NoteColors { get; set; }
        public Collection<int> FontSizes { get; set; }

        private AppDatabase _database;
        private MainPageViewModel _mainPageViewModel;
        private MenuPageViewModel _menuPageViewModel;

        private AppNote _note;
        int selectedFolderID = 0;
        AppNote selectedNote = null;
        private FontAttributes _fontAttribute = FontAttributes.None;
        private MenuViewModel _menuViewModel;




       
        private string _htmlContent;
        private MainPageViewModel mainPageViewModel;
        private AppNote note;

        public string NewNoteTitle { get; set; }

        public string HtmlContent
        {
            get => _htmlContent;
            set
            {
                _htmlContent = value;
                OnPropertyChanged(nameof(HtmlContent));
            }
        }
        public FullScreenEditorPage (string initialHtml)
		{
            _mainPageViewModel = mainPageViewModel;
            _note = note;

            if (_note != null)
            {
                // Existing note - initialize with its content
                NewNoteTitle = note.Title;
                HtmlContent = note.Text; // Ensure this is the correct property for HTML content
            }
            else
            {
                // New note - initialize as necessary
                NewNoteTitle = "";
                HtmlContent = ""; // Set as blank or a default value if needed
            }




            InitializeComponent ();
		}

     //   public string HtmlContent { get; private set; }

        public async Task OpenTEditor()
        {
            var toolbar = new ToolbarBuilder().AddBasic().AddH1().AddH2().AddH3().AddH4().AddH5().AddH6().AddBold().AddItalic().AddUnderline().AddJustifyLeft().AddAll(); // and so on
                                                                                                                                                                          // TEditorResponse response = await CrossTEditor.Current.ShowTEditor("<p>Initial content</p>", toolbar);
            TEditorResponse response = await CrossTEditor.Current.ShowTEditor(HtmlContent, toolbar);



            if (!string.IsNullOrEmpty(response.HTML))
            {
                HtmlContent = response.HTML;
                SaveNote();
            }
        }
        private async void SaveNote()
        {
            // Check if the HTML content is null or empty instead
            if (string.IsNullOrEmpty(HtmlContent))
            {
                return; // Return if the HTML content is null or empty
            }

            var db = AppDatabase.Instance();
            string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (_note == null) // If creating a new note
            {
                AppNote note = new AppNote
                {
                    folderID = selectedFolderID,
                    Text = HtmlContent, // Use HtmlContent here
                    Title = NewNoteTitle,
                    Date = currentDateTime,
                   // Color = NewNoteColor,
                  //  FontSize = FontSize,
                    // Other properties as needed
                };

                await db.InsertNote(note); // Insert the new note
                MessagingCenter.Send(this, "RefreshNotes");
            }
            else // If updating an existing note
            {
                _note.Text = HtmlContent; // Update the note's text with HtmlContent
                                          // Update other properties as needed
                await db.UpdateNote(_note); // Update the existing note

                MessagingCenter.Send(this, "NoteUpdated", _note);
            }

            // Navigation logic after saving
            if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
            {
                var navigationPage = mainFlyoutPage.Detail as NavigationPage;
                await navigationPage?.PopAsync();
                //  _mainPageViewModel.RefreshNotes(); // Ensure this refreshes the notes list
            }
        }

    }*/
}