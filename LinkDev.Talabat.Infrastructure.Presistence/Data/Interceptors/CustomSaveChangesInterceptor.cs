using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinkDev.Talabat.Core.Application.Abstraction;
using LinkDev.Talabat.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LinkDev.Talabat.Infrastructure.Presistence.Data.Interceptors
{
    internal class CustomSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ILoggedInUserService _loggedInUserService;
        public CustomSaveChangesInterceptor(ILoggedInUserService loggedInUserService)
        {
            _loggedInUserService = loggedInUserService;
        }

        public ILoggedInUserService LoggedInUserService { get; }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine("SavingChangesAsync Interceptor Triggered");

            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? dbContext)
        {
            if (dbContext == null)
                  return;
            

            foreach (var entity in dbContext.ChangeTracker.Entries<BaseAuditableEntity<int>>()
                         .Where(entity => entity.State is EntityState.Added or EntityState.Modified))
            {
                if (entity.State is EntityState.Added)
                {
                    entity.Entity.CreatedBy = _loggedInUserService.UserId;
                    entity.Entity.CreatedOn = DateTime.Now;
                }

                entity.Entity.LastModifiedBy = _loggedInUserService.UserId;
                entity.Entity.LastModifiedOn = DateTime.UtcNow;
            }
        }
    }
}
