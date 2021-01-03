using Ardalis.GuardClauses;
using BSU.Notes.Data.DTOs.Note;
using BSU.Notes.Models;
using BSU.Notes.Models.Exceptions;
using BSU.Notes.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSwag.Annotations;
using BSU.Notes.Data;
using System.ComponentModel;

namespace BSU.Notes.API.Controllers
{
    [Route("api/users/{userId}/notes")]
    [ApiController]
    [OpenApiTag("Notes", Description = "Endpoints to work with notes data.")]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly IValidator<CreateNoteDto> _createNoteValidator;
        private readonly IValidator<UpdateNoteDto> _updateNoteValdator;

        public NotesController(INoteService noteService, IValidator<CreateNoteDto> createNoteValidator, IValidator<UpdateNoteDto> updateNoteValidator)
        {
            _noteService = noteService ?? throw new ArgumentException(nameof(noteService));
            _createNoteValidator = createNoteValidator ?? throw new ArgumentException(nameof(createNoteValidator));
            _updateNoteValdator = updateNoteValidator ?? throw new ArgumentException(nameof(updateNoteValidator));
        }

        [HttpGet]
        [Description("Gets list of notes for given user. Supports pagination")]
        [SwaggerResponse(StatusCodes.Status200OK, typeof(ItemsResponseModel<NoteDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, typeof(string), Description = "Invalid parameters")]
        [SwaggerResponse(StatusCodes.Status404NotFound, typeof(string), Description = "User not found")]
        public async Task<IActionResult> GetNotes(int userId, [FromQuery] int? page, [FromQuery] int? size)
        {
            try
            {
                Guard.Against.NegativeOrZero(userId, nameof(userId));

                return Ok(await _noteService.GetNotes(userId, page, size));
            }
            catch (ArgumentException ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status400BadRequest);
            }
            catch (NotFoundException ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status500InternalServerError,
                    "An error occurred when attempting to retrieve notes.");
            }
        }

        [HttpGet("{noteId}")]
        [Description("Gets note data.")]
        [SwaggerResponse(StatusCodes.Status200OK, typeof(NoteDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, typeof(string), Description = "Invalid parameters")]
        [SwaggerResponse(StatusCodes.Status404NotFound, typeof(string), Description = "Note not found")]
        public async Task<IActionResult> GetNote(int userId, int noteId)
        {
            try
            {
                Guard.Against.NegativeOrZero(userId, nameof(userId));
                Guard.Against.NegativeOrZero(noteId, nameof(noteId));

                return Ok(await _noteService.GetNote(userId, noteId));
            }
            catch (ArgumentException ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status400BadRequest);
            }
            catch (NotFoundException ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status500InternalServerError,
                    "An error occurred when attempting to retrieve note data.");
            }
        }

        [HttpPost]
        [Description("Creates new note.")]
        [SwaggerResponse(StatusCodes.Status200OK, typeof(int))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, typeof(string), Description = "Invalid parameters")]
        [SwaggerResponse(StatusCodes.Status404NotFound, typeof(string), Description = "User not found")]
        public async Task<IActionResult> CreateNote(int userId, [FromBody] CreateNoteDto note)
        {
            try
            {
                Guard.Against.NegativeOrZero(userId, nameof(userId));

                var validationResult = await ValidateCreateNote(note);
                if (validationResult.ValidationErrors.Any())
                {
                    return BadRequest(validationResult);
                }

                return Ok(await _noteService.CreateNote(userId, note));
            }
            catch (ArgumentException ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status400BadRequest);
            }
            catch (NotFoundException ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status500InternalServerError,
                    "An error occurred when attempting to create a new note.");
            }
        }

        [HttpPut("{noteId}")]
        [Description("Updates existing note.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, typeof(EmptyResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, typeof(string), Description = "Invalid parameters")]
        [SwaggerResponse(StatusCodes.Status404NotFound, typeof(string), Description = "User or note not found")]
        public async Task<IActionResult> UpdateNote(int userId, int noteId, [FromBody] UpdateNoteDto note)
        {
            try
            {
                Guard.Against.NegativeOrZero(userId, nameof(userId));
                Guard.Against.NegativeOrZero(noteId, nameof(noteId));

                var validationResult = await ValidateUpdateNote(note);
                if (validationResult.ValidationErrors.Any())
                {
                    return BadRequest(validationResult);
                }

                await _noteService.UpdateNote(userId, noteId, note);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status400BadRequest);
            }
            catch (NotFoundException ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status500InternalServerError,
                    "An error occurred when attempting to update a note.");
            }
        }

        [HttpDelete("{noteId}")]
        [Description("Deletes existing note.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, typeof(EmptyResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, typeof(string), Description = "Invalid parameters")]
        [SwaggerResponse(StatusCodes.Status404NotFound, typeof(string), Description = "User or note not found")]
        public async Task<IActionResult> DeleteNote(int userId, int noteId)
        {
            try
            {
                Guard.Against.NegativeOrZero(userId, nameof(userId));
                Guard.Against.NegativeOrZero(noteId, nameof(noteId));

                await _noteService.DeleteNote(userId, noteId);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status400BadRequest);
            }
            catch (NotFoundException ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return GetErrorResponse(ex, StatusCodes.Status500InternalServerError,
                    "An error occurred when attempting to delete a note.");
            }
        }

        private ObjectResult GetErrorResponse(Exception exception, int statusCode, string message = null)
        {
            var errorResponse = new ErrorResponse("Notes API", message);
            return StatusCode(statusCode, errorResponse);
        }

        private async Task<ValidationErrorResponse> ValidateCreateNote(CreateNoteDto note)
        {
            ValidationErrorResponse validationErrorResponse = new ValidationErrorResponse(null, null, new List<ValidationError>());
            var validation = _createNoteValidator.Validate(note);

            if (validation?.IsValid == false)
            {
                AddValidationErrors(validationErrorResponse, validation.Errors);
            }

            return validationErrorResponse;
        }

        private async Task<ValidationErrorResponse> ValidateUpdateNote(UpdateNoteDto note)
        {
            ValidationErrorResponse validationErrorResponse = new ValidationErrorResponse(null, null, new List<ValidationError>());
            var validation = _updateNoteValdator.Validate(note);

            if (validation?.IsValid == false)
            {
                AddValidationErrors(validationErrorResponse, validation.Errors);
            }

            return validationErrorResponse;
        }

        private static void AddValidationErrors(ValidationErrorResponse validationErrorResponse, IList<ValidationFailure> failures)
        {
            validationErrorResponse.ValidationErrors.AddRange(failures.Select(e => new ValidationError
            {
                Name = e.PropertyName,
                Description = e.ErrorMessage
            }));
        }
    }
}
