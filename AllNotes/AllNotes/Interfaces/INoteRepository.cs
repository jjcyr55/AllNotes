using AllNotes.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AllNotes.Interfaces
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetNotes();

        Task<Note> GetNoteById(int id);

        Task CreateNote(string title, string text, string date, int color);

        Task DeleteNote(Note note);

        Task EditNote(int id, string title, string text, string date, int color);
        Task<IEnumerable<Note>> SearchNotesAsync(string searchKeyword);
    }
}
