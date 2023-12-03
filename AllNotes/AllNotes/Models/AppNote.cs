using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllNotes.Models
{
    public class AppNote
    {
        [AutoIncrement, PrimaryKey]
        public int id { get; set; }

        public int folderID { get; set; }

        public string description { get; set; } = "";

        public string dateTime { get; set; } = "";

        public AppNote()
        {

        }

        public AppNote(string text)
        {
            this.description = text;
        }
    }
}
