using CourtApp.Domain.Entities.LawyerDiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Application.Interfaces.Repositories
{
    public interface ICaseDocsRepository
    {
        Task<Guid> SaveCaseDocAsync(CaseDocsEntity entity);
        Task<Guid> UpdateAsync(CaseDocsEntity entity);
        IQueryable<CaseDocsEntity> Entities { get; }
        Task DeleteAsync(CaseDocsEntity delEntities);
        Task DeleteRangeAsync(List<CaseDocsEntity> delEntities);

    }
}
