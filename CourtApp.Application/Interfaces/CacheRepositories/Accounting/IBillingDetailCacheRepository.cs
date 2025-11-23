using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourtApp.Domain.Entities.Account;
using CourtApp.Domain.Entities.LawyerDiary;

namespace CourtApp.Application.Interfaces.CacheRepositories.Accounting
{
    public interface IBillingDetailCacheRepository
    {
        Task<List<BillingDetailEntity>> GetCachedListAsync();
        Task<BillingDetailEntity> GetByIdAsync(Guid id);
        Task<BillingDetailEntity> GetByLawyerIdAsync(string lawyerId);
    }
}
