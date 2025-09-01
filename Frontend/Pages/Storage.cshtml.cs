using ClassLibrary.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Frontend.Pages
{
    public class StorageModel : PageModel
    {
        private readonly ProductService _productService; // Sætter privat field til injected service
        public List<Item> Items { get; set; } = new(); // Liste af Items som skal vises på siden
        public StorageModel(ProductService productService) // Constructor som får injected ProductService
        {
            _productService = productService; // Sætter den private field til den injected service
        }

        public async Task OnGetAsync() // Metode som bliver kaldt når siden loades
        {

            ApiResponse<List<Item>> response = await _productService.GetItemsAsync(); // Kalder GetItemsAsync på ProductService
            if (response.Success && response.Data != null) // Hvis kaldet lykkedes og der er data
            {
                Items = response.Data; // Sætter Items til den hentede data
            }
            else
            {
                // Håndter fejl, f.eks. log fejlbesked eller vis notifikation
                Console.WriteLine($"Error fetching items: {response.Message}");
                ViewData["status"] = $"Error fetching items: {response.Message}"; // Sætter fejlbesked i ViewData for visning på siden
            }
        }
    }
}
