using AutoMapper;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Products;
using LinkDev.Talabat.Core.Domain.Entities.Products;
using Microsoft.Extensions.Configuration;

namespace LinkDev.Talabat.Core.Application.Mapping
{
    public class ProductPictureUrlResolver(IConfiguration configuration) : IValueResolver<Product, ProductToReturnDto, string?>
    {
        public string? Resolve(Product source, ProductToReturnDto destination, string? destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
            {
                return $"{configuration["Urls:ApiBaseUrl"]}/{source.PictureUrl}";
            }

            return string.Empty;
        }
    }
}
