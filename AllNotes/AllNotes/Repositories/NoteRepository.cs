using AllNotes.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using AllNotes.Models;

namespace AllNotes.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly SQLiteAsyncConnection _database = App.Database;
    
        public async Task CreateNote(string title, string text, string date, int color)
        {
            var note = new Note
            {
                Title = title,
                Text = text,
                Date = date,
                Color = color
            };
            await _database.InsertAsync(note);
        }

        public async Task DeleteNote(Note note)
        {
            await _database.DeleteAsync(note);
        }

        public async Task EditNote(int id, string title, string text, string date, int color)
        {
            var note = new Note
            {
                Id = id,
                Title = title,
                Text = text,
                Date = date,
                Color = color
            };
            await _database.UpdateAsync(note);
        }

        public async Task<Note> GetNoteById(int id)
        {
           var note = await _database.FindAsync<Note>(id);
            return note;
        }

        public async Task<IEnumerable<Note>> GetNotes()
        {
            var results = await _database.Table<Note>().ToListAsync();
            return results;
        }
    }
}
