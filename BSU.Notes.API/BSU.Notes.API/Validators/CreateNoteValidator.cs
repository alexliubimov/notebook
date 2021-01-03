using BSU.Notes.Data.DTOs.Note;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSU.Notes.API.Validators
{
    public class CreateNoteValidator : AbstractValidator<CreateNoteDto>
    {
        public CreateNoteValidator()
        {
            RuleFor(n => n.Title)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Note title is required and should not exceed 200 characters.");

            RuleFor(n => n.Content)
                .NotEmpty()
                .WithMessage("Note content is required.");
        }
    }
}
