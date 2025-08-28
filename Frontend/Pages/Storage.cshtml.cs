using ClassLibrary.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Frontend.Pages
{
    public class StorageModel : PageModel
    {
        private readonly ProductService _productService;
        public List<Item> Items { get; set; }
        public StorageModel(ProductService productService)
        {
            _productService = productService;
        }

        // Når siden loades hentes Items
        public async Task OnGetAsync()
        {
            Items = await _productService.GetItemsAsync();
        }
    }
}
