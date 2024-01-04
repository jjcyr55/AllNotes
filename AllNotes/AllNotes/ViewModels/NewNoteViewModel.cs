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

namespace AllNotes.ViewModels
{
    public class NewNoteViewModel : INotifyPropertyChanged
    {
        public ICommand SaveNoteCommand => new Command(SaveNote);
        public ICommand OpenMenuCommand => new Command(OpenMenu);
        public ICommand OpenFontSizePopupCommand => new Command(OpenFontSizePopup);
        public ICommand OpenJustifyTextPopupCommand => new Command(OpenJustifyTextPopup);
        public ICommand ChangeNoteColorCommand => new Command(ChangeNoteColor);
        public ICommand BoldTextCommand => new Command(BoldText);
        public ICommand ItalicizeTextCommand => new Command(ItalicizeText);
        public ICommand ChangeFontSizeCommand => new Command(ChangeFontSize);
        public ICommand AlignTextLeftCommand => new Command(AlignTextLeft);
        public ICommand AlignTextCenterCommand => new Command(AlignTextCenter);
        public ICommand AlignTextRightCommand => new Command(AlignTextRight);

        public string NewNoteTitle { get; set; }
        public string NewNoteText { get; set; }
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

        private int _textAlignment = 0;

        public int TextAlignment
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

        public NewNoteViewModel(MainPageViewModel mainPageViewModel, AppNote note)
        {
           // _menuPageViewModel = menuPageViewModel;
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
            // Assuming NewNoteText is the equivalent of textEditor.Text in your ViewModel
            if (string.IsNullOrEmpty(NewNoteText))
            {
                return; // Return if the note text is null or empty
            }

            var db = AppDatabase.Instance();

            if (_note == null) // If creating a new note
            {
                AppNote note = new AppNote();
                {
                    note.folderID = selectedFolderID; // Assuming folderID is the selected folder ID
                    note.Text = NewNoteText;
                    note.Title = NewNoteTitle;// Assuming NewNoteText is the note's text
                    note.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    // Set other properties like Title, Color if needed
                };
                db = AppDatabase.Instance();
                await db.InsertNote(selectedFolderID, NewNoteTitle, NewNoteText, Date, NewNoteColor);
                MessagingCenter.Send(this, "RefreshNotes");
                // _menuPageViewModel.Reset();
                if (_menuPageViewModel != null)
                {
                    _menuPageViewModel.Reset();
                }
            }
            else // If updating an existing note
            {
                // Assuming the existing note's text or other properties might have changed
                _note.Text = NewNoteText;
                _note.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                // Update other properties if needed
                db = AppDatabase.Instance();
                await db.UpdateNote(_note.id, NewNoteTitle, NewNoteText, Date, NewNoteColor); 
            }
            if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
            {
                var navigationPage = mainFlyoutPage.Detail as NavigationPage;
                await navigationPage?.PopAsync();
            }
           
        }*/
        private async void SaveNote()
        {
            if (string.IsNullOrEmpty(NewNoteText))
                return; // Return if the note text is null or empty

            var db = AppDatabase.Instance();

            if (_note == null) // If creating a new note
            {
                // Handle the case where no specific folder is selected
                if (selectedFolderID == 0)
                    selectedFolderID = GetDefaultFolderId();

                var note = new AppNote
                {
                    folderID = selectedFolderID,
                    Text = NewNoteText,
                    Title = NewNoteTitle,
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    // Set other properties like Color if needed
                };

                await db.InsertNote(selectedFolderID, NewNoteTitle, NewNoteText, Date, NewNoteColor);
                MessagingCenter.Send(this, "RefreshNotes");
                if (_menuPageViewModel != null)
                    _menuPageViewModel.Reset();
            }
            else // If updating an existing note
            {
                _note.Text = NewNoteText;
                _note.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                // Update other properties if needed
                await db.UpdateNote(_note.id, NewNoteTitle, NewNoteText, Date, NewNoteColor);
            }

            if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
            {
                var navigationPage = mainFlyoutPage.Detail as NavigationPage;
                await navigationPage?.PopAsync();
            }
        }

        // Method to get the default folder ID
        private int GetDefaultFolderId()
        {
            var defaultFolder = AppDatabase.Instance().GetFirstFolder(); // Or other logic to determine default
            return defaultFolder?.Id ?? 0; // 0 or another appropriate default
        }
        private void OpenMenu()
        {
            var noteColorSelectionPopup = new NoteColorSelectionPopup();
            noteColorSelectionPopup.BindingContext = this;
            Application.Current.MainPage.Navigation.ShowPopup(noteColorSelectionPopup);
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
            TextAlignment = 1;
        }

        private void AlignTextRight()
        {
            TextAlignment = 2;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}