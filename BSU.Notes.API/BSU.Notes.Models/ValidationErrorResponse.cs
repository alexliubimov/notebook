using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSU.Notes.Models
{
    public class ValidationErrorResponse : ProblemDetails
    {
        public ValidationErrorResponse(string title, string detail, List<ValidationError> validationErrors,
            string type = null, string instance = null)
        {
            Title = title;
            Detail = detail;
            ValidationErrors = validationErrors;
            Type = type;
            Instance = instance;
        }
        public List<ValidationError> ValidationErrors { get; set; }
    }
}
