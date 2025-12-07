using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourtApp.Entities.Common;

namespace CourtApp.Application.Interfaces.CacheRepositories.Common
{
    public interface IDsitrictMasterCacheRepository
    {
        Task<List<DistrictEntity>> GetDistrictListByStateAsync(int StateCode);
    }
}