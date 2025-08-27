using ClassLibrary.Model;
using System.Text;
using System.Text.Json; 


namespace Frontend.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // Sørger for at matche camelCase JSON fra API'et med dine C# properties
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var url = "https://localhost:7148/storage/items";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Fejl ved kald til API: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<Product>>(json, _jsonOptions);

                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl i GetProductsAsync: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<ApiResponse> SubmitOrdersAsync(List<Order> orders)
        {
            var url = "https://localhost:7148/orders";
            var json = JsonSerializer.Serialize(orders);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            return new ApiResponse
            {
                Success = response.IsSuccessStatusCode,
                Message = responseBody
            };
        }
    }

}
