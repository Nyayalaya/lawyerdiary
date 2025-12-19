using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using CourtApp.Application.CacheKeys;
using CourtApp.Application.Interfaces.CacheRepositories.Common;
using CourtApp.Application.Interfaces.Repositories.Common;
using CourtApp.Domain.Entities.Common;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Infrastructure.CacheRepositories
{
    public class MultiLangDictCacheRepository : IMultiLangWordCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMultiLangWordRepository _repository;

        public MultiLangDictCacheRepository(
            IMultiLangWordRepository repository,
            IDistributedCache distributedCache)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        /// <summary>
        /// Get single MultiLangDictEntity by Id from cache or repository
        /// </summary>
        public async Task<MultiLangDictEntity> GetByIdAsync(Guid id)
        {
            string cacheKey = MultiLangDictCacheKey.GetById(id);

            var entity = await _distributedCache.GetAsync<MultiLangDictEntity>(cacheKey);
            if (entity == null)
            {
                entity = await _repository.GetByIdAsync(id);
                Throw.Exception.IfNull(entity, nameof(MultiLangDictEntity), "No Dictionary Entry Found");

                await _distributedCache.SetAsync(cacheKey, entity);
            }

            return entity;
        }

        /// <summary>
        /// Get all MultiLangDictEntities filtered by language code
        /// </summary>
        public async Task<List<MultiLangDictEntity>> GetListByLanCodeAsync(string langCode)
        {
            string cacheKey =langCode==null?MultiLangDictCacheKey.All: MultiLangDictCacheKey.ByLanguage(langCode);

            var entities = await _distributedCache.GetAsync<List<MultiLangDictEntity>>(cacheKey);
            if (entities == null)
            {
                entities = await _repository.GetListByLanCodeAsync(langCode);
                if (entities != null && entities.Any())
                {
                    await _distributedCache.SetAsync(cacheKey, entities);
                }
            }

            return entities;
        }

        /// <summary>
        /// Clear all cached dictionary data
        /// </summary>
        public async Task ClearCacheAsync()
        {
            await _distributedCache.RemoveAsync(MultiLangDictCacheKey.All);
        }
    }
}
