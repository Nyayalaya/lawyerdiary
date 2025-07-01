using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Case;
using CourtApp.Application.Extensions;
using CourtApp.Application.Interfaces.CacheRepositories;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using KT3Core.Areas.Global.Classes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.UserCase
{
    public class GetCaseDetailsQuery : IRequest<PaginatedResult<CaseDetailResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public int Year { get; set; }
        public int StateCode { get; set; }
        public int DistrictCode { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid CourtId { get; set; }
        public Guid CaseTyepId { get; set; }
        public DateTime HearingDate { get; set; }
        public string CallingFrm { get; set; }
        public List<string> LinkedIds { get; set; }
        //public string UserId { get; set; }
    }
    public class GetCaseDetailsQueryHandler : IRequestHandler<GetCaseDetailsQuery, PaginatedResult<CaseDetailResponse>>
    {
        private readonly ICourtTypeCacheRepository _RepoCourtType;
        private readonly ICourtTypeRepository _RepoCrtType;
        private readonly ICaseStageCacheRepository _RepoStage;
        private readonly ICaseNatureCacheRepository _RepoNature;
        private readonly IUserCaseRepository _RepoCase;
        private readonly ICaseProceedingRepository _RepoProceeding;
        private readonly IFSTitleCacheRepository _RepoFSTitle;
        private readonly ICourtBenchRepository _RepoCourtBench;
        private readonly ICaseAssignedRepository _assignRepo;
        public GetCaseDetailsQueryHandler(ICaseNatureCacheRepository repoNature,
            IUserCaseRepository repoCase, ICourtTypeCacheRepository RepoCourtType,
            ICaseStageCacheRepository RepoStage, ICaseProceedingRepository repoProceeding,
            IFSTitleCacheRepository repoFSTitle, ICourtBenchRepository repoCourtBench,
            ICourtTypeRepository repoCrtType, ICaseAssignedRepository assignRepo)
        {
            _RepoNature = repoNature;
            _RepoCase = repoCase;
            _RepoCourtType = RepoCourtType;
            _RepoStage = RepoStage;
            _RepoProceeding = repoProceeding;
            _RepoFSTitle = repoFSTitle;
            _RepoCourtBench = repoCourtBench;
            _RepoCrtType = repoCrtType;
            _assignRepo = assignRepo;
        }
        public async Task<PaginatedResult<CaseDetailResponse>> Handle(GetCaseDetailsQuery request, CancellationToken cancellationToken)
        {
            // Step 1: Prepare raw EF Core query user wise data and its linked user data
            var baseQuery = from c in _RepoCase.Entites.AsNoTracking()
                            join ac in _assignRepo.Entities on c.Id equals ac.CaseId into caseAssignments
                            from ac in caseAssignments.DefaultIfEmpty()
                            where request.LinkedIds.Contains(c.CreatedBy)
                                  || request.LinkedIds.Contains(ac.LawyerId.ToString())
                            select new
                            {
                                Case = new
                                {
                                    c.Id,
                                    c.CaseNo,
                                    FTitleType = c.FTitle.Name_En,
                                    c.FirstTitle,
                                    STitleType = c.STitle.Name_En,
                                    c.SecondTitle,
                                    CaseYear = c.CaseYear.ToString(),
                                    CourtType = c.CourtType.CourtType.ToString(),
                                    CaseTypeName = c.CaseType.Name_En,
                                    CourtName = c.CourtBench.CourtBench_En,
                                    c.CaseStage.CaseStage,
                                    c.CaseProcEntities,
                                    c.NextDate
                                },
                                Assign = ac,
                                Refer = ac != null && request.LinkedIds.Contains(ac.LawyerId.ToString()) ? "Assigned" : "Self"
                            };

            // Optional: filter by hearing date (before loading to memory)
            if (request.HearingDate != default)
            {
                var hearingDate = request.HearingDate.Date;

                baseQuery = baseQuery.Where(e =>
                    e.Case.CaseProcEntities
                        .Any(p => p.ProceedingDate.HasValue && p.ProceedingDate.Value.Date == hearingDate)
                    || e.Case.CaseProcEntities
                        .Any(p => p.NextDate.HasValue && p.NextDate.Value.Date == hearingDate)
                    || e.Case.NextDate.HasValue && e.Case.NextDate.Value.Date == hearingDate);
            }

            // ✅ Step 2: Get total count BEFORE pagination
            var totalCount = await baseQuery.CountAsync();

            // ✅ Step 3: Paginate
            var pagedRawData = await baseQuery
                .OrderByDescending(e => e.Case.NextDate ?? e.Case.CaseProcEntities
                    .OrderByDescending(p => p.NextDate)
                    .Select(p => p.NextDate)
                    .FirstOrDefault()) // for ordering
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // ✅ Step 4: In-memory projection
            var results = pagedRawData.Select(e =>
            {
                var c = e.Case;
                var ac = e.Assign;
                var refer = e.Refer;

                var maxProcDate = c.CaseProcEntities != null
                        ? c.CaseProcEntities.OrderByDescending(p => p.NextDate)
                            .Select(p => p.NextDate)
                            .FirstOrDefault() ?? default
                        : (c.NextDate ?? default);



                var matchingProceeding = c.CaseProcEntities != null ?
                                         c.CaseProcEntities.FirstOrDefault(p =>
                                         p.ProceedingDate.HasValue
                                        && p.ProceedingDate.Value.Date == request.HearingDate.Date) : null;

                return new CaseDetailResponse
                {
                    Id = c.Id,
                    Reference = refer,
                    CaseNumber = c.CaseNo,
                    FTitleType = c.FTitleType,
                    FirstTitle = c.FirstTitle,
                    STitleType = c.FTitleType,
                    SecondTitle = c.SecondTitle,
                    CaseYear = c.CaseYear.ToString(),
                    CourtType = c.CourtType.ToString(),
                    CaseTypeName = c.CaseTypeName,
                    CourtName = c.CourtName,
                    CaseStage = c.CaseStage,
                    CaseTitle = (c.FirstTitle + " V/S " + c.SecondTitle + " [" +
                                 (string.IsNullOrEmpty(c.CaseNo)
                                     ? c.CaseYear.ToString()
                                     : c.CaseNo + "/" + c.CaseYear.ToString()) +
                                 "]").ToUpperInvariant(),
                    NextHearingDate = maxProcDate,
                    IsProceedingDone = matchingProceeding != null,
                    ProceedingDate = matchingProceeding?.ProceedingDate ?? default,
                    IsCaseAssigned = refer == "Self" && ac != null && ac.CaseId == c.Id,
                    LawyerId = refer == "Self" && ac != null ? ac.LawyerId : Guid.Empty,
                    HasChild = IsCaseHavingChild(c.Id)
                };
            }).ToList();

            // ✅ Step 6: Return as PaginatedResult
            return PaginatedResult<CaseDetailResponse>.Success(
                results,
                totalCount,
                request.PageNumber,
                request.PageSize
            );

            /* Older Code
            try
            {
                
                //Step 1: Getting all case of logged in user.
                var userCaseQuery = (from c in _RepoCase.Entites.AsNoTracking()
                                     join ac in _assignRepo.Entities on c.Id equals ac.CaseId into caseAssignments
                                     from ac in caseAssignments.DefaultIfEmpty()
                                     where request.LinkedIds.Contains(c.CreatedBy)
                                           || request.LinkedIds.Contains(ac.LawyerId.ToString())
                                     let refer = ac != null && request.LinkedIds.Contains(ac.LawyerId.ToString()) ? "Assigned" : "Self"
                                     let maxProcDate = c.CaseProcEntities.Any()
                                         ? c.CaseProcEntities
                                             .OrderByDescending(p => p.NextDate)
                                             .Select(p => p.NextDate)
                                             .FirstOrDefault() ?? default
                                         : (c.NextDate ?? default)
                                     let matchingProceeding = c.CaseProcEntities
                                         .FirstOrDefault(p => p.ProceedingDate.HasValue &&
                                                              p.ProceedingDate.Value.Date == request.HearingDate.Date)

                                     let isCaseAssigned = refer == "Self" && ac != null && ac.CaseId == c.Id
                                     let AssignedLawyerId = refer == "Self" && ac != null ? ac.LawyerId : Guid.Empty
                                     select new CaseDetailResponse
                                     {
                                         Id = c.Id,
                                         Reference = refer,
                                         CaseNumber = c.CaseNo,
                                         FTitleType = c.FTitle.Name_En,
                                         FirstTitle = c.FirstTitle,
                                         STitleType = c.STitle.Name_En,
                                         SecondTitle = c.SecondTitle,
                                         CaseYear = c.CaseYear.ToString(),
                                         CourtType = c.CourtType.CourtType.ToString(),
                                         CaseTypeName = c.CaseType.Name_En,
                                         CourtName = c.CourtBench.CourtBench_En,
                                         CaseStage = c.CaseStage.CaseStage,
                                         CaseTitle = (c.FirstTitle + " V/S " + c.SecondTitle + " [" +
                                                      (string.IsNullOrEmpty(c.CaseNo)
                                                          ? c.CaseYear.ToString()
                                                          : c.CaseNo + "/" + c.CaseYear.ToString()) +
                                                      "]").ToUpperInvariant(),
                                         NextHearingDate = maxProcDate,
                                         IsProceedingDone = matchingProceeding != null,
                                         ProceedingDate = matchingProceeding != null
                                             ? matchingProceeding.ProceedingDate.Value
                                             : default,
                                         IsCaseAssigned = isCaseAssigned,
                                         LawyerId = AssignedLawyerId,
                                         HasChild = IsCaseHavingChild(c.Id)
                                     })
                                    .OrderByDescending(o => o.CaseYear)
                                    .AsQueryable();

                // ✅ Step 2: Apply hearing date filter if provided (matches either ProceedingDate OR NextHearingDate)
                if (request.HearingDate != default)
                {
                    var hearingDate = request.HearingDate.Date;

                    userCaseQuery = userCaseQuery.Where(c =>
                        (c.ProceedingDate == hearingDate) // Proceeding on same date
                        || c.NextHearingDate.Date == hearingDate                                 // OR Next date matches
                    );
                }
                return await userCaseQuery
                            .OrderByDescending(c => c.NextHearingDate)
                            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }*/
        }


        // ✅ Check whether case has child or not.
        private bool IsCaseHavingChild(Guid caseId)
        {
            var childCase = _RepoCase.Entites.AsNoTracking().Where(c => c.LinkedCaseId == caseId);
            return childCase.Count()>0?true:false;
        }
    }
}
