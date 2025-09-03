using LinkDev.Talabat.Core.Domain.Common;
using System.Linq.Expressions;

namespace LinkDev.Talabat.Core.Domain.Contracts.Presistance
{
    public interface IGenericRepository<TEntity, TKey>

        where TEntity : BaseAuditableEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        Task<TEntity?> GetAsync(TKey id);
        Task<IEnumerable<TEntity>> GetWithSpecAsync(ISpecifications<TEntity, TKey> specifications);

        Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking = false);
        Task<IEnumerable<TEntity>> GetAllWithSpecAsync(ISpecifications<TEntity, TKey> specifications,bool withTracking = false);
        Task<int> GetCountAsync(ISpecifications<TEntity, TKey> specifications);

        Task AddAsync(TEntity entity);
           
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);

       
    }
}
