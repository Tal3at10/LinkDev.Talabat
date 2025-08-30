using LinkDev.Talabat.Core.Domain.Common;
using LinkDev.Talabat.Core.Domain.Entities.Products;
using LinkDev.Talabat.Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;

namespace LinkDev.Talabat.Infrastructure.Persistence.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all IEntityTypeConfiguration<T> classes in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyInformations).Assembly);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            foreach (var entity in this.ChangeTracker.Entries<BaseAuditableEntity<int>>()
                         .Where(entity => entity.State is EntityState.Added or EntityState.Modified))
            {
                if (entity.State is EntityState.Added)
                {
                    entity.Entity.CreatedBy = "";
                    entity.Entity.CreatedOn = DateTime.Now;
                }

                entity.Entity.LastModifiedBy = "";
                entity.Entity.LastModifiedOn = DateTime.UtcNow;
            }

            // بعد ما تخلص تعديل الـ entities كلهم
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> Brands { get; set; }
        public DbSet<ProductCategory> Categories { get; set; }
    }
}
