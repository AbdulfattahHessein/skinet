using System.Reflection;
using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!context.Products.Any())
        {
            Directory.GetCurrentDirectory();
            var productsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);
            if (products is null)
                return;
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        if (!context.DeliveryMethods.Any())
        {
            Directory.GetCurrentDirectory();
            var productsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");
            var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(productsData);
            if (deliveryMethods is null)
                return;
            await context.DeliveryMethods.AddRangeAsync(deliveryMethods);
            await context.SaveChangesAsync();
        }
    }
}
