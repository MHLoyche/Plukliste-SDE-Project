using ClassLibrary.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Frontend.Pages
{
    public class StorageModel : PageModel
    {
        private readonly ProductService _productService; // S�tter privat field til injected service
        public List<Item> Items { get; set; } = new(); // Liste af Items som skal vises p� siden
        public StorageModel(ProductService productService) // Constructor som f�r injected ProductService
        {
            _productService = productService; // S�tter den private field til den injected service
        }

        
        public async Task OnGetAsync() // Metode som bliver kaldt n�r siden loades
        {
            Items = await _productService.GetItemsAsync(); // S�tter items til responsen fra GetItemsAsync
        }
    }
}
