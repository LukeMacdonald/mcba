using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MCBAWebApi.Data;
using MCBAWebApi.Models;
using MCBAWebApi.Models.DataManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MCBAContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CustomerContext"));
    // options.UseLazyLoadingProxies();
    
});

builder.Services.AddScoped<CustomerManager>();
builder.Services.AddScoped<LoginManager>();
builder.Services.AddScoped<BillPayManager>();
builder.Services.AddScoped<AccountManager>();
builder.Services.AddScoped<PayeeManager>();

//builder.Services.AddTransient<MovieManager>();

builder.Services.AddControllers();

// Ignore JSON reference cycles during serialisation.
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed data.
using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch(Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
// app.UseAuthorization();

app.MapControllers();

// .NET 6 Minimal APIs - Simple Example.
// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-7.0

// GET api/UsingMapGet?name=Matthew&repeat=100
app.MapGet("api/UsingMapGet", (string name, int? repeat) =>
{
    if(string.IsNullOrWhiteSpace(name))
        name = "(empty)";
    if(repeat is null or < 1)
        repeat = 1;

    return string.Join(' ', Enumerable.Repeat(name, repeat.Value));
});

app.Run();
