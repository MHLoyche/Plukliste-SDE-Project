using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class StorageModel : PageModel
    {
        /*public async Task OnGetAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://localhost:7148/storage/items";

                try
                {
                    // Send GET request
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Ensure success (throws exception if not 200-299)
                    response.EnsureSuccessStatusCode();

                    // Read response content as string
                    string responseBody = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(responseBody);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Request error: {ex.Message}");
                }
            }
        }*/
    }
}
