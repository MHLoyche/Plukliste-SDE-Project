using ClassLibrary.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Frontend.Pages
{
    [IgnoreAntiforgeryToken]
    public class InterfaceModel : PageModel
    {
        private readonly ProductService _productService;
        public List<Order> Orders { get; set; } = new();
        public Order CurrentOrder { get; set; } = new();
        public int CurrentIndex { get; set; }

        public InterfaceModel(ProductService productService)
        {
            _productService = productService;
        }

        public async Task OnGetAsync(int index = 0)
        {
            ApiResponse<List<Order>> response = await _productService.GetOrdersAsync();
            if (response.Success && response.Data != null)
            {
                Orders = response.Data;
                if (Orders.Any())
                {
                    CurrentIndex = Math.Clamp(index, 0, Orders.Count - 1);
                    CurrentOrder = Orders[CurrentIndex];
                }
                else
                {
                    CurrentIndex = -1;
                }
            }
            else 
            {
                Console.WriteLine($"Error fetching orders: {response.Message}");
                ViewData["status"] = $"Error fetching orders: {response.Message}";
            }
        }



        public async Task<IActionResult> OnPostSubmitAsync([FromBody] List<Order> orders)
        {
            
            if (orders.Any())
            {
                ApiResponse<bool> response = await _productService.SubmitOrdersAsync(orders);
                if (response.Success)
                {
                    return RedirectToPage("Interface");
                }
                else
                {
                    return RedirectToPage("Interface");
                }
            }

            return RedirectToPage("Interface");



        }





    }
}
