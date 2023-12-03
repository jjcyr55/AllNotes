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

namespace AllNotes.Services
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
            try
            {
                // Use await to asynchronously execute the query
                var results = await _database.Table<Note>().ToListAsync();

                // Return the results
                return results;
            }
            catch (Exception ex)
            {
                // Handle exceptions here, log or rethrow if necessary
                return new List<Note>(); // Return an empty list in case of an error
            }
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
    }
}