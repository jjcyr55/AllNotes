using AllNotes.Models;
//using AllNote.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.CommunityToolkit.Core;
using System.Data.Common;
using AllNotes.ViewModels;
using AllNotes.Views.NewNote;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace AllNotes.Database
{
    public static class Constants
    {
        public const string DatabaseFilename = "FastNoteApp.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }


    public class AppDatabase
    {
        public static AppDatabase m_instance = null;
        public static AppDatabase Instance()
        {
            if (m_instance == null) m_instance = new AppDatabase();

            return m_instance;
        }

        //SQLite database = null;
        SQLiteConnection dbConnection = null;

        public AppDatabase()
        {
        }

       

        public void Init()
        {
            if (dbConnection is not null)
                return;

            dbConnection = new SQLiteConnection(Constants.DatabasePath, Constants.Flags);
            dbConnection.CreateTable<AppFolder>();
            dbConnection.CreateTable<AppNote>();

            // Ensure "Default Folder" exists
            var defaultFolder = dbConnection.Table<AppFolder>().FirstOrDefault(f => f.Name == "Default Folder");
            if (defaultFolder == null)
            {
                InsertFolder(new AppFolder("Default Folder", "folder_account_outline.png"));
            }
            // Ensure "Default Folder" exists
            EnsureDefaultFolderExists();

            // Ensure "Archive" folder exists
            EnsureArchiveFolderExists();
        }
        public List<AppFolder> GetFolderList()
        {
            // Your existing logic to get the list
            return dbConnection.Table<AppFolder>().ToList() ?? new List<AppFolder>(); // Assuming LINQ to SQL/EF or similar
        }
        public bool IsFolderArchive(int folderId)
        {
            // Assuming "Archive" is a special name for the Archive folder
            var folder = dbConnection.Table<AppFolder>().FirstOrDefault(f => f.Id == folderId);
            return folder != null && folder.Name == "Archive";
        }
        private void EnsureDefaultFolderExists()
        {
            var defaultFolder = dbConnection.Table<AppFolder>().FirstOrDefault(f => f.Name == "Default Folder");
            if (defaultFolder == null)
            {
                InsertFolder(new AppFolder { Name = "Default Folder", IconPath = "folder_account_outline.png" });
            }
        }

        private void EnsureArchiveFolderExists()
        {
            var archiveFolder = dbConnection.Table<AppFolder>().FirstOrDefault(f => f.Name == "Archive");
            if (archiveFolder == null)
            {
                InsertFolder(new AppFolder { Name = "Archive", IconPath = "archive_folder.png" });
                // Note: You can customize 'IconPath' with an appropriate icon for the Archive folder.
            }
        }
           /* public List<AppFolder> GetFolderList()
        {
            return dbConnection.Table<AppFolder>().ToList();
        }*/
      
        public async Task<AppFolder> GetFirstFolder()
        {
            return dbConnection.Table<AppFolder>().FirstOrDefault();
        }

        public AppFolder GetFolder(int id)
        {
            return dbConnection.Table<AppFolder>().Where(folder => folder.Id == id).FirstOrDefault();
        }   

        public int UpdateFolder(AppFolder folder)
        {
            return dbConnection.Update(folder);
        }

        public int InsertFolder(AppFolder folder)
        {
            return dbConnection.Insert(folder);
        }

        public int DeleteFolder(AppFolder folder)
        {
            return dbConnection.Delete(folder);
        }

        public List<AppNote> GetNoteList(int folderID)
        {
            return dbConnection.Table<AppNote>().Where(note => note.folderID == folderID).ToList();
        }


        /*public async Task<int> UpdateNote(int noteId, string title, string text, string date, int color)
        {
            var note = dbConnection.Find<AppNote>(noteId); // Use FindAsync
            if (note != null)
            {
                note.Title = title;
                note.Text = text;
                note.Date = date;
                note.Color = color;

                return dbConnection.Update(note); // Use UpdateAsync
            }
            return 0;
        }*/
        public async Task<int> UpdateNote(AppNote note)
        {
            // Assuming note is an instance of AppNote with all the updated properties
            return dbConnection.Update(note); // Use UpdateAsync if using async
        }
        

        /*public async Task<int> InsertNote(int folderId, string title, string text, string date, int color)
        {
            var note = new AppNote
            {
                folderID = folderId,
                Title = title,
                Text = text,
                Date = date,
                Color = color
            };

            var folder = GetFolder(note.folderID);
            int count = Convert.ToInt32(folder.noteCount) + 1;
            folder.noteCount = count.ToString();

            dbConnection.Update(folder);
            return dbConnection.Insert(note);
        }*/
        public async Task<int> InsertNote(AppNote note)
        {
            // Assuming note is an instance of AppNote with all the properties set
            return dbConnection.Insert(note); // Use InsertAsync if using async
        }


        public int DeleteNote(AppNote note)
        {
            AppFolder folder = GetFolder(note.folderID);

            int count = Convert.ToInt32(folder.noteCount) - 1;
            if (count < 0) count = 0;
            folder.noteCount = count.ToString();

            dbConnection.Update(folder);

            return dbConnection.Delete(note);
        }

        public void DeleteAllNotes(int folderID)
        {
            List<AppNote> noteList = GetNoteList(folderID);

            foreach (AppNote note in noteList)
            {
                dbConnection.Delete(note);
            }
        }
        public AppNote GetNoteById(int noteId)
        {
            return dbConnection.Find<AppNote>(noteId); // Synchronously finds an AppNote with the specified ID
        }

    }
}