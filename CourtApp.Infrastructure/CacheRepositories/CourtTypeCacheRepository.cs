using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using CourtApp.Application.Interfaces.CacheRepositories;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.LawyerDiary;
using CourtApp.Application.CacheKeys;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Infrastructure.CacheRepositories
{
    public class CourtTypeCacheRepository : ICourtTypeCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ICourtTypeRepository _Repository;
        public CourtTypeCacheRepository(IDistributedCache distributedCache, ICourtTypeRepository _Repository)
        {
            _distributedCache = distributedCache;
            this._Repository = _Repository;
        }
        public async Task<CourtTypeEntity> GetByIdAsync(Guid CourtTypeId)
        {
            string cacheKey = CourtTypeCacheKeys.GetKey(CourtTypeId);
            var CourtType = await _distributedCache.GetAsync<CourtTypeEntity>(cacheKey);
            if (CourtType == null)
            {
                CourtType = await _Repository.GetByIdAsync(CourtTypeId);
                Throw.Exception.IfNull(CourtType, "BookMaster", "No Book Master Found");
                await _distributedCache.SetAsync(cacheKey, CourtType);
            }
            return CourtType;
        }

        public async Task<List<CourtTypeEntity>> GetCachedListAsync()
        {
            string cacheKey = CourtTypeCacheKeys.ListKey;
            var CourtTypeList = await _distributedCache.GetAsync<List<CourtTypeEntity>>(cacheKey);
            if ( CourtTypeList==null)
            {
                CourtTypeList = await _Repository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, CourtTypeList);
            }
            return CourtTypeList;
        }
    }
}
