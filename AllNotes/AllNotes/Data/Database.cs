using AllNotes.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Essentials;

namespace AllNotes.Data
{
    
    public class Database
    {


        public readonly SQLiteAsyncConnection _database;

        public const string DatabaseFileName = "MyData.db";

        public const SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist          
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache; 
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);

        public Database()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MyData.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Note>();
            
        }
    }
}
