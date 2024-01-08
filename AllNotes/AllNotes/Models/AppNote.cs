using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AllNotes.Models
{
    public class AppNote
    {
        [AutoIncrement, PrimaryKey]
        public int id { get; set; }

        public int folderID { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }
        public string Date { get; set; }
        public int Color { get; set; }

        /*public int FontAttribute { get; set; } // stored as int
        public int FontSize { get; set; } // or int
        public int TextAlignment { get; set; }*/
        public int FontSize { get; set; }
        public string FontAttributes { get; set; } // Example: "Bold, Italic"
        public string TextAlignment { get; set; } // Example: "Left", "Center", "Right"
        

        //  public Type TargetPage { get; set; }

        public AppNote()
        {

        }

        public AppNote(string text)
        {
            this.Text = text;
            this.Title = text;
        }
    }
}
