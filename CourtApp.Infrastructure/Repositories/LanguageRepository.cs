using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourtApp.Application.CacheKeys;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace CourtApp.Infrastructure.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {

        private readonly IRepositoryAsync<LanguageEntity> _repository;
        private readonly IDistributedCache _distributedCache;
        public LanguageRepository(IRepositoryAsync<LanguageEntity> _repository, IDistributedCache _distributedCache)
        {
            this._repository = _repository;
            this._distributedCache = _distributedCache;
        }
        public IQueryable<LanguageEntity> Entities => _repository.Entities;

        public async Task DeleteAsync(LanguageEntity entity)
        {
            await _repository.DeleteAsync(entity);           
        }

        public async Task<LanguageEntity> GetByIdAsync(Guid Id)
        {
            var DetailById = _repository.Entities
               .Where(p => p.Id == Id).FirstOrDefaultAsync();
            return await DetailById;
        }

        public Task<List<LanguageEntity>> GetListAsync()
        {
            var langues = _repository.Entities.ToListAsync();
            return langues;
        }

        public async Task<Guid> InsertAsync(LanguageEntity entity)
        {
            await _repository.AddAsync(entity);
            return entity.Id;
        }

        public async Task UpdateAsync(LanguageEntity entity)
        {
            await _repository.UpdateAsync(entity);
        }
    }
}
