using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Frontend.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Disable caching for error responses
    [IgnoreAntiforgeryToken] // Disable antiforgery token validation for the error page
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; } // Property to hold the request identifier for tracking

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // Determine if the RequestId should be shown on the page

        private readonly ILogger<ErrorModel> _logger; // Logger for error tracking and diagnostics

        public ErrorModel(ILogger<ErrorModel> logger) // Constructor with dependency injection for logging
        {
            _logger = logger;
        }

        public void OnGet() // Handle GET requests to the error page 
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }

}
