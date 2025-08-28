using ClassLibrary;
using ClassLibrary.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Gør JSON håndtering mere robust (case-insensitive + enums som strings)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// De options vi bruger i vores egne JsonSerializer kald
var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
};
jsonOptions.Converters.Add(new JsonStringEnumConverter());

// GET: alle produkter
app.MapGet("/storage/items", () =>
{
    var json = File.ReadAllText("Database/ProductStock.json");
    var items = JsonSerializer.Deserialize<List<Item>>(json, jsonOptions) ?? new List<Item>();
    return Results.Json(items, jsonOptions);
})
.WithName("getitems")
.WithOpenApi();

app.MapGet("/storage/orders", () =>
{
    var exportDir = Path.Combine(Directory.GetCurrentDirectory(), "Export");
    Directory.CreateDirectory(exportDir);
    var orders = new List<Order>();
    var files = Directory.GetFiles(exportDir, "*_Order_*.json");
    foreach (var file in files)
    {
        var json = File.ReadAllText(file);
        var order = JsonSerializer.Deserialize<Order>(json, jsonOptions);
        if (order != null)
            orders.Add(order);
    }
    return Results.Json(orders, jsonOptions);
}).Accepts<string>("application/json")
  .Produces<List<Order>>(StatusCodes.Status200OK)
  .WithName("getorders")
  .WithOpenApi();
// POST: modtag en ordre
app.MapPost("/storage/orders", async (Order order) =>
{
    try
    {
        Console.WriteLine("✅ DESERIALISERET ORDRE:");
        Console.WriteLine(JsonSerializer.Serialize(order, jsonOptions));

        if (order == null || !order.Lines.Any())
            return Results.BadRequest(new { success = false, message = "Ingen ordrer modtaget." });

        var filePath = "Database/ProductStock.json";
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<Item>>(json, jsonOptions) ?? new List<Item>();

        var outOfStock = new List<object>();
        foreach (var line in order.Lines)
        {
            var product = items.FirstOrDefault(p => p.ProductID == line.ProductID);
            if (product == null || product.Amount < line.Amount)
            {
                outOfStock.Add(new
                {
                    ProductID = line.ProductID,
                    Requested = line.Amount,
                    Available = product?.Amount ?? 0
                });
            }
        }

        if (outOfStock.Any())
        {
            return Results.BadRequest(new
            {
                success = false,
                message = "Nogle varer er ikke på lager.",
                outOfStock
            });
        }

        // Træk antal fra lageret
        foreach (var line in order.Lines)
        {
            var product = items.First(p => p.ProductID == line.ProductID);
            product.Amount -= line.Amount;
        }

        await File.WriteAllTextAsync(filePath,
            JsonSerializer.Serialize(items, jsonOptions));

        // Eksportér ordren
        var exportDir = Path.Combine(Directory.GetCurrentDirectory(), "Export");
        Directory.CreateDirectory(exportDir);

        var exportPath = Path.Combine(exportDir, $"{order.Name}_Order_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        await File.WriteAllTextAsync(exportPath,
    JsonSerializer.Serialize(order, new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    }));

        return Results.Ok(new { success = true, message = "Ordren blev registreret." });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Der opstod en fejl: {ex.Message}");
    }
})
.WithName("submitorders")
.WithOpenApi();

app.Run();
