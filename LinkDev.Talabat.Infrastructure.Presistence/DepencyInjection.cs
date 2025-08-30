using LinkDev.Talabat.Core.Domain.Contracts.Presistance;
using LinkDev.Talabat.Infrastructure.Persistence.Data; // فقط Persistence الصح
using LinkDev.Talabat.Infrastructure.Presistence.Data;
using LinkDev.Talabat.Infrastructure.Presistence.Data.Interceptors;
using LinkDev.Talabat.Infrastructure.Presistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDev.Talabat.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("StoreContext"));
            });

            services.AddScoped<IStoreContextIntializer, StoreContextIntializer>();
            services.AddScoped(typeof(IStoreContextIntializer), typeof(StoreContextIntializer));
            services.AddScoped(typeof(ISaveChangesInterceptor), typeof(CustomSaveChangesInterceptor));

            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            return services;
        }
    }
}
