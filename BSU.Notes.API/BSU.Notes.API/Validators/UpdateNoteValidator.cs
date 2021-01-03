using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSU.Notes.Data.DTOs.Note;
using FluentValidation;

namespace BSU.Notes.API.Validators
{
    public class UpdateNoteValidator : AbstractValidator<UpdateNoteDto>
    {
        public UpdateNoteValidator()
        {
            RuleFor(n => n).SetValidator(new CreateNoteValidator());
        }
    }
}
