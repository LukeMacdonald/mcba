using Microsoft.EntityFrameworkCore;

namespace MCBAWebApi.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new MCBAContext(
            serviceProvider.GetRequiredService<DbContextOptions<MCBAContext>>());

        // Look for any movies.
        if (context.Customer.Any())
            return; // DB has been seeded.
    }
}