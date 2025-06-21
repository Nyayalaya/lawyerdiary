using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.DTOs.Dashboard;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Dashboard
{
    public class DashboardQueryCommand : IApplicationLayer, IRequest<Result<DashboardDto>>
    {
        public List<string> LinkedIds { get; set; }
    }
    public class DashboardQueryCommandHandler : IRequestHandler<DashboardQueryCommand, Result<DashboardDto>>
    {

        private readonly IUserCaseRepository _repository;
        private readonly ICaseProceedingRepository _ProcRepo;
        private readonly ICaseAssignedRepository _assignRepo;
        public DashboardQueryCommandHandler(IUserCaseRepository _repository,
            ICaseProceedingRepository procRepo,
            ICaseAssignedRepository assignRepo)
        {
            this._repository = _repository;
            _ProcRepo = procRepo;
            _assignRepo = assignRepo;
        }
        public async Task<Result<DashboardDto>> Handle(DashboardQueryCommand request, CancellationToken cancellationToken)
        {
            var baseQuery = (from c in _repository.Entites
                             join ac in _assignRepo.Entities
                             on c.Id equals ac.CaseId into caseAssignments
                             from ac in caseAssignments.DefaultIfEmpty()
                             where request.LinkedIds.Contains(c.CreatedBy)
                             || request.LinkedIds.Contains(ac.LawyerId.ToString()) // Check if user is the creator or assigned lawyer
                             let asignedOrSelf = ac != null && request.LinkedIds.Contains(ac.LawyerId.ToString()) ? "Assigned" : "Self"
                             let isCaseAssigned = asignedOrSelf == "Self" && ac != null && ac.CaseId == c.Id
                             let AssignedLawyerId = asignedOrSelf == "Self" && ac != null ? ac.LawyerId : Guid.Empty
                             select new GetCaseInfoDto
                             {
                                 Id = c.Id,
                                 InstitutionDate=c.InstitutionDate,
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
                                 NextDate = c.CaseProcEntities
                                     .OrderByDescending(o => o.NextDate.Value) // Order by latest date
                                     .Select(s => s.NextDate.Value.ToString("dd-MM-yyyy"))
                                     .FirstOrDefault() ?? (c.NextDate.HasValue ? c.NextDate.Value.ToString("dd-MM-yyyy") : "")
                             })
                   .OrderByDescending(o => o.Year)
                   .AsQueryable();

            var caseList = baseQuery.ToList();

            var totalCase = caseList.Count();

            var assignedCase = caseList.Count(c => c.Reference == "ASSIGNED");

            var disposalCase = caseList.Count(c => c.DisposalDate.HasValue);

            var pendingCase = totalCase - disposalCase;

            var today = DateTime.Now.Date;
            var todayHearing = caseList.Count(s =>
                DateTime.TryParseExact(s.NextDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate)
                && parsedDate.Date == today
            );

            var statusWiseSummary = new List<CaseStatusSummaryDto>
            {
                new() { Status = "DISPOSED", Count = disposalCase },
                new() { Status = "PENDING", Count = pendingCase },
                new() { Status = "ASSIGNED", Count = assignedCase },
                new() { Status = "SELF", Count = caseList.Count(c => c.Reference == "SELF") }
            };

            var upcomingHearings = caseList.Where(c => !string.IsNullOrWhiteSpace(c.NextDate))
                           .OrderBy(c => DateTime.ParseExact(c.NextDate, "dd-MM-yyyy", null))
                           .Select(c => new NextHearingItemDto
                           {
                               CaseTitle = c.CaseDetail,
                               HearingDate = c.NextDate,
                               CaseId = c.Id,
                               CourtName = c.Court,
                               OpponentName = ""
                           }).ToList();

            // Group by InstitutionDate (Filed Cases)
            var filedGroups = caseList
                .GroupBy(c => new
                {
                    Year = c.InstitutionDate.Year,
                    Month = c.InstitutionDate.Month
                })
                .Select(g => new MonthlyCaseStatusDto
                {
                    Year = g.Key.Year,
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"), // Jan, Feb, etc.
                    Filed = g.Count(),
                    Disposed = 0
                }).ToList();

            // Group by DisposalDate (Disposed Cases)
            var disposedGroups = caseList
                .Where(c => c.DisposalDate.HasValue)
                .GroupBy(c => new
                {
                    Year = c.DisposalDate.Value.Year,
                    Month = c.DisposalDate.Value.Month
                })
                .Select(g => new MonthlyCaseStatusDto
                {
                    Year = g.Key.Year,
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    Filed = 0,
                    Disposed = g.Count()
                }).ToList();

            // Merge both into monthly case statuses
            var monthlyCaseStatuses = filedGroups
                .Concat(disposedGroups)
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new MonthlyCaseStatusDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Filed = g.Sum(x => x.Filed),
                    Disposed = g.Sum(x => x.Disposed)
                })
                .OrderByDescending(x => x.Year)
                .ThenBy(x => DateTime.ParseExact(x.Month, "MMM", null))
                .ToList();

            var finalDashboardData = new DashboardDto()
            {
                TotalCases = totalCase,
                DisposedCases = disposalCase,
                AssignedCases = assignedCase,
                PendingCases = pendingCase,
                TodayHearing= todayHearing,
                StatusSummaries = statusWiseSummary,
                MonthlyCaseStatuses = monthlyCaseStatuses,
                UpcomingHearings = upcomingHearings
            };

            return await Result<DashboardDto>.SuccessAsync(finalDashboardData);
        }
    }
}
