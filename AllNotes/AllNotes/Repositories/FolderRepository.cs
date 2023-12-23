using AllNotes.Interfaces;
using AllNotes.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AllNotes.Repositories
{
    public class FolderRepository // : IFolderRepository
    {
        /* private static FolderRepository _instance;
         private readonly SQLiteAsyncConnection _database; // this might need to be uncommented = App.Database;
         public FolderRepository()
         {
             _database = App.Database;
         }


         public static FolderRepository Instance
         {
             get
             {
                 if (_instance == null)
                 {
                     _instance = new FolderRepository();
                 }
                 return _instance;
             }
         }

         public async Task InitializeDefaultFolder()
         {
             var firstFolder = await GetFirstFolder();
             if (firstFolder == null)
             {
                 var defaultFolder = new AppFolder { Name = "Default Folder", IconPath = "folder_account_outline.png" *//* other properties *//* };
                 await InsertFolder(defaultFolder);
             }
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
         }*/

    }


}
