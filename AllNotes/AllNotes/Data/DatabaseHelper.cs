using AllNotes.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllNotes.Data
{
    public class DatabaseHelper
    {
       /* SQLiteConnection database;

        public DatabaseHelper(string dbPath)
        {
            database = new SQLiteConnection(dbPath);
            database.CreateTable<Note>();
        }

        public List<Note> SearchNotes(string keyword)
        {
            return database.Table<Note>().Where(n => n.Title.Contains(keyword) || n.Content.Contains(keyword)).ToList();
        }
*/
        // Add other CRUD operations as needed
    }
}