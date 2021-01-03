using Microsoft.AspNetCore.Mvc;

namespace BSU.Notes.Models
{
    public class ErrorResponse : ProblemDetails
    {
        public ErrorResponse(string title, string detail, string type = null, string instance = null)
        {
            Title = title;
            Detail = detail;
            Type = type;
            Instance = instance;
        }
    }
}
