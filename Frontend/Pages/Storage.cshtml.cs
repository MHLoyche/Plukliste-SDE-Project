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
            Items = await _productService.GetItemsAsync(); // Sætter items til responsen fra GetItemsAsync
        }
    }
}
