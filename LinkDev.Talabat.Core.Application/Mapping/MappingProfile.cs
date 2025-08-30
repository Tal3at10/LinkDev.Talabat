using AutoMapper;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Products;
using LinkDev.Talabat.Core.Domain.Entities.Products;

namespace LinkDev.Talabat.Core.Application.Mapping
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductToReturnDto>()
           .ForMember(d => d.Brand, o => o.MapFrom(src => src.Brand!.Name))
           .ForMember(d => d.Category, o => o.MapFrom(src => src.Category!.Name))
                //.ForMember(d => d.PictureUrl, o => o.MapFrom(src => $"{"https://localohost:7088"}{src.PictureUrl}"))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPictureUrlResolver>());

            CreateMap<ProductBrand, BrandDto>();
            CreateMap<ProductCategory, CategoryDto>();
        }
    }
}
