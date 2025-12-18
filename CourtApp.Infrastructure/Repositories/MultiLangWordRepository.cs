using CourtApp.Application.CacheKeys;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using CourtApp.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Infrastructure.Repositories
{
    public class MultiLangWordRepository : IMultiLangWordRepository
    {
        private readonly IRepositoryAsync<MultiLangDictEntity> _repository;
        private readonly IDistributedCache _distributedCache;

        public MultiLangWordRepository(IRepositoryAsync<MultiLangDictEntity> _repository,
            IDistributedCache _distributedCache)
        {
            this._repository = _repository;
            this._distributedCache = _distributedCache;
        }
        public IQueryable<MultiLangDictEntity> Entities => _repository.Entities;

        public async Task<List<Guid>> BulkInsertAsync(List<MultiLangDictEntity> dictEntities)
        {           
            if (dictEntities == null || dictEntities.Count == 0)
                return new List<Guid>();
            foreach (var entity in dictEntities)
            {
                if (entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();
            }
            await _repository.AddRange(dictEntities); 
            await _distributedCache.RemoveAsync(MultiLangDictCacheKey.All);
            return dictEntities.Select(x => x.Id).ToList();
        }

        public async Task DeleteAsync(MultiLangDictEntity dictEntity)
        {
            if (dictEntity == null)
                throw new ArgumentNullException(nameof(dictEntity));

            await _distributedCache.RemoveAsync(MultiLangDictCacheKey.GetById(dictEntity.Id));
            await _repository.DeleteAsync(dictEntity);           
        }

        public async Task<MultiLangDictEntity> GetByIdAsync(Guid Id)
        {
            return await _repository.Entities
                          .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<List<MultiLangDictEntity>> GetListByLanCodeAsync(string langCode)
        {
            // If language code is empty → RETURN ALL records
            if (string.IsNullOrWhiteSpace(langCode))
            {
                return await _repository.Entities
                            .OrderBy(x => x.MultiLangs.Any())  // false → null/empty first
                            .ThenBy(x => x.KeyWord)
                            .ToListAsync();
            }

            // Filter by specific language key
            return await _repository.Entities
                .Where(x => x.MultiLangs != null &&
                            x.MultiLangs.Any(m => m.Key == langCode))
                .ToListAsync();
        }

        public async Task UpdateAsync(MultiLangDictEntity dictEntity)
        {
            if (dictEntity == null)
                throw new ArgumentNullException(nameof(dictEntity));
            await _distributedCache.RemoveAsync(MultiLangDictCacheKey.GetById(dictEntity.Id));
            await  _repository.UpdateAsync(dictEntity);
        }

        public async Task<List<Guid>> UpdateRangeAsync(List<MultiLangDictEntity> entities)
        {
            if (entities == null || !entities.Any())
                return new List<Guid>();
            await _repository.UpdateRangeAsync(entities);
            return entities.Select(e => e.Id).ToList();
        }

    }
}
