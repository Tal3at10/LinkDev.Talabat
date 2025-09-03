using LinkDev.Talabat.Core.Domain.Entities.Products;

namespace LinkDev.Talabat.Core.Domain.Specifications.Products
{
    public class ProductWithFilterationForCountSpecifcations : BaseSpecifications<Product, int>
    {
        // The object is created via constructor to build the query that will count all products.
        public ProductWithFilterationForCountSpecifcations(int? brandId, int? categoryId, string? search)
            : base
            (
                p =>
                 (string.IsNullOrEmpty(search) || p.NormalizedName.Contains(search.ToUpper()))
                  &&
                    (!brandId.HasValue || p.BrandId == brandId.Value) &&
                    (!categoryId.HasValue || p.CategoryId == categoryId.Value)
            )
        {
        }
    }
}
