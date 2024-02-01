using AllNotes.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AllNotes.Models
{
    public class AppNote : BaseViewModel
    {
        [AutoIncrement, PrimaryKey]
        public int id { get; set; }

        public int folderID { get; set; }

        public string Title { get; set; }
      //  public int ParentFolderId { get; set; }
        public string Text { get; set; }
        public string Text1 { get; set; }
        public string Date { get; set; }
        public int Color { get; set; }

        // public bool IsFavorite { get; set; }


        public string PreviewText { get; set; }
        public int FontSize { get; set; }
        public string FontAttributes { get; set; } // Example: "Bold, Italic"
        public string TextAlignment { get; set; } // Example: "Left", "Center", "Right"
                                                  //public int Color { get; set; } // Assuming color is stored as an integer
    
       /* private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                    OnIsCheckedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }*/
        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (_isFavorite != value)
                {
                    _isFavorite = value;
                    OnPropertyChanged(nameof(IsFavorite));
                   MessagingCenter.Send(this, "NoteFavoriteStatusChanged", this);
                }
            }
        }




        private ObservableCollection<AppNote> _selectedNotes;
        private Action _refreshSelectedNotes;

        public void Initialize(ObservableCollection<AppNote> selectedNotes, Action refreshSelectedNotes)
        {
            _selectedNotes = selectedNotes;
            _refreshSelectedNotes = refreshSelectedNotes;
        }
        private void UpdateSelectedNotesCollection()
        {
            if (_isSelected)
            {
                if (!_selectedNotes.Contains(this))
                    _selectedNotes.Add(this);
            }
            else
            {
                if (_selectedNotes.Contains(this))
                    _selectedNotes.Remove(this);
            }

            _refreshSelectedNotes?.Invoke();
        }

        /* private bool _isSelected;
         public bool IsSelected
         {
             get => _isSelected;
             set
             {
                 if (_isSelected != value)
                 {
                     _isSelected = value;
                     Debug.WriteLine($"Note {this.Title} selection changed to {value}");
                     OnPropertyChanged(nameof(IsSelected));
                 }
             }
         }*/


        /* private bool _isSelected;
         public bool IsSelected
         {
             get => _isSelected;
             set
             {
                 if (_isSelected != value)
                 {
                     _isSelected = value;
                     OnPropertyChanged(nameof(IsSelected));

                     Debug.WriteLine($"Note {Title} IsSelected changed to {value}");
                     OnIsCheckedChanged?.Invoke(this, EventArgs.Empty);
                     MessagingCenter.Send(this, "NoteSelectionChanged", this);

                     if (_isSelected)
                         MessagingCenter.Send(this, "AddToSelectedNotes", this);
                     else
                         MessagingCenter.Send(this, "RemoveFromSelectedNotes", this);
                 }
             }
         }*/

      

        [Ignore]
        public bool IsChecked { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                 //   OnIsCheckedChanged?.Invoke(this, EventArgs.Empty);
                 //   Debug.WriteLine($"Note {Title} IsSelected changed to {value}");
                 //   MessagingCenter.Send(this, "NoteSelectionChanged", this);
                    if (_isSelected)
                        MessagingCenter.Send(this, "AddToSelectedNotes", this);
                    else
                        MessagingCenter.Send(this, "RemoveFromSelectedNotes", this);
                }
            }
        }
        public event EventHandler OnIsCheckedChanged;


        /*private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    Debug.WriteLine($"Note {Title} IsSelected changed to {value}");

                    // Notify the ViewModel about the selection change
                    MessagingCenter.Send(this, "NoteSelectionChanged", this);
                }
            }
        }*/
        [Ignore]
        public List<AppNote> Notes { get; set; } = new List<AppNote>();

        public string Content
        {
            get
            {
                // Directly use the Date string without conversion
                return $"Title: {Title}\n\n{Text}\n\nDate: {Date}";
            }
        }

        public AppNote()
        {
            // Parameterless constructor for SQLite
        }



        /*public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    Debug.WriteLine($"IsSelected changed: {value}");
                    OnPropertyChanged(nameof(IsSelected));

                    if (value)
                        MessagingCenter.Send(this, "AddToSelectedNotes", this);
                    else
                        MessagingCenter.Send(this, "RemoveFromSelectedNotes", this);
                }
            }
        }*/
        /*private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));

                    if (_isSelected)
                        MessagingCenter.Send(this, "AddToSelectedNotes", this);
                    else
                        MessagingCenter.Send(this, "RemoveFromSelectedNotes", this);
                }
            }
        }*/





        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
                //  OnPropertyChanged(nameof(ShowSelectionCircle));
            }
        }
        private bool _isInEditMode;
        public bool IsInEditMode
        {
            get => _isInEditMode;
            set
            {
                if (_isInEditMode != value)
                {
                    _isInEditMode = value;
                    OnPropertyChanged(nameof(IsInEditMode));
                }
            }
        }
        public bool IsNotEditMode => !IsEditMode;
        // public bool ShowSelectionCircle => IsEditMode;

        public bool ShouldShowCheckmark => IsSelected && IsEditMode;


        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private bool _isLongPressed;
        public bool IsLongPressed
        {
            get => _isLongPressed;
            set
            {
                if (_isLongPressed != value)
                {
                    _isLongPressed = value;
                    OnPropertyChanged(nameof(IsLongPressed));
                }
            }
        }

        /*public ICommand ToggleSelectionCommand { get; set; }

        public AppNote()
        {
            ToggleSelectionCommand = new Command(() => IsSelected = !IsSelected);
        }*/
        /* public ICommand ToggleSelectionCommand { get; private set; }

         public AppNote()
         {
             ToggleSelectionCommand = new Command(() =>
             {
                 IsSelected = !IsSelected;
                 Debug.WriteLine($"Note toggled: {IsSelected}");
             });
         }*/
        public ICommand ToggleSelectionCommand { get; private set; }

        public AppNote(object data1, object data)
        {
            ToggleSelectionCommand = new Command(() =>
            {
                IsSelected = !IsSelected;
            });
        }
        private bool _isCheckboxVisible;


        public bool IsCheckboxVisible
        {
            get => _isCheckboxVisible;
            set
            {
                if (_isCheckboxVisible != value)
                {
                    _isCheckboxVisible = value;
                    OnPropertyChanged(nameof(IsCheckboxVisible));
                }
            }
        }


        public void UpdateCheckboxVisibility(bool isEditMode)
        {
            IsCheckboxVisible = isEditMode;
        }


        /*public AppNote()
        {
           
        }*/

        /* public AppNote()
         {

         }*/
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler SelectionChanged;

        public AppNote(string text)
        {
            this.Text = text;
            this.Title = text;
        }
    }
}