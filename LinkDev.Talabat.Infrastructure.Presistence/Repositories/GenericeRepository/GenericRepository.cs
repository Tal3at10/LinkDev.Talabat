using LinkDev.Talabat.Core.Domain.Common;
using LinkDev.Talabat.Core.Domain.Contracts;
using LinkDev.Talabat.Core.Domain.Contracts.Presistance;
using LinkDev.Talabat.Core.Domain.Entities.Products;
using LinkDev.Talabat.Infrastructure.Persistence.Data;
using LinkDev.Talabat.Infrastructure.Persistence.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace LinkDev.Talabat.Infrastructure.Presistence.Repositories.GenericeRepository
{
    internal class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : BaseAuditableEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public Task DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false)
        {
            return withTracking
                ? await _dbContext.Set<TEntity>().ToListAsync()
                : await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllWithSpecAsync(ISpecifications<TEntity, TKey> specifications, bool withTracking = false)
        {
            var query = ApplySpecifications(specifications);

            if (!withTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetAsync(TKey id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetWithSpecAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await ApplySpecifications(specifications).ToListAsync();
        }

        public Task UpdateAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            return Task.CompletedTask;
        }


        #region Helpers

        private IQueryable<TEntity> ApplySpecifications(ISpecifications<TEntity, TKey> specifications)
        {
          return SpecificationsEvaluator<TEntity, TKey>.GetQuery(_dbContext.Set<TEntity>(), specifications);
        }

        #endregion
    }
}
