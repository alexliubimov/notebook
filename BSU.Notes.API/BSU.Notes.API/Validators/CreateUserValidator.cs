using BSU.Notes.Data.DTOs.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSU.Notes.API.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("User email is required and should not exceed 100 characters.");

            RuleFor(u => u.UserName)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Username is required and should not exceed 50 characters.");
        }        
    }
}
