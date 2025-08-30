using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkDev.Talabat.Core.Domain.Entities.Products;

namespace LinkDev.Talabat.Core.Domain.Specifications.Products
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product, int>
    {
        //the object is created via constructor is used for building the query that will get all products.
        public ProductWithBrandAndCategorySpecifications(string? sort, int? brandId, int? categoryId,int pageSize, int pageIndex)
            : base
            (
                  p => 
                  (!brandId.HasValue || p.BrandId == brandId.Value)
                  &&
                  (!categoryId.HasValue || p.CategoryId == categoryId.Value)
            )
        {
            AddIncludes();

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "nameDesc":
                      AddOrderByDesc(p => p.Name);
                        break;

                    case "PriceAsc":
                        AddOrderBy(p => p.Price);
                        break;

                    case "PriceDesc":
                        AddOrderByDesc(p => p.Price);
                        break;

                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }




            }

            else
            {
                // OrderBy افتراضي دائمًا على Id
                AddOrderBy(p => p.Id);
            }

            // totall products = 18 ~ 20 
            // pagesize = 5
            // pageIndex = 3
            ApplyPagination(pageSize * (pageIndex - 1), pageSize);

        }

        public ProductWithBrandAndCategorySpecifications(int id)
           : base(id)
        {
            AddIncludes();
        }

        private protected override void AddIncludes()
        {
            base.AddIncludes();
            Includes.Add(p => p.Brand!);
            Includes.Add(p => p.Category!);

        }
    }
}
