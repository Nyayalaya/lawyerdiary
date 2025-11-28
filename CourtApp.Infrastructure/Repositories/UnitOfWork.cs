using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Shared;
using CourtApp.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IAuthenticatedUserService _authenticatedUserService;
        private readonly ApplicationDbContext _dbContext;
        private bool disposed;

        public UnitOfWork(ApplicationDbContext dbContext, IAuthenticatedUserService authenticatedUserService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _authenticatedUserService = authenticatedUserService;
        }

        public async Task<int> Commit(CancellationToken cancellationToken)
        {
            try
            {
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public Task Rollback()
        {
            //todo
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    _dbContext.Dispose();
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}