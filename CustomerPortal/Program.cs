using CustomerPortal.Data;
using CustomerPortal.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MCBAContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("McbaContext"));
    options.UseLazyLoadingProxies();
    
});
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString(nameof(MCBAContext));
    options.SchemaName = "dotnet";
    options.TableName = "SessionCache";
});
builder.Services.AddSession(options => options.Cookie.IsEssential = true);

builder.Services.AddHostedService<BillPayService>();

builder.Services.AddControllersWithViews();

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
if(!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession(); // Enable session

app.MapDefaultControllerRoute();

app.Run();