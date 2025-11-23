using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Infrastructure.Repositories
{
    public class CaseAssignedRepository : ICaseAssignedRepository
    {
        private readonly IRepositoryAsync<AssignCaseEntity> _repository;
        private readonly IDistributedCache _distributedCache;
        public CaseAssignedRepository(IRepositoryAsync<AssignCaseEntity> _repository, IDistributedCache _distributedCache)
        {
            this._repository = _repository;
            this._distributedCache = _distributedCache;
        }
        public IQueryable<AssignCaseEntity> Entities => _repository.Entities;

        public async Task DeleteRangeAsync(List<AssignCaseEntity> entity)
        {
            await _repository.DeleteRangeAsync(entity);
        }

        public async Task<Guid> InsertAsync(AssignCaseEntity entity)
        {
            await _repository.AddAsync(entity);
            return entity.Id;
        }

        public async Task UpdateAsync(AssignCaseEntity entity)
        {
            await _repository.UpdateAsync(entity);
        }
    }
}
