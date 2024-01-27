﻿using System;
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
using Syncfusion.XForms.RichTextEditor;
using AllNotes.Converters;

//using TEditor.Abstractions;
//using TEditor;

namespace AllNotes.ViewModels
{
    public class NewNoteViewModel : BaseViewModel
    {

        public ICommand MenuItemSelectedCommand { get; }
       public ICommand SaveNoteCommand => new Command(SaveNote);
        public ICommand OpenMenuCommand => new Command(OpenMenu);

        
       

       
        public ICommand BoldTextCommand => new Command(BoldText);

        public ICommand ShareNoteCommand => new Command(ShareNote);





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

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                _isFavorite = value;
                OnPropertyChanged(nameof(IsFavorite));
            }
        }
        public Action BackgroundColorAction { get; set; }
        public Action ShareAction { get; set; }


        public ICommand ToggleFavoriteCommand => new Command(ToggleFavorite);


        /* public ICommand ToggleFavoriteCommand => new Command(() =>
         {
             IsFavorite = !IsFavorite;
         });*/

        /* public int NewNoteColor
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
         }*/

        public ICommand BackgroundColorActionCommand => new Command(ExecuteBackgroundColorAction);
        public ICommand ShareActionCommand => new Command(ExecuteShareAction);

        public NewNotePage BindingContext { get; private set; }

        public ICommand MenuItemSelectedCommandd { get; }


        public NewNoteViewModel()
        {
        }


            public NewNoteViewModel(MainPageViewModel mainPageViewModel, AppNote note)
        {

            //    BackgroundColorAction = () => { /* Logic for Background Color */ };
            //   ShareAction = () => { /* Logic for Share */ };




            //  InitializeMenuItems();
            //THE UNSELECT AND DEVICE BACK BUTTON NEED TO BE HANDLED TO STOP NULL REFERENCE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            /*MessagingCenter.Subscribe<NewNoteViewModel, int>(this, "NoteUpdated", (sender, folderId) =>
            {
                _menuPageViewModel.UpdateNoteCountForFolder(folderId);
            });*/



            _mainPageViewModel = mainPageViewModel;
            _note = note;

            if (_note != null)
            {
                // Existing note - initialize with its content
                NewNoteTitle = note.Title;
                HtmlContent = note.Text; // Ensure this is the correct property for HTML content
                NewNoteColorValue = note.Color;
            }
            else
            {
                // New note - initialize as necessary
                NewNoteTitle = "";
                HtmlContent = ""; // Set as blank or a default value if needed
                NewNoteColorValue= 0;
            }



            MenuItemSelectedCommand = new Command<MenuItemModel>(ExecuteMenuItem);
            MenuItems = new ObservableCollection<MenuItemModel>
         {
        //    new MenuItemModel { Title = "Background Color", Type = MenuType.ColorPicker },
         //    new MenuItemModel { Title = "Share", CommandAction = ShareNote },
         //    new MenuItemModel {  Icon = "heart.png", CommandAction = ToggleFavorite }, // Update this line
             
         };

            //   new MenuItemModel { Title = "Favorite", Command = new Command(FavoriteNote) }

            NewNoteColorValue = (int)Colors.Orange;
            NoteColors = new ObservableCollection<int> {
                ((int)Colors.Yellow),
                ((int)Colors.Green),
                ((int)Colors.Blue),
                ((int)Colors.Purple),
                ((int)Colors.Pink),
                ((int)Colors.Red),
                ((int)Colors.Orange),
                ((int)Colors.White),
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
                NewNoteColorValue = note.Color;
                folderID = note.folderID;
                selectedNote = note;
                selectedFolderID = note.folderID;
            }
        }
       /* private void InitializeMenuItems()
        {
            MenuItemSelectedCommand = new Command<MenuItemModel>(ExecuteMenuItem);
            MenuItems = new ObservableCollection<MenuItemModel>
            
            {
                 new MenuItemModel { Title = "Share", CommandAction = ShareNote },
            // new MenuItemModel { Title = "Background Color", Command = ChangeNoteColorCommand },
           //   new MenuItemModel { Title = "Share", CommandAction = ShareNote },
            // Other dynamic menu items...

        };
        }*/

        private void FavoriteNote(object obj)
        {
            throw new NotImplementedException();
        }

        //OLD MENU STILL WANT TO USE BUT HAVE TO WORK ON IT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        private void OpenMenu()
        {
            var newNotePopup = new NewNotePopup();
            newNotePopup.BindingContext = this;
            Application.Current.MainPage.Navigation.ShowPopup(newNotePopup);
        }
        private void ExecuteBackgroundColorAction()
        {
            // Assuming BackgroundColorAction is an Action you've defined
            BackgroundColorAction?.Invoke();
        }

        private void ExecuteShareAction()
        {
            ShareAction?.Invoke();
        }
        private void ExecuteMenuItem(MenuItemModel item)
        {
            item?.CommandAction?.Invoke();
        }
       
        private async void ToggleFavorite()
        {
            IsFavorite = !IsFavorite;

            if (_note != null)
            {
                _note.IsFavorite = IsFavorite;
                await AppDatabase.Instance().UpdateNote(_note); // Update the note in the database
                MessagingCenter.Send(this, "NoteUpdated", _note.folderID);
            }
            else if (!string.IsNullOrEmpty(HtmlContent))
            {
                SaveNote(); // Save the new note with the favorite status
            }
            // Send a message to refresh the notes list in the main view
            MessagingCenter.Send(this, "RefreshNotes");
        }
        /*private async void ShareNote()
        {
            try
            {
                // Assuming CurrentNoteContent holds the text of the note being edited or created
                var noteContent = NewNoteText;
                var noteTitle = NewNoteTitle;
                var noteColor = NewNoteColorValue;

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
        }*/



        private async void ShareNote()
        {
            try
            {
                // Save and share logic
                if (_note == null || string.IsNullOrWhiteSpace(HtmlContent))
                {
                    var action = await Application.Current.MainPage.DisplayAlert("Save and Share",
                                      "The note must be saved before sharing. Save and share now?",
                                      "Save and Share", "Cancel");

                    if (action)
                    {
                        await SaveNoteInternal(); // Use an internal save method

                        // Check if the note now has content after saving
                        if (!string.IsNullOrWhiteSpace(HtmlContent))
                        {
                            await Share.RequestAsync(new ShareTextRequest
                            {
                                Text = HtmlContent, // Assuming HtmlContent has the note text
                                Title = NewNoteTitle
                            });
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Share", "There is nothing to share.", "OK");
                        }
                        return;
                    }
                }

                // Proceed with sharing if there is content
                if (!string.IsNullOrWhiteSpace(HtmlContent))
                {
                    await Share.RequestAsync(new ShareTextRequest
                    {
                        Text = HtmlContent,
                        Title = NewNoteTitle
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in sharing: {ex.Message}");
            }
        }

        // Internal method for saving a note
        private async Task SaveNoteInternal()
        {
            if (string.IsNullOrEmpty(HtmlContent))
            {
                return;
            }

            var db = AppDatabase.Instance();
            string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (_note == null) // Creating a new note
            {
                _note = new AppNote
                {
                    folderID = selectedFolderID,
                    Text = HtmlContent,
                    Title = NewNoteTitle,
                    Date = currentDateTime,
                    IsFavorite = IsFavorite
                };
                await db.InsertNote(_note);
            }
            else // Updating an existing note
            {
                _note.Text = HtmlContent;
                await db.UpdateNote(_note);
            }

            MessagingCenter.Send(this, "RefreshNotes");
            // Other logic after saving...
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

        public NewNoteViewModel(NavigationService navigationService, AppNote note)
        {
        }

        /*public NewNoteViewModel()
        {
        }
*/




        /*private async void SaveNote()
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
                   // Color = NewNoteColorValue,
                  //  FontSize = FontSize,
                    IsFavorite = IsFavorite,
                    
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
                MessagingCenter.Send<NewNoteViewModel, int>(this, "NoteUpdated", _note.folderID);
                MessagingCenter.Send(this, "NoteUpdated", _note);
            }

            // Navigation logic after saving
            if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
            {
                var navigationPage = mainFlyoutPage.Detail as NavigationPage;
                await navigationPage?.PopAsync();
               
                //  _mainPageViewModel.RefreshNotes(); // Ensure this refreshes the notes list

            }
        }*/
        private async void SaveNote()
        {
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
                    Text = HtmlContent,
                    Title = NewNoteTitle,
                    Date = currentDateTime,
                    IsFavorite = IsFavorite,
                    // Other properties as needed
                };

                await db.InsertNote(note); // Insert the new note
                MessagingCenter.Send(this, "RefreshNotes");
                MessagingCenter.Send<NewNoteViewModel, int>(this, "NoteUpdated", selectedFolderID); // Notify MenuPageViewModel
            }
            else // If updating an existing note
            {
                _note.Text = HtmlContent;
                await db.UpdateNote(_note); // Update the existing note
                MessagingCenter.Send<NewNoteViewModel, int>(this, "NoteUpdated", _note.folderID);
                MessagingCenter.Send(this, "NoteUpdated", _note);
            }

            // Navigation logic after saving
            if (Application.Current.MainPage is FlyoutPage mainFlyoutPage)
            {
                var navigationPage = mainFlyoutPage.Detail as NavigationPage;
                await navigationPage?.PopAsync();
            }
        }


        private string _htmlContent;
        public string HtmlContent
        {
            get => _htmlContent;
            set
            {
                if (_htmlContent != value)
                {
                    _htmlContent = value;
                    OnPropertyChanged(nameof(HtmlContent));
                }
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
        private string _editorContent;
        public string EditorContent
        {
            get { return _editorContent; }
            set
            {
                _editorContent = value;
                OnPropertyChanged(nameof(EditorContent));
            }
        }







        /*private string ConvertFontAttributesToString(FontAttributes fontAttributes)
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
        }*/


       
        public void UpdateEditorBackgroundColor(Color newColor)
        {
            // Convert Color to HEX
            var hexColor = newColor.ToHex();

            // CSS to change background color
            string css = $"<style>body{{background-color:{hexColor};}}</style>";

            // Prepend CSS to HtmlContent
            HtmlContent = css + HtmlContent;

            // Notify the view that HtmlContent has changed
            OnPropertyChanged(nameof(HtmlContent));
        }
      //  public ICommand ChangeNoteColorCommand => new Command<int>(ChangeNoteColor);

        /* private void ChangeNoteColor(int colorValue)
         {
             Color newColor = Color.FromUint((uint)colorValue);
           NewNoteColorValue = newColor; // This should be fine if NewNoteColor is of type Color
             UpdateEditorBackgroundColor(newColor);
         }*/
       /* private void ChangeNoteColor(int colorValue)
        {
            // Convert colorValue to Color
            Color newColor = Color.FromUint((uint)colorValue);

            // Assign the integer color value
            NewNoteColorValue = colorValue;

            // Update the background color of the editor
            UpdateEditorBackgroundColor(newColor);
        }*/



        private int _newNoteColorValue;
        public int NewNoteColorValue
        {
            get => _newNoteColorValue;
            set
            {
                _newNoteColorValue = value;
                OnPropertyChanged(nameof(NewNoteColorValue));
                OnPropertyChanged(nameof(NewNoteColorValue)); // Update the UI color as well
            }
        }

        public Color NewNoteColor => ColorConverter.ConvertColor(NewNoteColorValue);






        private void ChangeNoteColor(object o)
        {
            int color = (int)o;
            NewNoteColorValue = color;
          //  UpdateEditorBackgroundColor(newColor);
        }
       /* private void OpenMenu()
        {
            var noteColorSelectionPopup = new NoteColorSelectionPopup();
            noteColorSelectionPopup.BindingContext = this;
            Application.Current.MainPage.Navigation.ShowPopup(noteColorSelectionPopup);
        }*/
        private void BoldText()
        {
            if (FontAttribute != FontAttributes.Bold)
                FontAttribute = FontAttributes.Bold;
            else
                FontAttribute = FontAttributes.None;
        }

       

       
    }
}