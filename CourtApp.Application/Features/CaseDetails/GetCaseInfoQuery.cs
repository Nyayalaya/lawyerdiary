using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.Extensions;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using KT3Core.Areas.Global.Classes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace CourtApp.Application.Features.CaseDetails
{
    public class GetCaseInfoQuery : IRequest<PaginatedResult<GetCaseInfoDto>>
    {
        public string CaseNumber { get; set; }
        public int Year { get; set; }
        public List<string> LinkedIds { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
    }

    public class GetCaseInfoQueryHandler : IRequestHandler<GetCaseInfoQuery, PaginatedResult<GetCaseInfoDto>>
    {
        private readonly IUserCaseRepository _repository;
        private readonly ICaseProceedingRepository _ProcRepo;
        private readonly ICaseAssignedRepository _assignRepo;
        public GetCaseInfoQueryHandler(IUserCaseRepository _repository,
            ICaseProceedingRepository procRepo,
            ICaseAssignedRepository assignRepo)
        {
            this._repository = _repository;
            _ProcRepo = procRepo;
            _assignRepo = assignRepo;
        }
        public async Task<PaginatedResult<GetCaseInfoDto>> Handle(GetCaseInfoQuery request, CancellationToken cancellationToken)
        {
            var predicate = PredicateBuilder.True<CaseDetailEntity>();

            // Filtering
            //if (request.LinkedIds?.Any() == true)
            //    predicate = predicate.And(c => request.LinkedIds.Contains(c.CreatedBy));

            if (request.Year != 0)
                predicate = predicate.And(c => c.CaseYear == request.Year);

            if (!string.IsNullOrWhiteSpace(request.CaseNumber))
                predicate = predicate.And(c => c.CaseNo == request.CaseNumber);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                DateTime parsedDate;
                bool isDateSearch = DateTime.TryParseExact(search, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
                int parsedYear;
                bool isYearSearch = int.TryParse(search, out parsedYear);
                predicate = predicate.And(c =>
                    c.CaseNo.ToLower().Contains(search) ||
                    c.FirstTitle.ToLower().Contains(search) ||
                    c.SecondTitle.ToLower().Contains(search) ||
                    c.CaseStage.CaseStage.ToLower().Contains(search) ||
                    (isYearSearch && c.CaseYear == parsedYear) ||
                    (isDateSearch && c.NextDate.HasValue && c.NextDate.Value.Date == parsedDate.Date) ||
                    c.CaseType.Name_En.ToLower().Contains(search));
            }

            try
            {
                var baseQuery = (from c in _repository.Entites.Where(predicate)
                                 join ac in _assignRepo.Entities
                                 on c.Id equals ac.CaseId into caseAssignments
                                 from ac in caseAssignments.DefaultIfEmpty()
                                 where request.LinkedIds.Contains(c.CreatedBy)
                                 || request.LinkedIds.Contains(ac.LawyerId.ToString()) // Check if user is the creator or assigned lawyer
                                 let asignedOrSelf = ac != null && request.LinkedIds.Contains(ac.LawyerId.ToString()) ? "Assigned" : "Self"
                                 let isCaseAssigned = asignedOrSelf == "Self" && ac != null && ac.CaseId == c.Id
                                 let AssignedLawyerId = asignedOrSelf == "Self" && ac != null ? ac.LawyerId : Guid.Empty
                                 let caseLastProceedingDate = c.CaseProcEntities.Any() ?
                                                       c.CaseProcEntities
                                                       .OrderByDescending(d => d.ProceedingDate)
                                                       .Select(s => new { s.ProceedingDate, s.NextDate })
                                                       .FirstOrDefault() : null
                                 let prcDate = caseLastProceedingDate != null ? caseLastProceedingDate.ProceedingDate : (DateTime?)null
                                 let nextProcDate = caseLastProceedingDate != null ? caseLastProceedingDate.NextDate : (DateTime?)null
                                 let latestNextDate = (nextProcDate != null && prcDate!=null && nextProcDate >= prcDate ? nextProcDate : prcDate).Value
                                 let caseFirstDate = c.NextDate != null ? c.NextDate.Value.ToString("dd-MM-yyyy") : ""
                                 let caseLatestNextDate = (prcDate == null && nextProcDate == null) && c.NextDate != null ? caseFirstDate : latestNextDate.ToString("dd-MM-yyyy")
                                 select new GetCaseInfoDto
                                 {
                                     Id = c.Id,
                                     Reference = asignedOrSelf.ToUpper(),
                                     IsCaseAssigned = isCaseAssigned,
                                     LawyerId = AssignedLawyerId,
                                     No = c.CaseNo,
                                     Year = c.CaseYear.ToString(),
                                     CourtType = c.CourtType.CourtType.ToString(),
                                     CaseType = c.CaseType.Name_En,
                                     Court = c.CourtBench.CourtBench_En.ToUpper(),
                                     CaseStage = c.CaseStage.CaseStage.ToUpper(),
                                     DisposalDate = c.DisposalDate,
                                     CaseDetail = (c.FirstTitle + " V/S " + c.SecondTitle).ToUpper(),
                                     NextDate = caseLatestNextDate
                                 })
                   .OrderByDescending(o => o.Year)
                   .AsQueryable();

                // Optional: apply sorting before pagination
                if (!string.IsNullOrEmpty(request.SortColumn))
                {
                    switch (request.SortColumn)
                    {
                        case "No":
                            baseQuery = request.SortDirection == "asc"
                                ? baseQuery.OrderBy(x => x.No)
                                : baseQuery.OrderByDescending(x => x.No);
                            break;

                        case "Year":
                            baseQuery = request.SortDirection == "asc"
                                ? baseQuery.OrderBy(x => x.Year)
                                : baseQuery.OrderByDescending(x => x.Year);
                            break;
                        case "Reference":
                            baseQuery = request.SortDirection == "asc"
                                ? baseQuery.OrderBy(x => x.Reference)
                                : baseQuery.OrderByDescending(x => x.Reference);
                            break;
                        case "NextDate":
                            baseQuery = request.SortDirection == "asc"
                                ? baseQuery.OrderBy(x => x.NextDate)
                                : baseQuery.OrderByDescending(x => x.NextDate);
                            break;

                        case "Stage":
                            baseQuery = request.SortDirection == "asc"
                                ? baseQuery.OrderBy(x => x.CaseStage)
                                : baseQuery.OrderByDescending(x => x.CaseStage);
                            break;
                        case "Type":
                            baseQuery = request.SortDirection == "asc"
                                ? baseQuery.OrderBy(x => x.CaseType)
                                : baseQuery.OrderByDescending(x => x.CaseType);
                            break;

                        default:
                            baseQuery = baseQuery.OrderByDescending(x => x.Year);
                            break;
                    }
                }

                // Apply distinct if required (EF Core does not support DistinctBy directly)
                var distinctQuery = baseQuery
                    .GroupBy(c => c.Id)
                    .Select(g => g.FirstOrDefault());
                int pageSize = request.PageSize == -1 ? distinctQuery.Count() : request.PageSize;
                return await distinctQuery.ToPaginatedListAsync(request.PageNumber, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while processing cases: " + ex);
                return PaginatedResult<GetCaseInfoDto>.Failure(new List<string>());
            }
        }
    }
}
