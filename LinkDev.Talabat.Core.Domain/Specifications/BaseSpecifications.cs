using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinkDev.Talabat.Core.Domain.Common;
using LinkDev.Talabat.Core.Domain.Contracts;

namespace LinkDev.Talabat.Core.Domain.Specifications
{
    public abstract class BaseSpecifications<TEntity, TKey> : ISpecifications<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public Expression<Func<TEntity, bool>>? Critaria { get; set; }
        public List<Expression<Func<TEntity, object>>> Includes { get; set; } = new ();
        public Expression<Func<TEntity, object>>? OrderBy { get; set; } = null;
        public Expression<Func<TEntity, object>>? OrderByDes { get; set; } = null;
        public int Skip { get; set ; }
        public int Take { get; set; }
        public bool IsPaginationEnabled { get; set; }

        public BaseSpecifications(Expression<Func<TEntity, bool>> CritariaExpression)
        {
            Critaria = CritariaExpression;
        }

        public BaseSpecifications(TKey id)
        {
            Critaria = E => E.Id.Equals(id);
        }

        private protected virtual void AddIncludes()
        {

        }

        private protected virtual void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        private protected virtual void AddOrderByDesc(Expression<Func<TEntity, object>> orderByExpressionDes)
        {
            OrderByDes = orderByExpressionDes;
        }

        private protected void ApplyPagination(int skip, int take)
        {
            IsPaginationEnabled = true;
            Skip = skip;
            Take = take;
        }

    }
}
