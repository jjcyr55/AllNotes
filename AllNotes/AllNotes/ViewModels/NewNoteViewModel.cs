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
        public string NewNoteDate { get; set; }
        public ObservableCollection<int> NoteColors { get; set; }
        public Collection<int> FontSizes { get; set; }

        private NoteRepository _noteRepository;
        private MainPageViewModel _mainPageViewModel;
        private Note _note;

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

        public NewNoteViewModel(MainPageViewModel mainPageViewModel, Note note)
        {
            NewNoteColor = (int)Colors.Gray;
            NoteColors = new ObservableCollection<int> {
                ((int)Colors.Yellow),
                ((int)Colors.Green),
                ((int)Colors.Blue),
                ((int)Colors.Purple),
                ((int)Colors.Pink),
                ((int)Colors.Red),
                ((int)Colors.Orange),
                ((int)Colors.Gray),
            };
            FontSizes = new Collection<int> { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 22, 24, 26, 28, 30, 32, 36, 40 };
            NewNoteTitle = "";
            NewNoteDate = DateTime.Now.ToString();
            _noteRepository = new NoteRepository();
            _mainPageViewModel = mainPageViewModel;
            _note = note;

            if (_note != null)
            {
                NewNoteTitle = note.Title;
                NewNoteText = note.Text;
                NewNoteDate = note.Date;
                NewNoteColor = note.Color;
            }
        }

        private async void SaveNote()
        {
            if (NewNoteText != null && _note == null)
            {
                await _noteRepository.CreateNote(NewNoteTitle, NewNoteText, NewNoteDate, NewNoteColor);
                _mainPageViewModel.GetNotesFromDb();
            }

            if (_note != null)
            {
                await _noteRepository.EditNote(_note.Id, NewNoteTitle, NewNoteText, NewNoteDate, NewNoteColor);
                _mainPageViewModel.GetNotesFromDb();
            }

            await Application.Current.MainPage.Navigation.PopAsync();
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