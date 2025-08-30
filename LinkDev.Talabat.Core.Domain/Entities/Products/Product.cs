using LinkDev.Talabat.Core.Domain.Common;

namespace LinkDev.Talabat.Core.Domain.Entities.Products
{
    public class Product : BaseAuditableEntity<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string? PictureUrl { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; } // Forignkey of ProductBrand
        public ProductBrand? Brand { get; set; }
        public int CategoryId { get; set; } // Forignkey of ProductCategory
        public virtual ProductCategory? Category { get; set; }
    }
}
