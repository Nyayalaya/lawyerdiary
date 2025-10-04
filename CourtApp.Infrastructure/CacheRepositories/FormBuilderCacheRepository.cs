using CourtApp.Application.CacheKeys;
using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using CourtApp.Domain.Entities.FormBuilder;
using CourtApp.Application.Interfaces.Repositories.FormBuilder;
using CourtApp.Application.Interfaces.CacheRepositories.FormBuilder;
namespace CourtApp.Infrastructure.CacheRepositories
{
    public class FormBuilderCacheRepository : IFormBuilderCacheRepository
    {
        private readonly IFormBuilderRepository _repository;
        private readonly IDistributedCache _distributedCache;

        public FormBuilderCacheRepository(IFormBuilderRepository _repository, IDistributedCache _distributedCache)
        {
            this._distributedCache = _distributedCache;
            this._repository = _repository;
        }

        public async Task<FormBuilderEntity> GetByIdAsync(Guid Id)
        {
            string cacheKey = FormBuilderCacheKeys.GetKey(Id);
            var brand = await _distributedCache.GetAsync<FormBuilderEntity>(cacheKey);
            if (brand == null)
            {
                brand = await _repository.GetByIdAsync(Id);
                Throw.Exception.IfNull(brand, "TemplateInfo", "No no template info found");
                await _distributedCache.SetAsync(cacheKey, brand);
            }
            return brand;
        }

        public async Task<List<FormBuilderEntity>> GetCachedListAsync()
        {
            string cacheKey = FormBuilderCacheKeys.ListKey;
            var bookList = await _distributedCache.GetAsync<List<FormBuilderEntity>>(cacheKey);
            if (bookList == null)
            {
                bookList = await _repository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, bookList);
            }
            return bookList;
        }
    }
}
