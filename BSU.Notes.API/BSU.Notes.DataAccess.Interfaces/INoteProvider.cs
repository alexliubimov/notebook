using System.Collections.Generic;
using System.Threading.Tasks;
using BSU.Notes.Data.DTOs.Note;

namespace BSU.Notes.DataAccess.Interfaces
{
    public interface INoteProvider
    {
        Task<IEnumerable<NoteDto>> GetNotes(int userId, int? page, int? size);
        Task<int> CountNotes(int userId);
        Task<NoteDto> GetNote(int userId, int noteId);
        Task<int> CreateNote(int userId, CreateNoteDto note);
        Task<int> UpdateNote(int userId, int noteId, UpdateNoteDto note);
        Task<int> DeleteNote(int userId, int noteId);
    }
}
