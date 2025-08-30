using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkDev.Talabat.Core.Domain.Common;
using LinkDev.Talabat.Core.Domain.Entities.Products;

namespace LinkDev.Talabat.Core.Domain.Contracts.Presistance
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity, TKey> GenericRepository<TEntity, TKey>()
        where TEntity : BaseAuditableEntity<TKey>
        where TKey : IEquatable<TKey>;

         Task<int> CompleteAsync();
    }
}
