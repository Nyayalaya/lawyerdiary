using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using CourtApp.Application.CacheKeys;
using CourtApp.Application.Interfaces.CacheRepositories;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.Account;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourtApp.Infrastructure.CacheRepositories
{
    public class CourtFeeStructureCacheRepository : ICourtFeeStructureCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ICourtFeeStructureRepository _repository;
        public CourtFeeStructureCacheRepository(ICourtFeeStructureRepository _repository, IDistributedCache _distributedCache)
        {
            this._repository = _repository;
            this._distributedCache = _distributedCache;
        }
        public async Task<CourtFeeStructureEntity> GetCacheDataByIdAsync(Guid Id)
        {
            string cacheKey = AppCacheKeys.CourtFeeKey;
            var data = await _distributedCache.GetAsync<CourtFeeStructureEntity>(cacheKey);
            if (data == null)
            {
                data = await _repository.GetByIdAsync(Id);
                Throw.Exception.IfNull(data, "CourtFeeStructureEntity", "No Court Fee Structure");
                await _distributedCache.SetAsync(cacheKey, data);
            }
            return data;
        }

        public async Task<List<CourtFeeStructureEntity>> GetCacheDataListAsync()
        {
            string cacheKey = AppCacheKeys.CourtFeeKey;
            try
            {
                var dataList = await _distributedCache.GetAsync<List<CourtFeeStructureEntity>>(cacheKey);
                if (dataList == null)
                {
                    dataList = await _repository.GetListAsync();
                    await _distributedCache.SetAsync(cacheKey, dataList);
                }
                return dataList;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            return null;
        }
    }
}
