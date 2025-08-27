using System.Text.Json;
using ApiBackend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Henter alle produkter fra lageret
app.MapGet("/storage/items", () =>
{
    var json = File.ReadAllText("Database/ProductStock.json");
    return Results.Content(json, "application/json");
})
.WithName("getitems")
.WithOpenApi();

// Modtager ordrer og opdaterer lageret
app.MapPost("/orders", async (HttpRequest request) =>
{
    try
    {
        var orders = await request.ReadFromJsonAsync<List<Order>>();

        if (orders == null || !orders.Any())
            return Results.BadRequest(new { success = false, message = "Ingen ordrer modtaget." });

        var filePath = "Database/ProductStock.json";
        var json = await File.ReadAllTextAsync(filePath);
        var products = JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();

        var outOfStock = new List<object>();
        foreach (var order in orders)
        {
            var product = products.FirstOrDefault(p => p.ProductID == order.ProductID);
            if (product == null || product.Amount < order.Amount)
            {
                outOfStock.Add(new
                {
                    ProductID = order.ProductID,
                    Requested = order.Amount,
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

        foreach (var order in orders)
        {
            var product = products.First(p => p.ProductID == order.ProductID);
            product.Amount -= order.Amount;
        }

        await File.WriteAllTextAsync(filePath,
            JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true }));

        var exportDir = Path.Combine(Directory.GetCurrentDirectory(), "Export");
        Directory.CreateDirectory(exportDir);

        var exportPath = Path.Combine(exportDir, $"Kaj-Nielsen_Order_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        await File.WriteAllTextAsync(exportPath,
            JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true }));

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


