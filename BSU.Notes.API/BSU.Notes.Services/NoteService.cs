using BSU.Notes.Data;
using BSU.Notes.Data.DTOs.Note;
using BSU.Notes.DataAccess.Interfaces;
using BSU.Notes.Models.Exceptions;
using BSU.Notes.Services.Interfaces;
using System.Threading.Tasks;

namespace BSU.Notes.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteProvider _noteProvider;
        private readonly IUserProvider _userProvider;

        public NoteService(IUserProvider userProvider, INoteProvider noteProvider)
        {
            _userProvider = userProvider;
            _noteProvider = noteProvider;
        }

        public async Task<int> CreateNote(int userId, CreateNoteDto note)
        {
            var user = await _userProvider.GetUser(userId);

            if (user == null)
            {
                throw new NotFoundException();
            }

            return await _noteProvider.CreateNote(userId, note);
        }

        public async Task DeleteNote(int userId, int noteId)
        {
            var result = await _noteProvider.DeleteNote(userId, noteId);

            if (result == 0)
            {
                throw new NotFoundException();
            }
        }

        public async Task<NoteDto> GetNote(int userId, int noteId)
        {
            var note = await _noteProvider.GetNote(userId, noteId);

            if (note == null)
            {
                throw new NotFoundException();
            }

            return note;
        }

        public async Task<ItemsResponseModel<NoteDto>> GetNotes(int userId, int? page, int? size)
        {
            var user = await _userProvider.GetUser(userId);

            if (user == null)
            {
                throw new NotFoundException();
            }

            var notesTask = _noteProvider.GetNotes(userId, page, size);

            if (page.HasValue && size.HasValue)
            {
                var countTask = _noteProvider.CountNotes(userId);

                return new ItemsResponseModel<NoteDto>
                {
                    Items = await notesTask,
                    PaginationInfo = new PaginationModel
                    {
                        Page = page.Value,
                        Size = size.Value,
                        TotalRecords = await countTask
                    }
                };
            }

            return new ItemsResponseModel<NoteDto>
            {
                Items = await notesTask,
                PaginationInfo = null
            };
        }

        public async Task UpdateNote(int userId, int noteId, UpdateNoteDto note)
        {
            var result = await _noteProvider.UpdateNote(userId, noteId, note);

            if (result == 0)
            {
                throw new NotFoundException();
            }
        }
    }
}
