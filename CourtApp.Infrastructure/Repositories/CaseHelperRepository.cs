using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Infrastructure.Shared.Services
{
    public class CaseHelperRepository : ICaseHelperRepository
    {
        private readonly IRepositoryAsync<CaseDetailEntity> _repository;
        public CaseHelperRepository(IRepositoryAsync<CaseDetailEntity> _repository)
        {
            this._repository = _repository;
        }
        public bool IsCaseAssignedOrSelf(Guid caseId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsCaseHavingChildAsync(Guid caseId)
        {
            return await _repository.Entities
                .AsNoTracking()
                .AnyAsync(c => c.LinkedCaseId == caseId);
        }

        public async Task<List<CaseProcedingEntity>> CaseProceedingsAsync(Guid caseId)
        {
            var caseProceedings = await _repository.Entities
                .AsNoTracking()
                .Where(c => c.Id == caseId)
                .SelectMany(c => c.CaseProcEntities)
                .ToListAsync();

            return caseProceedings;
        }
    }
}
