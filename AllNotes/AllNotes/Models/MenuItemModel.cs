using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AllNotes.Models
{
    public class MenuItemModel
    {
        public string Title { get; set; }
        public ImageSource Icon { get; set; }
        public Action Action { get; set; }
        public string iconPath { get; set; }
    }
}
