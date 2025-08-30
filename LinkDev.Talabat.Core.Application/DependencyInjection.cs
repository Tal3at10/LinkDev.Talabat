using LinkDev.Talabat.Core.Application.Abstraction.Services;
using LinkDev.Talabat.Core.Application.Abstraction.Services.Products;
using LinkDev.Talabat.Core.Application.Mapping;
using LinkDev.Talabat.Core.Application.Services;
using LinkDev.Talabat.Core.Application.Services.Products;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDev.Talabat.Core.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // 1) إضافة Profile بشكل مباشر (إضافة instance من MappingProfile)
            // إضافة كل الـ Profiles اللي موجودة في نفس Assembly
           services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

            // تسجيل Resolver
            services.AddScoped<ProductPictureUrlResolver>();

            // 2) نفس اللي فوق بالظبط (مكرر) - ممكن تستغني عنه
            //services.AddAutoMapper(Mapper => Mapper.AddProfile(new MappingProfile()));

            // 3) إضافة Profile واحد محدد بالـ Type
            //services.AddAutoMapper(typeof(MappingProfile)); // OLder AutoMapper Version

            // 4) إضافة كل الـ Profiles اللي موجودة في نفس Assembly
            //services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddScoped(typeof(IProductService), typeof(ProductService));

            services.AddScoped(typeof(IServiceManager), typeof(ServiceManager));


            return services;
        }
    }
}
