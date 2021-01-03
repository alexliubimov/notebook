using BSU.Notes.Data;
using BSU.Notes.Data.DTOs.Note;
using System.Threading.Tasks;

namespace BSU.Notes.Services.Interfaces
{
    public interface INoteService
    {
        Task<ItemsResponseModel<NoteDto>> GetNotes(int userId, int? page, int? size);
        Task<NoteDto> GetNote(int userId, int noteId);
        Task<int> CreateNote(int userId, CreateNoteDto note);
        Task UpdateNote(int userId, int noteId, UpdateNoteDto note);
        Task DeleteNote(int userId, int noteId);
    }
}
