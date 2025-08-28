namespace ClassLibrary.Model
{
    public class ApiResponse
    {
        // Indicates if the API call was successful
        public bool Success { get; set; }
        // Message from the API, can be used for error details or other information
        public string Message { get; set; }
    }
}
