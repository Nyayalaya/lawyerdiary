using CourtApp.Domain.Entities.CaseDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Application.Interfaces.Repositories
{
    public interface ICaseProceedingRepository
    {
        IQueryable<CaseProcedingEntity> Entities { get; }
        Task<List<CaseProcedingEntity>> GetListAsync();
        Task<List<CaseProcedingEntity>> GetProceedingByCaseIdAsync(Guid CaseId);
        Task<CaseProcedingEntity> GetByIdAsync(Guid CaseId, DateTime? SelDate);
        Task<CaseProcedingEntity> GetDetailById(Guid Id);
        Task<Guid> AddAsync(CaseProcedingEntity Entity);
        Task<Guid> AddAsyncRange(List<CaseProcedingEntity> Entity);
        Task UpdateAsync(CaseProcedingEntity Entity);
        Task UpdateRangeAsync(List<CaseProcedingEntity> Entities);
        Task DeleteAsync(CaseProcedingEntity Entity);
        Task DeleteRangeAsync(List<CaseProcedingEntity> Entities);
    }
}
