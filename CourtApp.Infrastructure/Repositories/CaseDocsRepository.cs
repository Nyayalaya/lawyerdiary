using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.LawyerDiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Infrastructure.Repositories
{
    public class CaseDocsRepository : ICaseDocsRepository
    {
        private readonly IRepositoryAsync<CaseDocsEntity> _repository;
        public CaseDocsRepository(IRepositoryAsync<CaseDocsEntity> _repository)
        {
            this._repository = _repository;
        }

        public IQueryable<CaseDocsEntity> Entities => _repository.Entities;

        public async Task DeleteRangeAsync(List<CaseDocsEntity> delEntities)
        {
            await _repository.DeleteRangeAsync(delEntities);
        }
        public async Task DeleteAsync(CaseDocsEntity entity)
        {
            await _repository.DeleteAsync(entity);
        }

        public async Task<Guid> SaveCaseDocAsync(CaseDocsEntity entity)
        {
            await _repository.AddAsync(entity);
            return entity.Id;
        }

        public async Task<Guid> UpdateAsync(CaseDocsEntity entity)
        {
            await _repository.UpdateAsync(entity);
            return entity.Id;
        }
    }
}
