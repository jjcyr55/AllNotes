using SQLite;
using System;

namespace AllNotes.Models
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        public string Description { get; set; } = "";
        public string Date {  get; set; }

        //   public DateTime Date { get; set; }
        public string NoteBody { get; set; } = ""; // Add this line


        public bool IsChecked { get; set; }

        public string HexColor { get; set; } = "#000000";
        public double Size { get; set; } = 12;

        public string Text { get; set; }

        public int Color { get; set; }
        public string Content { get; set; }
        public string IconPath { get; set; }
        public DateTime CreatedAt { get; set; }

       
    }
}