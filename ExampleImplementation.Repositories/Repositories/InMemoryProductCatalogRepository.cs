using ExampleImplementation.Repositories.Abstractions;
using ExampleImplementation.Repositories.Models;

namespace ExampleImplementation.Repositories.Repositories;

public sealed class InMemoryProductCatalogRepository : IProductCatalogRepository
{
    private static readonly IReadOnlyList<ExampleProduct> SeedData = new List<ExampleProduct>
    {
        new() { Id = 1, Name = "Starter Wrench Set", Category = "Tools", Price = 49.99m, IsActive = true },
        new() { Id = 2, Name = "Precision Screwdriver Kit", Category = "Tools", Price = 29.50m, IsActive = true },
        new() { Id = 3, Name = "Compact Drill", Category = "Power Tools", Price = 89.00m, IsActive = true },
        new() { Id = 4, Name = "Cordless Circular Saw", Category = "Power Tools", Price = 159.00m, IsActive = true },
        new() { Id = 5, Name = "Basic Work Gloves", Category = "Safety", Price = 12.99m, IsActive = true },
        new() { Id = 6, Name = "Safety Glasses", Category = "Safety", Price = 9.75m, IsActive = false },
        new() { Id = 7, Name = "Jobsite Tape Measure", Category = "Tools", Price = 14.25m, IsActive = true },
        new() { Id = 8, Name = "Laser Level", Category = "Measuring", Price = 119.95m, IsActive = true },
        new() { Id = 9, Name = "Digital Caliper", Category = "Measuring", Price = 39.95m, IsActive = true },
        new() { Id = 10, Name = "Heavy Duty Clamp", Category = "Hardware", Price = 18.00m, IsActive = true },
        new() { Id = 11, Name = "Bench Vise", Category = "Hardware", Price = 76.49m, IsActive = true },
        new() { Id = 12, Name = "Discontinued Router", Category = "Power Tools", Price = 210.00m, IsActive = false }
    };

    public Task<IReadOnlyList<ExampleProduct>> ListAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<ExampleProduct>>(SeedData.ToList());
    }
}
