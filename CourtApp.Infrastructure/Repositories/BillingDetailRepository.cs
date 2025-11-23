using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourtApp.Application.CacheKeys;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Accounting;
using CourtApp.Domain.Entities.Account;
using CourtApp.Domain.Entities.LawyerDiary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using static CourtApp.Application.Constants.Permissions;

namespace CourtApp.Infrastructure.Repositories
{
    public class BillingDetailRepository : IBillingDetailRepository
    {

        private readonly IRepositoryAsync<BillingDetailEntity> _repository;
        private readonly IDistributedCache _distributedCache;

        public BillingDetailRepository(IRepositoryAsync<BillingDetailEntity> _repository,
            IDistributedCache _distributedCache)
        {
            this._distributedCache = _distributedCache;
            this._repository = _repository;
        }
        public IQueryable<BillingDetailEntity> Entities => _repository.Entities;

        public async Task DeleteAsync(BillingDetailEntity entity)
        {
            await _repository.DeleteAsync(entity);
            await _distributedCache.RemoveAsync(BillingDetailCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(BillingDetailCacheKeys.GetKey(entity.Id));
        }

        public async Task<BillingDetailEntity> GetByIdAsync(Guid id)
        {
            var Detail = await _repository.Entities
                .Where(p => p.Id == id).FirstOrDefaultAsync();
            return Detail;
        }

        public async Task<List<BillingDetailEntity>> GetListAsync()
        {
            return await _repository.Entities.ToListAsync();
        }

        public async Task<Guid> InsertAsync(BillingDetailEntity entity)
        {
            await _repository.AddAsync(entity);
            await _distributedCache.RemoveAsync(BillingDetailCacheKeys.ListKey);
            return entity.Id;
        }

        public async Task UpdateAsync(BillingDetailEntity entity)
        {
            await _repository.UpdateAsync(entity);
            await _distributedCache.RemoveAsync(BillingDetailCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(BillingDetailCacheKeys.GetKey(entity.Id));
        }
    }
}
