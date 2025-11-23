using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.FormBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace CourtApp.Infrastructure.Repositories
{
    public class CourtFormTypeRepository : ICourtFormTypeRepository
    {
        private readonly IRepositoryAsync<CourtFormTypeEntity> _repository;
        private readonly IDistributedCache _distributedCache;
        public CourtFormTypeRepository(IRepositoryAsync<CourtFormTypeEntity> _repository, IDistributedCache _distributedCache)
        {
            this._repository = _repository;
            this._distributedCache = _distributedCache;
        }
        public IQueryable<CourtFormTypeEntity> Entities => _repository.Entities;

        public async Task DeleteAsync(CourtFormTypeEntity entity)
        {
            await _repository.DeleteAsync(entity);
        }

        public async Task<CourtFormTypeEntity> GetByIdAsync(Guid Id)
        {
            var DetailById = _repository.Entities
               .Where(p => p.Id == Id).FirstOrDefaultAsync();
            return await DetailById;
        }

        public Task<List<CourtFormTypeEntity>> GetListAsync()
        {
            var langues = _repository.Entities.ToListAsync();
            return langues;
        }

        public async Task<Guid> InsertAsync(CourtFormTypeEntity entity)
        {
            await _repository.AddAsync(entity);
            return entity.Id;
        }

        public async Task UpdateAsync(CourtFormTypeEntity entity)
        {
            await _repository.UpdateAsync(entity);
        }
    }
}
