using AspNetCoreHero.Results;
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
        public GetCaseWohDateQueryHandler(IUserCaseRepository _repository)
        {
            this._repository = _repository;
        }
        public async Task<PaginatedResult<GetCaseInfoDto>> Handle(GetCaseWohDateQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today.Date;
            var cases = await _repository.Entites
                        .Include(c => c.CourtType)
                        .Include(c => c.CaseType)
                        .Include(c => c.CaseStage)
                        .Include(c => c.CourtBench)
                        .Where(c => request.LinkedIds.Contains(c.CreatedBy) && c.DisposalDate == null)
                        .Select(e => new
                        {
                            Case = e,
                            LatestProceeding = e.CaseProcEntities.Where(w => w.NextDate.HasValue)
                                            .OrderByDescending(o => o.NextDate.Value)
                                            .Select(s => new
                                            {
                                                s.NextDate,
                                                s.ProceedingDate
                                            })
                                            .FirstOrDefault()
                        })
                        .Where(x => !x.Case.NextDate.HasValue
                                    || (
                                x.LatestProceeding != null &&
                                (x.LatestProceeding.NextDate > x.Case.NextDate
                                    ? x.LatestProceeding.NextDate
                                    : x.Case.NextDate) < today
                                )
                        ).Select(x => new GetCaseInfoDto
                        {
                            Id = x.Case.Id,
                            No = x.Case.CaseNo,
                            Year = x.Case.CaseYear.ToString(),
                            CaseType = x.Case.CaseType.Name_En,
                            Court = x.Case.CourtBench.CourtBench_En,
                            CaseStage = x.Case.CaseStage.CaseStage,
                            DisposalDate = x.Case.DisposalDate,
                            CaseDetail = x.Case.FirstTitle + " V/S " + x.Case.SecondTitle,
                            ProceedingDate = x.LatestProceeding != null ? (x.LatestProceeding.ProceedingDate.Value.ToString("dd/MM/yyyy")) : "",
                            NextDate = x.LatestProceeding != null ? (x.LatestProceeding.NextDate.Value.ToString("yyyy-MM-dd")) : ""
                        })
                        .OrderByDescending(o => o.Year)
                        .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return cases;
        }
    }
}
