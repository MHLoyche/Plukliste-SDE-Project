using ClassLibrary.Model;                
using System.Text;                         
using System.Text.Json;                    
using System.Text.Json.Serialization;      
public class ProductService
{
    private readonly HttpClient _httpClient;   // HttpClient bruges til at kalde API’et
    JsonSerializerOptions options = new JsonSerializerOptions // Settings til JSON serialization/deserialization
    {
        PropertyNameCaseInsensitive = true     // Gør at JSON feltnavne ikke skelner mellem store/små bogstaver
    };

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;              // Får HttpClient ind via dependency injection

        // Tilføjer en converter der sørger for at enums kan serialiseres/deserialiseres som tekst (fx "Product")
        options.Converters.Add(new JsonStringEnumConverter());
    }

    // Henter alle ordrer fra backend
    public async Task<ApiResponse<List<Order>>> GetOrdersAsync()
    {
        string url = "https://localhost:7148/storage/orders/get"; // URL til API’et

        try
        {
            var response = await _httpClient.GetAsync(url); // Sender HTTP GET request

            if (!response.IsSuccessStatusCode)              // Tjekker om kaldet lykkedes
            {
                return new ApiResponse<List<Order>>        // Hvis fejl -> returnér fejlbesked
                {
                    Success = false,
                    Message = $"Failed to fetch orders: {response.StatusCode}"
                };
            }

            var json = await response.Content.ReadAsStringAsync();                 // Læser rå JSON tekst
            var orders = JsonSerializer.Deserialize<List<Order>>(json, options)    // Deserialiserer til liste af Order
                          ?? new();                                               // Hvis null -> tom liste

            return new ApiResponse<List<Order>>   // Returnér succes med data
            {
                Success = true,
                Data = orders
            };
        }
        catch (HttpRequestException ex)          // Hvis der er problemer med HTTP-kaldet
        {
            return new ApiResponse<List<Order>>
            {
                Success = false,
                Message = $"Request failed: {ex.Message}"
            };
        }
        catch (Exception ex)                     // Alle andre fejl
        {
            return new ApiResponse<List<Order>>
            {
                Success = false,
                Message = $"Unexpected error: {ex.Message}"
            };
        }
    }

    // Henter alle items (lager) fra backend
    public async Task<ApiResponse<List<Item>>> GetItemsAsync()
    {
        string url = "https://localhost:7148/storage/items/get"; // URL til API’et

        try
        {
            var response = await _httpClient.GetAsync(url); // HTTP GET request

            if (!response.IsSuccessStatusCode)              // Tjekker om kaldet lykkedes
            {
                return new ApiResponse<List<Item>>         // Returnér fejlbesked hvis ikke
                {
                    Success = false,
                    Message = $"Failed to fetch items: {response.StatusCode}"
                };
            }

            var json = await response.Content.ReadAsStringAsync();                // Læs rå JSON
            var items = JsonSerializer.Deserialize<List<Item>>(json, options)     // Deserialiser til Item-liste
                        ?? new();                                                // Tom liste hvis null

            return new ApiResponse<List<Item>>   // Returnér succes med data
            {
                Success = true,
                Data = items
            };
        }
        catch (HttpRequestException ex)          // Netværksfejl, timeouts osv.
        {
            return new ApiResponse<List<Item>>
            {
                Success = false,
                Message = $"Request error: {ex.Message}"
            };
        }
        catch (JsonException ex)                 // Fejl ved JSON-parsing
        {
            return new ApiResponse<List<Item>>
            {
                Success = false,
                Message = $"Deserialization error: {ex.Message}"
            };
        }
        catch (Exception ex)                     // Andre uforudsete fejl
        {
            return new ApiResponse<List<Item>>
            {
                Success = false,
                Message = $"Unexpected error: {ex.Message}"
            };
        }
    }

    // Sender en liste af ordrer til backend for at oprette dem
    public async Task<ApiResponse<bool>> SubmitOrdersAsync(List<Order> orders)
    {
        string url = "https://localhost:7148/storage/orders/submit"; // URL til API’et

        try
        {
            string json = JsonSerializer.Serialize(orders, options); // Serialiserer ordrer til JSON
            var content = new StringContent(json, Encoding.UTF8, "application/json"); // Pakker JSON i en HTTP-body

            var response = await _httpClient.PostAsync(url, content); // Sender POST request

            if (!response.IsSuccessStatusCode)                        // Hvis POST fejler
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to submit orders: {response.StatusCode} - {error}"
                };
            }

            return new ApiResponse<bool>   // Returnér succes = true
            {
                Success = true,
                Data = true
            };
        }
        catch (HttpRequestException ex)   // Netværksfejl
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Request error: {ex.Message}"
            };
        }
        catch (JsonException ex)          // Fejl ved serialisering
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Serialization error: {ex.Message}"
            };
        }
        catch (Exception ex)              // Andre fejl
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Unexpected error: {ex.Message}"
            };
        }
    }

    // Opretter en enkelt ordre i backend
    public async Task<ApiResponse<Order>> CreateOrderAsync(Order order)
    {
        string url = "https://localhost:7148/storage/order/create"; // URL til API’et

        try
        {
            string json = JsonSerializer.Serialize(order, options); // Serialiserer en ordre til JSON
            var content = new StringContent(json, Encoding.UTF8, "application/json"); // Laver body til request

            var response = await _httpClient.PostAsync(url, content); // Sender POST request

            if (!response.IsSuccessStatusCode)                       // Tjekker status
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<Order>
                {
                    Success = false,
                    Message = $"Failed to create order: {response.StatusCode} - {error}"
                };
            }

            var responseBody = await response.Content.ReadAsStringAsync();               // Læs JSON svar
            var createdOrder = JsonSerializer.Deserialize<Order>(responseBody, options); // Deserialiser til Order

            return new ApiResponse<Order>  // Returnér succes med det oprettede objekt
            {
                Success = true,
                Data = createdOrder ?? new Order()
            };
        }
        catch (HttpRequestException ex)    // Netværksfejl
        {
            return new ApiResponse<Order>
            {
                Success = false,
                Message = $"Request error: {ex.Message}"
            };
        }
        catch (JsonException ex)           // Fejl ved JSON-deserialisering
        {
            return new ApiResponse<Order>
            {
                Success = false,
                Message = $"Deserialization error: {ex.Message}"
            };
        }
        catch (Exception ex)               // Andre uforudsete fejl
        {
            return new ApiResponse<Order>
            {
                Success = false,
                Message = $"Unexpected error: {ex.Message}"
            };
        }
    }
}
