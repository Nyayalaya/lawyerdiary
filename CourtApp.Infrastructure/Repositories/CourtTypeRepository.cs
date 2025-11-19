using CourtApp.Application.CacheKeys;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.LawyerDiary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CourtApp.Infrastructure.Repositories
{
    public class CourtTypeRepository : ICourtTypeRepository
    {
        private readonly IRepositoryAsync<CourtTypeEntity> _repository;
        private readonly IDistributedCache _distributedCache;

        public CourtTypeRepository(IRepositoryAsync<CourtTypeEntity> _repository, IDistributedCache _distributedCache)
        {
            this._repository = _repository;
            this._distributedCache = _distributedCache;
        }

        public IQueryable<CourtTypeEntity> CourtTypeEntities => _repository.Entities;

        public async Task DeleteAsync(CourtTypeEntity courtTypeEntity)
        {
            await _repository.DeleteAsync(courtTypeEntity);
            await _distributedCache.RemoveAsync(CourtTypeCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CourtTypeCacheKeys.GetKey(courtTypeEntity.Id));
        }

        public async Task<CourtTypeEntity> GetByIdAsync(Guid CourtTypeId)
        {
            return await _repository.Entities.Where(p => p.Id == CourtTypeId).FirstOrDefaultAsync();
        }

        public async Task<List<CourtTypeEntity>> GetListAsync()
        {

            return await _repository
                .Entities
                .AsNoTracking()
                .Include(ct => ct.Languages)
                .Select(ct => new CourtTypeEntity
                {
                    Id=ct.Id,
                    CourtType = ct.CourtType,
                    Abbreviation = ct.Abbreviation,
                    Languages = ct.Languages
                                .Where(l => l.Code == "hi")
                                .ToList()
                })
                .ToListAsync();

        }

        public async Task<Guid> InsertAsync(CourtTypeEntity courtTypeEntity)
        {
            await _repository.AddAsync(courtTypeEntity);
            await _distributedCache.RemoveAsync(CourtTypeCacheKeys.ListKey);
            return courtTypeEntity.Id;
        }

        public async Task UpdateAsync(CourtTypeEntity courtTypeEntity)
        {
            await _repository.UpdateAsync(courtTypeEntity);
            await _distributedCache.RemoveAsync(CourtTypeCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CourtTypeCacheKeys.GetKey(courtTypeEntity.Id));
        }
    }
}
