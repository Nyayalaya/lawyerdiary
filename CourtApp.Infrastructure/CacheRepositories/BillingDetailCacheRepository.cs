using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using CourtApp.Application.CacheKeys;
using CourtApp.Application.Interfaces.CacheRepositories.Accounting;
using CourtApp.Application.Interfaces.Repositories.Accounting;
using CourtApp.Domain.Entities.Account;
using Microsoft.Extensions.Caching.Distributed;

namespace CourtApp.Infrastructure.CacheRepositories
{
    public class BillingDetailCacheRepository : IBillingDetailCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IBillingDetailRepository repository;
        public BillingDetailCacheRepository(IBillingDetailRepository repository, 
            IDistributedCache _distributedCache)
        {
            this.repository = repository;
            this._distributedCache = _distributedCache;
        }
        public async Task<BillingDetailEntity> GetByIdAsync(Guid id)
        {
            string cacheKey = BillingDetailCacheKeys.GetKey(id);
            var billingDetail = await _distributedCache.GetAsync<BillingDetailEntity>(cacheKey);
            if (billingDetail == null)
            {
                billingDetail = await repository.GetByIdAsync(id);
                Throw.Exception.IfNull(billingDetail, "BillingDetail", "No lawyer billing information found!");
                await _distributedCache.SetAsync(cacheKey, billingDetail);
            }
            return billingDetail;
        }

        public Task<BillingDetailEntity> GetByLawyerIdAsync(string lawyerId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BillingDetailEntity>> GetCachedListAsync()
        {
            string cacheKey = BillingDetailCacheKeys.ListKey;
            var billingList = await _distributedCache.GetAsync<List<BillingDetailEntity>>(cacheKey);
            if (billingList == null)
            {
                billingList = await repository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, billingList);
            }
            return billingList;
        }
    }
}
