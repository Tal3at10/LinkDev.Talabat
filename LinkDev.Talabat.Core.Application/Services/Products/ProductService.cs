using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LinkDev.Talabat.Core.Application.Abstraction.Common;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Products;
using LinkDev.Talabat.Core.Application.Abstraction.Services.Products;
using LinkDev.Talabat.Core.Domain.Contracts.Presistance;
using LinkDev.Talabat.Core.Domain.Entities.Products;
using LinkDev.Talabat.Core.Domain.Specifications;
using LinkDev.Talabat.Core.Domain.Specifications.Products;

namespace LinkDev.Talabat.Core.Application.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Pagination<ProductToReturnDto>> GetProductsAsync(ProductSpecParams Specparams)
         {
            var specification = new ProductWithBrandAndCategorySpecifications(Specparams.Sort, Specparams.BrandId, Specparams.CategoryId, Specparams.PageIndex, Specparams.PageSize);
            var products = await _unitOfWork.GenericRepository<Product, int>().GetAllWithSpecAsync(specification);
            var productsToReturn = _mapper.Map<IEnumerable<ProductToReturnDto>>(products);

            return new Pagination<ProductToReturnDto>(Specparams.PageIndex,Specparams.PageSize,productsToReturn);
        }

        public async Task<IEnumerable<BrandDto>> GetBrandsAsync()
        {

            var brands = await _unitOfWork.GenericRepository<ProductBrand, int>().GetAllAsync();
            var brandsToReturn = _mapper.Map<IEnumerable<BrandDto>>(brands);

            return brandsToReturn;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _unitOfWork.GenericRepository<ProductCategory, int>().GetAllAsync();
            var categoriesToReturn = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            return categoriesToReturn;
        }

        public async Task<ProductToReturnDto> GetProductAsync(int id)
        {
            var specification = new ProductWithBrandAndCategorySpecifications(id);
            var product = await _unitOfWork.GenericRepository<Product, int>().GetWithSpecAsync(specification);
            var productToReturn = _mapper.Map<ProductToReturnDto>(product);

            return productToReturn;
        }
    }
}
