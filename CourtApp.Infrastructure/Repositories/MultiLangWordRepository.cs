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

            // 1️⃣ Normalize + distinct incoming keywords ONCE
            var incomingNormalized = dictEntities
                .Where(x => !string.IsNullOrWhiteSpace(x.KeyWord))
                .Select(x => x.KeyWord.Trim().ToUpperInvariant())
                .Distinct()
                .ToList();

            if (incomingNormalized.Count == 0)
                return new List<Guid>();

            // 2️⃣ Fetch existing keywords (normalized)
            var existingNormalized = await _repository.Entities
                .AsNoTracking()
                .Where(x => incomingNormalized.Contains(x.KeyWord.Trim().ToUpper()))
                .Select(x => x.KeyWord.Trim().ToUpper())
                .ToListAsync();

            // 3️⃣ Convert to HashSet for O(1) lookup
            var existingSet = existingNormalized.ToHashSet();

            // 4️⃣ Filter only missing entities
            var newEntities = dictEntities
                .Where(x => !string.IsNullOrWhiteSpace(x.KeyWord))
                .Select(x => new
                {
                    Entity = x,
                    Normalized = x.KeyWord.Trim().ToUpperInvariant()
                })
                .Where(x => !existingSet.Contains(x.Normalized))
                .GroupBy(x => x.Normalized)
                .Select(g =>
                {
                    var entity = g.First().Entity;
                    entity.KeyWord = entity.KeyWord.Trim();
                    return entity;
                })
                .ToList();


            if (newEntities.Count == 0)
                return new List<Guid>();

            // 4️⃣ Ensure IDs
            foreach (var entity in newEntities)
            {
                if (entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();

                entity.KeyWord = entity.KeyWord.Trim();
            }

            // 5️⃣ Bulk insert
            await _repository.AddRange(newEntities);

            // 6️⃣ Invalidate cache ONCE
            await _distributedCache.RemoveAsync(MultiLangDictCacheKey.All);

            return newEntities.Select(x => x.Id).ToList();
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
                return await _repository.Entities.Where(x=>!x.MultiLangs.Any())
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
            await _distributedCache.RemoveAsync(MultiLangDictCacheKey.All);
            await _repository.UpdateRangeAsync(entities);
            return entities.Select(e => e.Id).ToList();
        }

    }
}
