using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkDev.Talabat.Core.Domain.Common;
using LinkDev.Talabat.Core.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LinkDev.Talabat.Infrastructure.Persistence.Repositories.GenericRepository
{
    internal static class SpecificationsEvaluator<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public static IQueryable<TEntity> GetQuery
        (
            IQueryable<TEntity> inputQuery,
            ISpecifications<TEntity, TKey> specifications
        )
        {
            var query = inputQuery; // _dbContext.Set<Product>();

            // Apply criteria (e.g. p => p.Id.Equals(1))
            if (specifications.Critaria is not null)
            {
                query = query.Where(specifications.Critaria);
            }

            if (specifications.OrderByDes is not null)
            {
                query = query.OrderByDescending(specifications.OrderByDes);
            }
            else if(specifications.OrderByDes is not null)
            {
                query = query.OrderBy(specifications.OrderBy);

            }

            if(specifications.IsPaginationEnabled)
            {
                query = query.Skip(specifications.Skip).Take(specifications.Take);
            }


            // Apply includes (navigation properties)
            query = specifications.Includes.Aggregate(
                query,
                (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

            return query;
        }
    }
}
