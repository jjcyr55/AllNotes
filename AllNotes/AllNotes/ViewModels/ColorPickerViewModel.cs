using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AllNotes.ViewModels
{
    public class ColorPickerViewModel : BaseViewModel
    {
        public ICommand SelectColorCommand { get; }

        public ColorPickerViewModel()
        {
            SelectColorCommand = new Command<string>(SelectColor);
        }

        private void SelectColor(string color)
        {
            // Convert string to Color if necessary
            // Notify the NewNoteViewModel about the color selection
            MessagingCenter.Send(this, "ColorSelected", color);
        }
        public ICommand SetTextColorRedCommand => new Command(() =>
        {
            MessagingCenter.Send(this, "ExecuteJavaScript", "setTextColor('#FF0000')");
        });

        public ICommand SetTextColorGreenCommand => new Command(() =>
        {
            MessagingCenter.Send(this, "ExecuteJavaScript", "setTextColor('#00FF00')");
        });

        public ICommand SetTextColorBlueCommand => new Command(() =>
        {
            MessagingCenter.Send(this, "ExecuteJavaScript", "setTextColor('#0000FF')");
        });
    }
    }