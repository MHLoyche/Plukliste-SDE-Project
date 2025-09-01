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
        public List<Item> Items { get; set; } = new(); // List of Items to be displayed on the page
        [BindProperty]
        public Order Order { get; set; } = new(); // Binder Order med post input fields. Så det er up to date når formen bliver submittet

        public StoreModel(ProductService productService) // Constructor that gets the injected ProductService
        {
            _productService = productService; // sets the private field to the injected service
        }
        
        public async Task OnGetAsync() // Metode som bliver kaldt når siden loades
        {
            ApiResponse<List<Item>> response = await _productService.GetItemsAsync(); // Kalder GetItemsAsync på ProductService
            if (response.Success && response.Data != null) // Hvis kaldet lykkedes og der er data
            {
                Items = response.Data; // Sætter Items til den hentede data
                Order.Lines = Items.Select(i => new Item
                {
                    ProductID = i.ProductID,
                    Title = i.Title,
                    Type = i.Type,
                    Amount = 0
                }).ToList(); // Initialiserer Order.Lines med default værdier og Amount = 0
            }
            else
            {
                // Håndter fejl, f.eks. log fejlbesked eller vis notifikation
                Console.WriteLine($"Error fetching items: {response.Message}");
                ViewData["status"] = $"Error fetching items: {response.Message}"; // Sætter fejlbesked i ViewData for visning på siden
            }

           
        }


        public async Task<IActionResult> OnPostSubmitAsync()
        {
            Order.Lines = Order.Lines
            .Where(l => l.Amount > 0)
            .ToList();

            if (!Order.Lines.Any())
            {
                TempData["Status"] = "Du skal vælge mindst 1 vare med mængde > 0.";
                return RedirectToPage();
            }

            ApiResponse<Order> result = await _productService.CreateOrderAsync(Order);

            if (result.Success) {
                // Hvis oprettelsen lykkedes, send brugeren til betaling
                return RedirectToPage();
            } else {
                // Hvis oprettelsen fejlede, vis fejlbesked
                TempData["Status"] = $"Fejl ved oprettelse af ordre: {result.Message}";
                return RedirectToPage();
            }



            
        }


    }
}
