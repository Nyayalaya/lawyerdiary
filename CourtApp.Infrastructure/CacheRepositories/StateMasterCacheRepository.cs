using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.Extensions.Caching;
using CourtApp.Entities.Common;
using CourtApp.Application.CacheKeys;
using Microsoft.Extensions.Caching.Distributed;
using CourtApp.Application.Interfaces.CacheRepositories.Common;
using CourtApp.Application.Interfaces.Repositories.Common;

namespace CourtApp.Infrastructure.CacheRepositories
{
    public class StateMasterCacheRepository : IStateMasterCacheRepository
    {
         private readonly IDistributedCache _distributedCache;
        private readonly IStateMasterRepository _repository;
        public StateMasterCacheRepository(IDistributedCache _distributedCache,IStateMasterRepository _repository)
        {
            this._distributedCache=_distributedCache;
            this._repository=_repository;
        }
        public async Task<List<StateEntity>> GetStateListAsync()
        {
             string cacheKey = StateMasterCacheKeys.SelectListKey;
            var StateList = await _distributedCache.GetAsync<List<StateEntity>>(cacheKey);
            if (StateList==null)
            {
                StateList = await _repository.GetStateListAsync();
                //await _distributedCache.SetAsync(cacheKey, StateList);
            }
            return StateList;
        }
    }
}