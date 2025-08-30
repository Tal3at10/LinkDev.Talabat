using LinkDev.Talabat.Core.Domain.Contracts.Presistance;

namespace LinkDev.Talabat.APIs.Extensions
{
    public static class IntializerExtensions
    {
        public static async Task<WebApplication> IntializeStoreContextAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var storeContextIntializer = services.GetRequiredService<IStoreContextIntializer>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await storeContextIntializer.IntializeAsync();
                await storeContextIntializer.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during applying migrations or the Seeding of the Data");
            }

            return app;
        }

    }
}
