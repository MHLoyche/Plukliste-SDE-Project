using ClassLibrary;
using ClassLibrary.Enums;
using ClassLibrary.HelperClasses;
using ClassLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class StoreModel : PageModel
    {
        private readonly ProductService _productService;
        public IEnumerable<Delivery> DeliveryTypes => EnumUtil.GetValues<Delivery>();
        public StoreModel(ProductService productService)
        {
            _productService = productService;
        }

        public List<Item> Items { get; set; } = new();

        [BindProperty]
        public Order Order { get; set; } = new();

        public async Task OnGetAsync()
        {
            Items = await _productService.GetItemsAsync();

            // Initier Lines så Razor binder korrekt
            Order.Lines = Items.Select(i => new Item
            {
                ProductID = i.ProductID,
                Title = i.Title,
                Type = i.Type,
                Amount = 0
            }).ToList();
        }



        public async Task<IActionResult> OnPostSubmitRequest()
        {
            // Fjern linjer med amount = 0
            Order.Lines = Order.Lines.Where(l => l.Amount > 0).ToList();

            if (!Order.Lines.Any())
            {
                TempData["Error"] = "Du skal vælge mindst ét produkt med en mængde > 0.";
                return RedirectToPage();
            }

            // Send ordren til API via ProductService
            var result = await _productService.SubmitOrdersAsync(Order);

            TempData["Status"] = result.Success
                ? "Ordren blev sendt!"
                : $"Fejl fra API: {result.Message}";

            return RedirectToPage();
        }
    }
}
