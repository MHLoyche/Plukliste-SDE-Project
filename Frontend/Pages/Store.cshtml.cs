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


        // Når siden loades hentes Items og Order.Lines bliver sat til en kopi af Items
        public async Task OnGetAsync()
        {
            Items = await _productService.GetItemsAsync();

            
            Order.Lines = Items.Select(i => new Item
            {
                ProductID = i.ProductID,
                Title = i.Title,
                Type = i.Type,
                Amount = 0
            }).ToList();
        }


        // Når en form bliver submittet køres metoden og Alle odre der har mere end 0 i Amount bliver sendt til backend
        public async Task<IActionResult> OnPostSubmitRequest()
        {
            Order.Lines = Order.Lines.Where(l => l.Amount > 0).ToList();

            if (!Order.Lines.Any())
            {
                TempData["Error"] = "Du skal vælge mindst ét produkt med en mængde > 0.";
                return RedirectToPage();
            }

            var result = await _productService.SubmitOrdersAsync(Order);

            TempData["Status"] = result.Success
                ? "Ordren blev sendt!"
                : $"Fejl fra API: {result.Message}";

            return RedirectToPage();
        }
    }
}
