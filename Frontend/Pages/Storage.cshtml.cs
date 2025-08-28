using ClassLibrary.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Dynamic;


namespace Frontend.Pages
{
    public class StorageModel : PageModel
    {
        private readonly ProductService _productService;

        public StorageModel(ProductService productService)
        {
            _productService = productService;
        }

        public List<Item> Items { get; set; }

        public async Task OnGetAsync()
        {
            Items = await _productService.GetItemsAsync();
        }
    }
}
