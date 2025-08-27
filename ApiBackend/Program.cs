var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            // Allows anything to speak to the API backend
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

// Enable CORS middleware BEFORE mapping endpoints
app.UseCors("AllowFrontend");

app.MapGet("/storage/items", () =>
{
    using (StreamReader reader = new StreamReader("Database/ProductStock.json"))
    {
        string json = reader.ReadToEnd();
        return Results.Content(json, "application/json");
    }
})
.WithName("getitems")
.WithOpenApi();

app.Run();
