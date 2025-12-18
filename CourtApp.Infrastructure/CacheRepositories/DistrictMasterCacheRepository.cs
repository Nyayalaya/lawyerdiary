using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.Extensions.Caching;
using CourtApp.Entities.Common;
using CourtApp.Application.CacheKeys;
using Microsoft.Extensions.Caching.Distributed;
using CourtApp.Application.Interfaces.CacheRepositories.Common;
using CourtApp.Application.Interfaces.Repositories.Common;

namespace CourtApp.Infrastructure.CacheRepositories
{
    public class DistrictMasterCacheRepository:IDsitrictMasterCacheRepository
    {
        private readonly IDistrictMasterRepository _repository;
        private readonly IDistributedCache _distributedCache;
        public DistrictMasterCacheRepository(IDistrictMasterRepository _repository, IDistributedCache _distributedCache)
        {
            this._distributedCache = _distributedCache;
            this._repository = _repository;
        }

        public async Task<List<DistrictEntity>> GetDistrictListByStateAsync(int StateCode)
        {
            string cacheKey = DistrictMasterCacheKeys.SelectListByStateKey(StateCode);
            var DistrictList = await _distributedCache.GetAsync<List<DistrictEntity>>(cacheKey);
            if (DistrictList==null)
            {
                DistrictList = await _repository.GetDistrictListByStateAsync(StateCode);
                await _distributedCache.SetAsync(cacheKey, DistrictList);
            }
            return DistrictList;
        }
    }
}