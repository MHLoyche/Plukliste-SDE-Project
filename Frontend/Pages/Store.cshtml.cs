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
        // Dependency Injection: vi får en instans af ProductService ind i konstruktøren

        public IEnumerable<Delivery> DeliveryTypes =>
            EnumUtil.GetValues<Delivery>();
        // Returnerer alle værdier fra Delivery-enummet vha. en helper-metode.
        // Kan bruges direkte i .cshtml til at lave dropdowns eller knapper.

        public List<Item> Items { get; set; } = new();
        // Liste af produkter, som skal vises på butiks-siden.

        [BindProperty]
        public Order Order { get; set; } = new();
        // Binder en Order-model til formularens inputfelter.
        // Når brugeren poster formen, vil værdierne blive mappet hertil automatisk.

        public StoreModel(ProductService productService)
        {
            _productService = productService;
            // Gemmer DI-injectet ProductService til senere brug
        }

        public async Task OnGetAsync() // Kører når siden loades første gang (HTTP GET)
        {
            // Henter produkter fra backend via ProductService
            ApiResponse<List<Item>> response = await _productService.GetItemsAsync();

            if (response.Success && response.Data != null) // Tjekker om API-kaldet lykkedes
            {
                Items = response.Data; // Lagrer listen af produkter

                // Initialiserer ordrelinjer med alle varer, men sætter mængde = 0 som start
                Order.Lines = Items.Select(i => new Item
                {
                    ProductID = i.ProductID,
                    Title = i.Title,
                    Type = i.Type,
                    Amount = 0
                }).ToList();
            }
            else
            {
                // Hvis noget fejler: log til konsollen og vis besked i ViewData
                Console.WriteLine($"Error fetching items: {response.Message}");
                ViewData["status"] = $"Error fetching items: {response.Message}";
            }
        }

        public async Task<IActionResult> OnPostSubmitAsync() // Kører når brugeren submitter formen (HTTP POST)
        {
            // Fjern linjer hvor brugeren ikke har valgt nogen mængde
            Order.Lines = Order.Lines
                .Where(l => l.Amount > 0)
                .ToList();

            if (!Order.Lines.Any()) // Hvis ingen varer er valgt
            {
                TempData["Status"] = "Du skal vælge mindst 1 vare med mængde > 0.";
                return RedirectToPage(); // Reload siden med statusbesked
            }

            // Send ordre til backend for at oprette den
            ApiResponse<Order> result = await _productService.CreateOrderAsync(Order);

            if (result.Success)
            {
                // Oprettelsen lykkedes → redirect (evt. til en betalingsside senere)
                return RedirectToPage();
            }
            else
            {
                // Oprettelsen fejlede → vis fejlbesked til brugeren
                TempData["Status"] = $"Fejl ved oprettelse af ordre: {result.Message}";
                return RedirectToPage();
            }
        }
    }
}
