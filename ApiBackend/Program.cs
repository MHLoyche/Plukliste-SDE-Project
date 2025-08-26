var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/storage/items", () =>
{
    using (StreamReader reader = new StreamReader("DatabaseData/Data.json"))
    {
        string json = reader.ReadToEnd();
        return Results.Content(json, "application/json");
    }
    
})
.WithName("getitems")
.WithOpenApi();

app.Run();

