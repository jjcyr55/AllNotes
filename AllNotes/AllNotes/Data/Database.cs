using AllNotes.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AllNotes.Data
{

    public class Database
    {
        public readonly SQLiteAsyncConnection _database;

        public Database()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MyData.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Note>();
        }
    }
}