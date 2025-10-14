using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Case;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.Extensions;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseDetails
{
    public class GetCaseWohDateQuery : IRequest<PaginatedResult<GetCaseInfoDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<string> LinkedIds { get; set; }
    }
    public class GetCaseWohDateQueryHandler : IRequestHandler<GetCaseWohDateQuery, PaginatedResult<GetCaseInfoDto>>
    {
        private readonly IUserCaseRepository _repository;
        private readonly ICaseHelperRepository caseHelper;
        public GetCaseWohDateQueryHandler(IUserCaseRepository _repository, ICaseHelperRepository caseHelper)
        {
            this._repository = _repository;
            this.caseHelper = caseHelper;
        }
        public async Task<PaginatedResult<GetCaseInfoDto>> Handle(GetCaseWohDateQuery request, CancellationToken cancellationToken)
        {
            // Step 1: Get all case IDs which have child cases
            var parentCaseIdsWithChildren = new HashSet<Guid>(
                _repository.Entites.AsNoTracking()
                    .Where(c => c.LinkedCaseId != null)
                    .Select(c => c.LinkedCaseId.Value)
                    .Distinct()
                    .ToList()
            );
            // Step 2: Main query
            var baseQuery = (from c in _repository.Entites.AsNoTracking().Where(d => d.DisposalDate == null)
                             where request.LinkedIds.Contains(c.CreatedBy)
                             let caseLastProceedingDate = c.CaseProcEntities.Any()
                                 ? c.CaseProcEntities
                                     .OrderByDescending(d => d.ProceedingDate)
                                     .Select(s => new { s.ProceedingDate, s.NextDate })
                                     .FirstOrDefault()
                                 : null
                             let prcDate = caseLastProceedingDate != null ? caseLastProceedingDate.ProceedingDate : (DateTime?)null
                             let nextProcDate = caseLastProceedingDate != null ? caseLastProceedingDate.NextDate : (DateTime?)null
                             let latestNextDate = (nextProcDate != null && prcDate != null && nextProcDate >= prcDate
                                 ? nextProcDate
                                 : prcDate).GetValueOrDefault()
                             let caseFirstDate = c.NextDate != null ? c.NextDate.Value.ToString("dd/MM/yyyy") : ""
                             let caseLatestNextDate = (prcDate == null && nextProcDate == null) && c.NextDate != null
                                 ? caseFirstDate
                                 : latestNextDate.ToString("dd/MM/yyyy")
                             // Use HashSet lookup to mark if case has children
                             let hasChild = parentCaseIdsWithChildren.Contains(c.Id)
                             select new GetCaseInfoDto
                             {
                                 Id = c.Id,
                                 No = c.CaseNo,
                                 Year = c.CaseYear.ToString(),
                                 CourtType = c.CourtType.CourtType.ToString(),
                                 CaseType = c.CaseType.Name_En,
                                 Court = c.CourtBench.CourtBench_En.ToUpper(),
                                 CaseStage = c.CaseStage.CaseStage.ToUpper(),
                                 DisposalDate = c.DisposalDate,
                                 CaseDetail = (c.FirstTitle + " V/S " + c.SecondTitle).ToUpper(),
                                 NextDate = caseLatestNextDate,
                                 ProceedingDate = prcDate.HasValue ? prcDate.Value.ToString("dd/MM/yyyy") : "",
                                 IsCaseHavingChild = hasChild
                             })
                            .OrderByDescending(o => o.Year)
                            .AsQueryable();

            int pageSize = request.PageSize == -1 ? baseQuery.Count() : request.PageSize;
            var pagedResult = await baseQuery.ToPaginatedListAsync(request.PageNumber, pageSize);
            // Step 4: Return updated paged result
            return pagedResult;
        }
    }
}
