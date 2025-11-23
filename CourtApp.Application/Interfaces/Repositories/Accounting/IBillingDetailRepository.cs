using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourtApp.Domain.Entities.Account;
namespace CourtApp.Application.Interfaces.Repositories.Accounting
{
    public interface IBillingDetailRepository
    {
        IQueryable<BillingDetailEntity> Entities { get; }

        Task<List<BillingDetailEntity>> GetListAsync();

        Task<BillingDetailEntity> GetByIdAsync(Guid id);

        Task<Guid> InsertAsync(BillingDetailEntity entity);

        Task UpdateAsync(BillingDetailEntity entity);

        Task DeleteAsync(BillingDetailEntity entity);
    }
}
