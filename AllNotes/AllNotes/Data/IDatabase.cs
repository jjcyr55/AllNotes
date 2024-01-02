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

    public class IDatabase
    {
        public readonly SQLiteAsyncConnection _database;

        public IDatabase()
        {
           /* var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MyData.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<AppNote>();
            _database.CreateTableAsync<AppFolder>().Wait();*/
        }
    }
}