
using ClassLibrary;
using ClassLibrary.Enums;
using ClassLibrary.Model;
using Frontend.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class StoreModel : PageModel
    {
       
        private readonly ProductService _productService;
        public Order _order = new Order();
        public string Name = string.Empty;
        public string Address = string.Empty;
        public Forsendelse Forsendelse = Forsendelse.Pickup;

        public StoreModel(ProductService productService)
        {
            _productService = productService;
        }

        public List<Product> Products { get; set; } = new();

        [BindProperty]
        public List<Item> Items { get; set; } = new();

        public async Task OnGetAsync()
        {
            Products = await _productService.GetProductsAsync();
        }

        public async Task<IActionResult> OnPostSubmitRequest()
        {
            // Fjern ordrer med amount = 0
            var validOrders = Items.Where(o => o.Amount > 0).ToList();

            if (!validOrders.Any())
            {
                TempData["Error"] = "Du skal vælge mindst ét produkt med en mængde > 0.";
                return RedirectToPage();
            }

            // Send ordrerne til dit API via ProductService
            var result = await _productService.SubmitOrdersAsync();

            if (result.Success)
            {
                TempData["Status"] = "Ordrene blev sendt!";
            }
            else
            {
                TempData["Status"] = $"Fejl fra API: {result.Message}";
            }

            return RedirectToPage();
        }
    }
}
