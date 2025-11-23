using CourtApp.Domain.Entities.Account;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Application.Interfaces.CacheRepositories
{
    public interface ICourtFeeStructureCacheRepository
    {
        Task<List<CourtFeeStructureEntity>> GetCacheDataListAsync();
        Task<CourtFeeStructureEntity> GetCacheDataByIdAsync(System.Guid Id);
    }
}
