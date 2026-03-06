using ExampleImplementation.Repositories.Models;

namespace ExampleImplementation.Website.Models.DataModels
{
    public class ExampleDataViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public decimal Price { get; init; }

        public static ExampleDataViewModel FromModel(ExampleProduct product)
        {
            return new ExampleDataViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price
            };
        }
    }
}
