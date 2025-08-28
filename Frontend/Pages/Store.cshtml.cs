using ClassLibrary.Enums;
using ClassLibrary.HelperClasses;
using ClassLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class StoreModel : PageModel
    {
        private readonly ProductService _productService; // out product services injected via DI
        public IEnumerable<Delivery> DeliveryTypes => EnumUtil.GetValues<Delivery>(); // an IEnumerable list of Delivery enum values from our EnumUtil helper class
        public StoreModel(ProductService productService) // Constructor that gets the injected ProductService
        {
            _productService = productService; // sets the private field to the injected service
        }

        public List<Item> Items { get; set; } = new(); // List of Items to be displayed on the page

        [BindProperty]
        public Order Order { get; set; } = new(); // Binder Order med post input fields. Så det er up to date når formen bliver submittet


        
        public async Task OnGetAsync() // Metode som bliver kaldt når siden loades
        {
            Items = await _productService.GetItemsAsync(); // Sætter items til response fra GetItemsAsync


            Order.Lines = Items.Select(i => new Item
            {
                ProductID = i.ProductID,
                Title = i.Title,
                Type = i.Type,
                Amount = 0
            }).ToList(); // Initialiserer Order.Lines med default værdier og Amount = 0
        }


        
        public async Task<IActionResult> OnPostSubmitRequest() // Metode som bliver kaldt når formen bliver submittet samt returnerer en IActionResult
        {
            Order.Lines = Order.Lines.Where(l => l.Amount > 0).ToList(); // Sætter Order.Lines til kun at indeholde de linjer hvor Amount > 0 (Kun dem vi har valgt)

            if (!Order.Lines.Any()) // Tjekker om vi har valgt nogen
            {
                TempData["Error"] = "Du skal vælge mindst ét produkt med en mængde > 0."; // Sætter en fejlbesked i TempData
                return RedirectToPage(); // Redirecter tilbage til siden
            }

            var result = await _productService.SubmitOrdersAsync(Order); // Kalder SubmitOrdersAsync med den nuværende Order og venter på resultatet

            TempData["Status"] = result.Success
                ? "Ordren blev sendt!"
                : $"Fejl fra API: {result.Message}"; // Sætter status til enten success eller fejlbesked fra API'et

            return RedirectToPage(); // Redirecter tilbage til siden
        }
    }
}
