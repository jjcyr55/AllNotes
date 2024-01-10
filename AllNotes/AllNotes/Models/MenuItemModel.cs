using AllNotes.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AllNotes.Models
{
    public class MenuItemModel
    {
        public string Title { get; set; }
        public ImageSource Icon { get; set; }
        public Action Action { get; set; }
        public string iconPath { get; set; }
        public Type TargetType { get; set; }
        public Action CommandAction { get; set; }

        public MenuType Type { get; set; } // Add an enum for MenuType with values like Normal, ColorPicker, etc.
        public ICommand Command { get; set; }
    }
}
