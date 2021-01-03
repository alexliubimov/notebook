using System;
using System.Collections.Generic;
using System.Text;

namespace BSU.Notes.Data.DTOs.Note
{
    public class NoteDto : UpdateNoteDto
    {
        public int NoteId { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }
    }
}
