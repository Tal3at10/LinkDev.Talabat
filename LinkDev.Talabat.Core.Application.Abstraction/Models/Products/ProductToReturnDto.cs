namespace LinkDev.Talabat.Core.Application.Abstraction.Models.Products
{
    public class ProductToReturnDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string? PictureUrl { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int? BrandId { get; set; } // Forignkey of ProductBrand
        public int? CategoryId { get; set; } // Forignkey of ProductCategory
    }
}
