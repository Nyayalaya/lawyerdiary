using CourtApp.Application.CacheKeys;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Infrastructure.Repositories
{
    public class CourtFeeStructureRepository : ICourtFeeStructureRepository
    {
        private readonly IRepositoryAsync<CourtFeeStructureEntity> _repository;
        private readonly IDistributedCache _distributedCache;
        public CourtFeeStructureRepository(IRepositoryAsync<CourtFeeStructureEntity> _repository, IDistributedCache _distributedCache)
        {
            this._repository = _repository;
            this._distributedCache = _distributedCache;
        }
        public IQueryable<CourtFeeStructureEntity> Entites => _repository.Entities;
         

        public async Task DeleteAsync(CourtFeeStructureEntity objEntity)
        {
            await _repository.DeleteAsync(objEntity);
            await _distributedCache.RemoveAsync(CourtFeeStructureCacheKey.ListKey);
            await _distributedCache.RemoveAsync(CourtFeeStructureCacheKey.GetKey(objEntity.Id));
        }

        public async Task<CourtFeeStructureEntity> GetByIdAsync(Guid Id)
        {
            return await _repository.Entities.Where(p => p.Id == Id).FirstOrDefaultAsync();
        }

        public async Task<List<CourtFeeStructureEntity>> GetListAsync()
        {
            return await _repository.Entities.ToListAsync();
        }

        public async Task<Guid> InsertAsync(CourtFeeStructureEntity objEntity)
        {
            await _repository.AddAsync(objEntity);
            await _distributedCache.RemoveAsync(CourtFeeStructureCacheKey.ListKey);
            return objEntity.Id;
        }

        public async Task UpdateAsync(CourtFeeStructureEntity objEntity)
        {
            await _repository.UpdateAsync(objEntity);
            await _distributedCache.RemoveAsync(CourtFeeStructureCacheKey.ListKey);
            await _distributedCache.RemoveAsync(CourtFeeStructureCacheKey.GetKey(objEntity.Id));
        }
    }
}
