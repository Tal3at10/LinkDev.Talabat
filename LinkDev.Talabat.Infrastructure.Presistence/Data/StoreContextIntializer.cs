using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LinkDev.Talabat.Core.Domain.Contracts.Presistance;
using LinkDev.Talabat.Core.Domain.Entities.Products;
using LinkDev.Talabat.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace LinkDev.Talabat.Infrastructure.Presistence.Data
{
    internal class StoreContextIntializer : IStoreContextIntializer
    {
        private readonly StoreContext _dbContext;

        public StoreContextIntializer(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task IntializeAsync()
        {
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
                await _dbContext.Database.MigrateAsync(); // Update Database
        }

        public async Task SeedAsync()
        {
            #region Brands
            if (!_dbContext.Brands.Any())
            {
                var brandsPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "../LinkDev.Talabat.Infrastructure/Data/Seeds/brands.json"
                );

                var brandsData = await File.ReadAllTextAsync(brandsPath);
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                if (brands != null && brands.Any())
                {
                    await _dbContext.Set<ProductBrand>().AddRangeAsync(brands);
                    await _dbContext.SaveChangesAsync();
                }
            }
            #endregion

            #region Categories
            if (!_dbContext.Categories.Any())
            {
                var categoriesPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "../LinkDev.Talabat.Infrastructure/Data/Seeds/categories.json"
                );

                var categoriesData = await File.ReadAllTextAsync(categoriesPath);
                var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoriesData);

                if (categories != null && categories.Any())
                {
                    await _dbContext.Set<ProductCategory>().AddRangeAsync(categories);
                    await _dbContext.SaveChangesAsync();
                }
            }
            #endregion

            #region Products
            if (!_dbContext.Products.Any())
            {
                var productsPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "../LinkDev.Talabat.Infrastructure/Data/Seeds/products.json"
                );

                var productsData = await File.ReadAllTextAsync(productsPath);
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products != null && products.Any())
                {
                    await _dbContext.Set<Product>().AddRangeAsync(products);
                    await _dbContext.SaveChangesAsync();
                }
            }
            #endregion
        }
    }
}
