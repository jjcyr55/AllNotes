using AllNotes.Interfaces;
using System;
using System.Collections.Generic;
using AllNotes.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using AllNotes.Models;
using System.Linq;
using AllNotes.Repositories;

namespace AllNotes.Services
{
   /* public class NoteRepository : INoteRepository
    {
        private static NoteRepository _instance;
        private readonly SQLiteAsyncConnection _database = App.Database;
        public static NoteRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NoteRepository();
                }
                return _instance;
            }
        }
      

        public async Task DeleteNote(Note note)
        {
            await _database.DeleteAsync(note);
        }

        public async Task EditNote(int id, string title, string text, string date, int _selectedFolderId)
        {
            var note = new Note
            {
                Id = id,
                Title = title,
                Text = text,
                Date = date,
                FolderId = _selectedFolderId,
                
            };
            await _database.UpdateAsync(note);
        }

        public async Task<Note> GetNoteById(int id)
        {
            var note = await _database.FindAsync<Note>(id);
            return note;
        }
        public async Task CreateNote(string title, string text, string date, int folderId)
        {
            // Retrieve the folder using the FolderId
            AppFolder folder = await _database.FindAsync<AppFolder>(folderId);
            if (folder != null)
            {
                // Update the note count of the folder
                int count = Convert.ToInt32(folder.NoteCount) + 1;
                folder.NoteCount = count.ToString();
                await _database.UpdateAsync(folder);
            }

            // Create and insert the note
            var note = new Note
            {
                Title = title,
                Text = text,
                Date = date,
                FolderId = folderId // Assuming your Note class has a FolderId property
            };
            await _database.InsertAsync(note);
        }

        *//* public async Task CreateNote(string title, string text, string date, int FolderId)
         {

             var note = new Note
             {
                 Title = title,
                 Text = text,
                 Date = date,
                 //  FolderId = FolderId,

             };
             await _database.InsertAsync(note);
         }*//*
        public async Task<IEnumerable<Note>> GetNotes(int folderId)
        {
            try
            {
                var results = await _database.Table<Note>().Where(note => note.FolderId == folderId).ToListAsync();
                return results;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return new List<Note>();
            }
        }

        public async Task<IEnumerable<Note>> GetNotesInFolder(int folderId)//Get Note List
        {
            return await _database.Table<Note>().Where(n => n.FolderId == folderId).ToListAsync();
        }

        /// <summary>
        /// Search Note
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Note>> SearchNotesAsync(string searchKeyword)
        {
            try
            {
                string searchNoSpaces = searchKeyword.Replace(" ", "%");

                // Use await to asynchronously execute the query
                var result = await _database.QueryAsync<Note>("SELECT * FROM Notes WHERE Title LIKE ?", "%" + searchNoSpaces + "%");

                // Convert the result to a List<Note> and return it
                return result.ToList();
            }
            catch (Exception ex)
            {
                // Handle exceptions here, log, or rethrow if necessary
                return new List<Note>(); // Return an empty list in case of an error
            }

        }


        *//*public async Task InitializeDefaultFolder()
        {
            var firstFolder = await GetFirstFolder();
            if (firstFolder == null)
            {
                var defaultFolder = new AppFolder { Name = "Default Folder", IconPath = "folder_account_outline.png" *//* other properties *//* };
                await InsertFolder(defaultFolder);
            }
        }*//*
        public async Task InitializeDefaultFolder()
        {
            var firstFolder = await GetFirstFolder();

            if (firstFolder == null)
            {
                var defaultFolder = new AppFolder { Name = "Default Folder", IconPath = "folder_account_outline.png" *//* other properties *//* };
                await InsertFolder(defaultFolder);

                // After inserting the default folder, get it back from the database
                firstFolder = await GetFirstFolder();
                if (firstFolder == null)
                {
                    // Handle the error if the default folder still cannot be retrieved
                    // This might involve logging the error or notifying the user
                    return;
                }
            }

            // Now you can safely use firstFolder, knowing it's not null
        }


        public async Task<IEnumerable<AppFolder>> GetFolderList()
        {
            return await _database.Table<AppFolder>().ToListAsync();
        }
        public async Task<AppFolder> GetFirstFolder()
        {
            return await _database.Table<AppFolder>().FirstOrDefaultAsync();
        }
        public async Task<AppFolder> GetFolder(int id)
        {
            return await _database.FindAsync<AppFolder>(id);
        }
        public async Task UpdateFolder(AppFolder folder)
        {
            await _database.UpdateAsync(folder);
        }
        public async Task InsertFolder(AppFolder folder)
        {
            await _database.InsertAsync(folder);
        }
        public async Task DeleteFolder(AppFolder folder)
        {
            await _database.DeleteAsync(folder);
        }


    
}*/
}