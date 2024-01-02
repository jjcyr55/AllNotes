using AllNotes.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AllNotes.Interfaces
{
    public interface INoteRepository
    {
        //Task<IEnumerable<Note>> GetNotes(int folderId);
       /* Task<IEnumerable<Note>> GetNotes(int selectedFolder);

        Task<Note> GetNoteById(int id);

        Task CreateNote(string title, string text, string date, int FolderId);

        Task DeleteNote(Note note);

        Task EditNote(int id, string title, string text, string date, int _selectedFolderId);
        Task<IEnumerable<Note>> SearchNotesAsync(string searchKeyword);
        Task<IEnumerable<Note>> GetNotesInFolder(int folderId);//Get Note List

        Task<IEnumerable<AppFolder>> GetFolderList();
        Task<AppFolder> GetFirstFolder();
        Task<AppFolder> GetFolder(int id);
        Task UpdateFolder(AppFolder folder);
        Task InsertFolder(AppFolder folder);
        Task DeleteFolder(AppFolder folder);

        Task InitializeDefaultFolder();*/
    }
}
