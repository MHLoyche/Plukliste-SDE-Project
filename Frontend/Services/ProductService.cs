using ClassLibrary.Model;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ProductService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _jsonOptions.Converters.Add(
    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true)
);

    }

    // Henter lager fra backend
    public async Task<List<Item>> GetItemsAsync()
    {
        var url = "https://localhost:7148/storage/items";

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Fejl ved kald til API: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("🔎 RAW JSON FRA API:");
            Console.WriteLine(json);

            var items = JsonSerializer.Deserialize<List<Item>>(json, _jsonOptions);
            return items ?? new List<Item>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fejl i GetItemsAsync: {ex.Message}");
            return new List<Item>();
        }
    }

    // Sender en ordre til backend
    public async Task<ApiResponse> SubmitOrdersAsync(Order order)
    {
        var url = "https://localhost:7148/orders";
        var json = JsonSerializer.Serialize(order, _jsonOptions);

        Console.WriteLine("➡️ SENDER ORDRE TIL API:");
        Console.WriteLine(json);

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine("⬅️ MODTAGET FRA API:");
        Console.WriteLine(responseBody);

        return new ApiResponse
        {
            Success = response.IsSuccessStatusCode,
            Message = responseBody
        };
    }
}
