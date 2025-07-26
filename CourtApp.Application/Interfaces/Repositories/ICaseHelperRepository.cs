using CourtApp.Domain.Entities.CaseDetails;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourtApp.Application.Interfaces.Repositories
{
    public interface ICaseHelperRepository
    {
        Task<bool> IsCaseHavingChildAsync(Guid caseId);
        Task<List<CaseProcedingEntity>> CaseProceedingsAsync(Guid caseId);
        bool IsCaseAssignedOrSelf(Guid caseId);
    }
}
