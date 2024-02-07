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
      //  public ICommand SelectColorCommand { get; }
       
        public ColorPickerViewModel()
        {
           // SelectColorCommand = new Command<string>(SelectColor);

           // SetTextColorRedCommand = new Command(() => ExecuteJavaScript("setTextColor('#FF0000')"));
          //  SetTextColorGreenCommand = new Command(() => ExecuteJavaScript("setTextColor('#00FF00')"));
         //   SetTextColorBlueCommand = new Command(() => ExecuteJavaScript("setTextColor('#0000FF')"));
        }

        public ICommand SetTextColorCommand => new Command<string>(color =>
        {
            MessagingCenter.Send(this, "ColorChanged", color);
        });
        private void SelectColor(string color)
        {
            // Convert string to Color if necessary
            // Notify the NewNoteViewModel about the color selection
            MessagingCenter.Send(this, "ColorSelected", color);
        }
        private void ExecuteJavaScript(string script)
        {
            MessagingCenter.Send(this, "ExecuteJavaScript", script);
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