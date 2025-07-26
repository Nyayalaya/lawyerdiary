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

            var today = DateTime.Today;

            // Step 1: Load and filter cases with their latest proceeding
            var rawCases = await _repository.Entites
                .Include(c => c.CourtType)
                .Include(c => c.CaseType)
                .Include(c => c.CaseStage)
                .Include(c => c.CourtBench)
                .Include(c => c.CaseProcEntities)
                .Where(c =>
                    request.LinkedIds.Contains(c.CreatedBy) &&
                    c.DisposalDate == null
                )
                .ToListAsync();

            // Step 2: Filter and project manually in-memory
            var filteredCases = rawCases
                .Select(c => new
                {
                    Case = c,
                    LatestProceeding = c.CaseProcEntities
                        .OrderByDescending(p => p.NextDate)
                        .Select(p => new
                        {
                            p.NextDate,
                            p.ProceedingDate
                        })
                        .FirstOrDefault()
                })
                .Where(x =>
                    (x.LatestProceeding == null && x.Case.NextDate < today) ||
                    (x.LatestProceeding != null && !x.LatestProceeding.NextDate.HasValue && x.Case.NextDate < today) ||
                    (x.LatestProceeding != null && x.LatestProceeding.NextDate.HasValue &&
                        (
                            (x.LatestProceeding.NextDate > x.Case.NextDate
                                ? x.LatestProceeding.NextDate
                                : x.Case.NextDate
                            ) < today
                        )
                    )
                )
                .ToList(); // Now fully materialized, safe for async

            // Step 3: Project to DTO with async call
            var items = new List<GetCaseInfoDto>();

            foreach (var x in filteredCases)
            {
                var dto = new GetCaseInfoDto
                {
                    Id = x.Case.Id,
                    No = x.Case.CaseNo,
                    Year = x.Case.CaseYear.ToString(),
                    CaseType = x.Case.CaseType?.Name_En,
                    Court = x.Case.CourtBench?.CourtBench_En,
                    CaseStage = x.Case.CaseStage?.CaseStage,
                    DisposalDate = x.Case.DisposalDate,
                    CaseDetail = $"{x.Case.FirstTitle} V/S {x.Case.SecondTitle}",
                    ProceedingDate = x.LatestProceeding?.ProceedingDate?.ToString("dd/MM/yyyy") ?? "",
                    NextDate = x.LatestProceeding?.NextDate?.ToString("dd/MM/yyyy") ??
                               (x.Case.NextDate.HasValue ? x.Case.NextDate.Value.ToString("dd/MM/yyyy") : ""),
                    IsCaseHavingChild = await caseHelper.IsCaseHavingChildAsync(x.Case.Id)
                };
                items.Add(dto);
            }

            // Step 4: Order and paginate
            var orderedCases = items
                .OrderByDescending(x => int.TryParse(x.Year, out var y) ? y : 0)
                .ToList();

            var pagedResult = orderedCases
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList(); // or .ToPaginatedList() if your helper supports it

            return PaginatedResult<GetCaseInfoDto>.Success(
               pagedResult,
               orderedCases.Count(),
               request.PageNumber,
               request.PageSize
           );
        }
    }
}
