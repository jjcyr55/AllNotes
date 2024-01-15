using System;
using System.Windows.Input;
using Xamarin.Forms;
//using AllNotes.Services;
using AllNotes.Enum;
using AllNotes.Models;
using System.Collections.ObjectModel;
using Xamarin.CommunityToolkit.Extensions;
using AllNotes.Views.NewNote.Popups;
using System.ComponentModel;
using AllNotes.Services;
using AllNotes.Views.NewNote;
using AllNotes.Views;
using AllNotes.Interfaces;
using System.Collections.Generic;
using AllNotes.Database;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Diagnostics;
using AllNotes.Data.Enum;
using System.Runtime.InteropServices;
using TEditor;
using TEditor.Abstractions;
//using TEditor.Abstractions;
//using TEditor;

namespace AllNotes.ViewModels
{
    public class NewNoteViewModel : INotifyPropertyChanged
    {

        public ICommand MenuItemSelectedCommand { get; }
        public ICommand SaveNoteCommand => new Command(SaveNote);
        public ICommand OpenMenuCommand => new Command(OpenMenu);

        public ICommand OpenFontSizePopupCommand => new Command(OpenFontSizePopup);
        public ICommand OpenJustifyTextPopupCommand => new Command(OpenJustifyTextPopup);
        // public ICommand ChangeNoteColorCommand => new Command(ChangeNoteColor);

        // public ICommand ChangeNoteColorCommand => new Command(ChangeNoteColor);
        public ICommand ChangeNoteColorCommand => new Command(ChangeNoteColor);
        public ICommand BoldTextCommand => new Command(BoldText);
        public ICommand ItalicizeTextCommand => new Command(ItalicizeText);
        public ICommand ChangeFontSizeCommand => new Command(ChangeFontSize);
        public ICommand AlignTextLeftCommand => new Command(AlignTextLeft);
        public ICommand AlignTextCenterCommand => new Command(AlignTextCenter);
        public ICommand AlignTextRightCommand => new Command(AlignTextRight);





        // public string NewNoteTitle { get; set; }
        // public string NewNoteText { get; set; }
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




        /* public IEnumerable<Colors> GetAllColors()
         {
             return Enum.GetValues(typeof(Colors)).Cast<Colors>();
         }*/
        private string _htmlContent;
        public string HtmlContent
        {
            get => _htmlContent;
            set
            {
                _htmlContent = value;
                OnPropertyChanged(nameof(HtmlContent));
            }
        }



        public FontAttributes FontAttribute
        {
            get => _fontAttribute;
            set
            {
                if (_fontAttribute != value)
                {
                    _fontAttribute = value;
                    OnPropertyChanged(nameof(FontAttribute));
                }
            }
        }

        private TextAlignment _textAlignment; // Assuming TextAlignment is an enum
        public TextAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                if (_textAlignment != value)
                {
                    _textAlignment = value;
                    OnPropertyChanged(nameof(TextAlignment));
                }
            }
        }

        private int _fontSize = 18;

        public int FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged(nameof(FontSize));
                }
            }
        }

        private int _newNoteColor;
        private bool FlyoutPage1Detail;
        private bool _firstNoteCreated;



        public int NewNoteColor
        {
            get => _newNoteColor;
            set
            {
                if (_newNoteColor != value)
                {
                    _newNoteColor = value;
                    OnPropertyChanged(nameof(NewNoteColor));
                }
            }
        }

        public NewNotePage BindingContext { get; private set; }
        //  public ICommand OpenTEditorCommand { get; private set; }
        public ICommand MenuItemSelectedCommandd { get; }
        public NewNoteViewModel(MainPageViewModel mainPageViewModel, AppNote note)
        {

            MessagingCenter.Subscribe<MainPageViewModel>(this, "OpenFullScreenEditor", (sender) =>
            {
                OpenTEditor();
            });



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



            MenuItemSelectedCommand = new Command<MenuItemModel>(ExecuteMenuItem);
            MenuItems = new ObservableCollection<MenuItemModel>
         {
            new MenuItemModel { Title = "Background Color", Type = MenuType.ColorPicker },
             new MenuItemModel { Title = "Share", CommandAction = ShareNote }
         };



            NewNoteColor = (int)Colors.Yellow;
            NoteColors = new ObservableCollection<int> {
                ((int)Colors.Yellow),
                ((int)Colors.Green),
                ((int)Colors.Blue),
                ((int)Colors.Purple),
                ((int)Colors.Pink),
                ((int)Colors.Red),
                ((int)Colors.Orange),
            };
            FontSizes = new Collection<int> { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 22, 24, 26, 28, 30, 32, 36, 40 };
            NewNoteTitle = "";
            Date = DateTime.Now.ToString();
            _database = new AppDatabase();
            _mainPageViewModel = mainPageViewModel;
            _note = note;

            if (_note != null)
            {
                NewNoteTitle = note.Title;
                NewNoteText = note.Text;
                Date = note.Date;
                NewNoteColor = note.Color;
                folderID = note.folderID;
                selectedNote = note;
                selectedFolderID = note.folderID;
            }
        }
      //  public ICommand OpenTEditorCommand => new Command(async () => await OpenTEditor());

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





       /* TEditorResponse response = await CrossTEditor.Current.ShowTEditor("<p>XAM consulting</p>");
         if (!string.IsNullOrEmpty(response.HTML))
             _displayWebView.Source = new HtmlWebViewSource() { Html = response.HTML };*/

        private void OpenMenu()
        {
            var newNotePopup = new NewNotePopup();
            newNotePopup.BindingContext = this;
            Application.Current.MainPage.Navigation.ShowPopup(newNotePopup);
        }

        private void ExecuteMenuItem(MenuItemModel item)
        {
            item?.CommandAction?.Invoke();
        }
        private async void SelectColor()
        {
            // Implement color selection logic here
        }

        private async void ShareNote()
        {
            try
            {
                // Assuming CurrentNoteContent holds the text of the note being edited or created
                var noteContent = NewNoteText;
                var noteTitle = NewNoteTitle;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Share.RequestAsync(new ShareTextRequest
                    {
                        Text = noteContent,
                        Title = noteTitle
                    });
                });
            }
            catch (Exception ex)
            {
                // Log or display the exception message
                Debug.WriteLine($"Error in sharing: {ex.Message}");
            }
        }

        private ObservableCollection<MenuItemModel> _menuItems;
        public ObservableCollection<MenuItemModel> MenuItems
        {
            get => _menuItems;
            set
            {
                _menuItems = value;
                OnPropertyChanged(nameof(MenuItems)); // Assuming INotifyPropertyChanged implementation
            }
        }

        private string _newNoteText;

        public string NewNoteText
        {
            get { return _newNoteText; }
            set
            {
                _newNoteText = value;
                OnPropertyChanged(nameof(NewNoteText));
            }
        }
        private string _newNoteTitle;

        public string NewNoteTitle
        {
            get { return _newNoteTitle; }
            set
            {
                _newNoteTitle = value;
                OnPropertyChanged(nameof(NewNoteTitle));
            }
        }


        public NewNoteViewModel(int folderID)
        {


            selectedFolderID = folderID;


        }
        public NewNoteViewModel(MainPageViewModel mainPageViewModel, int folderID)
        {
            this.folderID = folderID;
        }

        public NewNoteViewModel(MenuPageViewModel menuPageViewModel)
        {
            _menuPageViewModel = menuPageViewModel;
        }

        public NewNoteViewModel()
        {
        }



        /*private async void SaveNote()
        {
            if (string.IsNullOrEmpty(HtmlContent))
            {
                return; // Return if the note text is null or empty
            }
            var noteContent = HtmlContent;

            var db = AppDatabase.Instance();
            string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (_note == null) // If creating a new note
            {
                AppNote note = new AppNote
                {
                    folderID = selectedFolderID,
                    Text = HtmlContent,
                    Title = NewNoteTitle,
                    Date = currentDateTime,
                    Color = NewNoteColor,
                    FontSize = FontSize,
                   // Text1 = HtmlContent
                    *//* FontAttributes = ConvertFontAttributesToString(FontAttribute),
                     TextAlignment = ConvertTextAlignmentToString(TextAlignment)*//*
                };

                await db.InsertNote(note); // Use 'note' here
                MessagingCenter.Send(this, "RefreshNotes");

                if (_menuPageViewModel != null)
                {
                    _menuPageViewModel.Reset();
                }
            }
            else // If updating an existing note
            {
                _note.Text = HtmlContent;
                _note.Title = NewNoteTitle;
                _note.Date = currentDateTime;
                _note.Color = NewNoteColor;
                _note.FontSize = FontSize;
                _note.FontAttributes = ConvertFontAttributesToString(FontAttribute);
                _note.TextAlignment = ConvertTextAlignmentToString(TextAlignment);

                await db.UpdateNote(_note); // Use '_note' here

                // Inform any relevant part of your app that the note has been updated
                MessagingCenter.Send(this, "NoteUpdated", _note);
            }

            if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
            {
                var navigationPage = mainFlyoutPage.Detail as NavigationPage;
                await navigationPage?.PopAsync();
               // _mainPageViewModel.RefreshNotes();
            }
        }*/
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
                    Color = NewNoteColor,
                    FontSize = FontSize,
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

        private string ConvertFontAttributesToString(FontAttributes fontAttributes)
        {
            return fontAttributes.ToString(); // Modify this based on how you want to store the value
        }
        private string ConvertTextAlignmentToString(TextAlignment textAlignment)
        {
            return textAlignment.ToString(); // Modify this based on how you want to store the value
        }
        // Method to get the default folder ID
        private int GetDefaultFolderId()
        {
            var defaultFolder = AppDatabase.Instance().GetFirstFolder(); // Or other logic to determine default
            return defaultFolder?.Id ?? 0; // 0 or another appropriate default
        }


        private void OpenFontSizePopup()
        {
            var fontSizeSelectionPopup = new FontSizeSelectionPopup();
            fontSizeSelectionPopup.BindingContext = this;
            Application.Current.MainPage.Navigation.ShowPopup(fontSizeSelectionPopup);
        }

        private void OpenJustifyTextPopup()
        {
            var justifyTextSelectionPopup = new JustifyTextSelectionPopup();
            justifyTextSelectionPopup.BindingContext = this;
            Application.Current.MainPage.Navigation.ShowPopup(justifyTextSelectionPopup);
        }

        private void ChangeNoteColor(object o)
        {
            int color = (int)o;
            NewNoteColor = color;
        }

        private void BoldText()
        {
            if (FontAttribute != FontAttributes.Bold)
                FontAttribute = FontAttributes.Bold;
            else
                FontAttribute = FontAttributes.None;
        }

        private void ItalicizeText()
        {
            if (FontAttribute != FontAttributes.Italic)
                FontAttribute = FontAttributes.Italic;
            else
                FontAttribute = FontAttributes.None;
        }

        private void ChangeFontSize(object o)
        {
            int fontSize = (int)o;
            FontSize = fontSize;
        }

        private void AlignTextLeft()
        {
            TextAlignment = 0;
        }

        private void AlignTextCenter()
        {
            TextAlignment = TextAlignment.Center; // Using the enum value
        }

        private void AlignTextRight()
        {
            TextAlignment = TextAlignment.End; // Using the enum value
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}