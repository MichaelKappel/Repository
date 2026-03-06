namespace ExampleImplementation.Repositories.Models;

public sealed class ExampleProduct
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
}
