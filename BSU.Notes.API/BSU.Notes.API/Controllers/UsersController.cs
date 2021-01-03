using Ardalis.GuardClauses;
using BSU.Notes.Data;
using BSU.Notes.Data.DTOs.User;
using BSU.Notes.Models;
using BSU.Notes.Models.Exceptions;
using BSU.Notes.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BSU.Notes.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [OpenApiTag("Users", Description = "Endpoints to work with users data.")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<CreateUserDto> _createUserValidator;
        private readonly IValidator<UpdateUserDto> _updateUserValidator;

        public UsersController(IUserService userService, IValidator<CreateUserDto> createUserValidator, IValidator<UpdateUserDto> updateUserValidator)
        {
            _userService = userService ?? throw new ArgumentException(nameof(userService));
            _createUserValidator = createUserValidator ?? throw new ArgumentException(nameof(createUserValidator));
            _updateUserValidator = updateUserValidator ?? throw new ArgumentException(nameof(updateUserValidator));
        }

        [HttpGet]
        [Description("Gets list of users. Supports pagination")]
        [SwaggerResponse(StatusCodes.Status200OK, typeof(ItemsResponseModel<UserDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, typeof(string), Description = "Invalid parameters")]
        public async Task<IActionResult> GetUsers([FromQuery] int? page, [FromQuery] int? size)
        {
            try
            {
                return Ok(await _userService.GetUsers(page, size));
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
                    "An error occurred when attempting to retrieve users.");
            }
        }

        [HttpGet("{userId}")]
        [Description("Gets user data.")]
        [SwaggerResponse(StatusCodes.Status200OK, typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, typeof(string), Description = "Invalid parameters")]
        [SwaggerResponse(StatusCodes.Status404NotFound, typeof(string), Description = "User not found")]
        public async Task<IActionResult> GetUser(int userId)
        {
            try
            {
                Guard.Against.NegativeOrZero(userId, nameof(userId));

                return Ok(await _userService.GetUser(userId));
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
                    "An error occurred when attempting to retrieve user data.");
            }
        }

        [HttpPost]
        [Description("Creates new user.")]
        [SwaggerResponse(StatusCodes.Status200OK, typeof(int))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, typeof(string), Description = "Invalid parameters")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto user)
        {
            try
            {
                var validationResult = await ValidateCreateUser(user);
                if (validationResult.ValidationErrors.Any())
                {
                    return BadRequest(validationResult);
                }

                return Ok(await _userService.CreateUser(user));
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
                    "An error occurred when attempting to create a new user.");
            }
        }

        [HttpPut("{userId}")]
        [Description("Updates existing user.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, typeof(EmptyResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, typeof(string), Description = "Invalid parameters")]
        [SwaggerResponse(StatusCodes.Status404NotFound, typeof(string), Description = "User not found")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto user)
        {
            try
            {
                Guard.Against.NegativeOrZero(userId, nameof(userId));

                var validationResult = await ValidateUpdateUser(user);
                if (validationResult.ValidationErrors.Any())
                {
                    return BadRequest(validationResult);
                }

                await _userService.UpdateUser(userId, user);

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
                    "An error occurred when attempting to update a user.");
            }
        }

        [HttpDelete("{userId}")]
        [Description("Deletes existing user.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, typeof(EmptyResult))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, typeof(string), Description = "Invalid parameters")]
        [SwaggerResponse(StatusCodes.Status404NotFound, typeof(string), Description = "User not found")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                Guard.Against.NegativeOrZero(userId, nameof(userId));

                await _userService.DeleteUser(userId);

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


        private async Task<ValidationErrorResponse> ValidateCreateUser(CreateUserDto user)
        {
            ValidationErrorResponse validationErrorResponse = new ValidationErrorResponse(null, null, new List<ValidationError>());
            var validation = _createUserValidator.Validate(user);

            if (validation?.IsValid == false)
            {
                AddValidationErrors(validationErrorResponse, validation.Errors);
            }

            return validationErrorResponse;
        }

        private async Task<ValidationErrorResponse> ValidateUpdateUser(UpdateUserDto user)
        {
            ValidationErrorResponse validationErrorResponse = new ValidationErrorResponse(null, null, new List<ValidationError>());
            var validation = _updateUserValidator.Validate(user);

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
