using CourtApp.Application.CacheKeys;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CourtApp.Infrastructure.Repositories
{
    public class CaseProceedingRepository : ICaseProceedingRepository
    {
        private readonly IRepositoryAsync<CaseProcedingEntity> _repository;
        private readonly IDistributedCache _distributedCache;
        public CaseProceedingRepository(IRepositoryAsync<CaseProcedingEntity> _repository,
            IDistributedCache _distributedCache)
        {
            this._repository = _repository;
            this._distributedCache = _distributedCache;
        }
        public IQueryable<CaseProcedingEntity> Entities => _repository.Entities;

        public async Task<Guid> AddAsync(CaseProcedingEntity Entity)
        {
            await _repository.AddAsync(Entity);
            await _distributedCache.RemoveAsync(AppCacheKeys.ProcHeadKey);
            return Entity.Id;
        }

        public async Task<Guid> AddAsyncRange(List<CaseProcedingEntity> Entity)
        {
            await _repository.AddRange(Entity);
            await _distributedCache.RemoveAsync(AppCacheKeys.ProcHeadKey);
            return Entity.FirstOrDefault().Id;

        }

        public Task DeleteAsync(CaseProcedingEntity Entity)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteRangeAsync(List<CaseProcedingEntity> Entities)
        {
            await _repository.DeleteRangeAsync(Entities);
        }

        public async Task<CaseProcedingEntity> GetByIdAsync(Guid CaseId, DateTime? SelDate)
        {

            if (SelDate != null)
            {
                var data = await _repository
                    .Entities
                    .Include(c=>c.Case)
                    .Include(t => t.ProcWork)
                    .ThenInclude(s => s.Works)
                    .Where(w => w.CaseId == CaseId && w.ProceedingDate.Value.Date == SelDate.Value.Date)
                    .FirstOrDefaultAsync();
                return data;
            }
            else
            {
                var data = await _repository
                        .Entities
                        .Include(c => c.Case)
                        .Include(t => t.ProcWork)
                        .ThenInclude(s => s.Works)
                        .Where(w => w.CaseId == CaseId)
                        .OrderByDescending(w => w.NextDate)
                        .FirstOrDefaultAsync();
                return data;
            }
        }

        public async Task<CaseProcedingEntity> GetDetailById(Guid Id)
        {
            var data = await _repository
                .Entities
                .Include(t => t.ProcWork)
                .ThenInclude(s => s.Works)
                .Where(w => w.Id == Id)
                .FirstOrDefaultAsync();
            return data;
        }

        public async Task<List<CaseProcedingEntity>> GetListAsync()
        {
            var data = await _repository
                 .Entities
                 .Include(t => t.ProcWork)
                 .Include(s => s.Case).ThenInclude(t => t.CaseType)
                 .Include(s => s.Case).ThenInclude(t => t.CourtBench)

                 .ToListAsync();
            return data;
        }

        public async Task<List<CaseProcedingEntity>> GetProceedingByCaseIdAsync(Guid CaseId)
        {
            var data = await _repository.Entities.Where(w=>w.CaseId==CaseId)
                .Include(w => w.Head)
                .Include(w => w.SubHead)
                .Include(w => w.Stage)
                .Include(p => p.ProcWork)
                    .ThenInclude(w => w.Works)
                .ToListAsync();
            return data;
        }

        //public async Task<Guid> InsertAsync(List<CaseProcedingEntity> Entity)
        //{
        //    await _repository.BulkInsert(Entity);
        //    await _distributedCache.RemoveAsync(AppCacheKeys.CourtComplexKey);
        //    return Entity.Select(s => s.CaseId).FirstOrDefault();
        //}

        public async Task UpdateAsync(CaseProcedingEntity Entity)
        {
            await _repository.UpdateAsync(Entity);
            await _distributedCache.RemoveAsync(AppCacheKeys.ProcHeadKey);

        }

        public async Task UpdateRangeAsync(List<CaseProcedingEntity> Entities)
        {
            await _repository.UpdateRangeAsync(Entities);
            await _distributedCache.RemoveAsync(AppCacheKeys.ProcHeadKey);
        }
    }
}
