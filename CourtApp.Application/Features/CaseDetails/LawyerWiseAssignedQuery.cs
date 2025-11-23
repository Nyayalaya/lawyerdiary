using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Case;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CaseDetails
{
    public class LawyerWiseAssignedQuery : IRequest<Result<List<LawyerWiseAssignedCaseDto>>>
    {
        public List<string> LinkedIds { get; set; }
    }
    public class LawyerWiseAssignedQueryHandler : IRequestHandler<LawyerWiseAssignedQuery, Result<List<LawyerWiseAssignedCaseDto>>>
    {

        private readonly IUserCaseRepository _userCaseRepository;
        private readonly ICaseAssignedRepository _caseAssignedRepository;
        public LawyerWiseAssignedQueryHandler(ICaseAssignedRepository caseAssignedRepository, IUserCaseRepository userCaseRepository)
        {
            _caseAssignedRepository = caseAssignedRepository;
            _userCaseRepository = userCaseRepository;
        }
        public async Task<Result<List<LawyerWiseAssignedCaseDto>>> Handle(LawyerWiseAssignedQuery request, CancellationToken cancellationToken)
        {
            var loggedInUserCaseDetail = (from _us in _userCaseRepository.Entites
                                          where request.LinkedIds.Contains(_us.CreatedBy)
                                          join _ac in _caseAssignedRepository.Entities on _us.Id equals _ac.Id
                                          select new
                                          {
                                              LawyerId = _ac.LawyerId,
                                              CaseId = _us.Id,
                                              FirstTitleCode = _us.FTitle.Name_En,
                                              FirstTitle = _us.FirstTitle,
                                              SecondTitleCode = _us.STitle.Name_En,
                                              SecondTitle = _us.SecondTitle,
                                              Year = _us.CaseYear,
                                              No = _us.CaseNo,
                                              CourtType = _us.CourtType.CourtType,
                                              CourtName = _us.CourtBench.CourtBench_En,
                                              CaseType = _us.CaseType.Name_En,
                                              CaseStage = _us.CaseStage.CaseStage,
                                              DisposalDate = _us.DisposalDate,
                                              CaseDetail = _us.FirstTitle + " V/S " + _us.SecondTitle,
                                              NextDate = _us.CaseProcEntities
                                                          .OrderByDescending(o => o.NextDate.Value) // Order by latest date
                                                          .Select(s => s.NextDate.Value.ToString("dd-MM-yyyy"))
                                                          .FirstOrDefault() ?? (_us.NextDate.HasValue ? _us.NextDate.Value.ToString("dd-MM-yyyy") : "")
                                          }).ToList();
            var fnlResult = loggedInUserCaseDetail
                .GroupBy(x => x.LawyerId)
                .Select(g => new LawyerWiseAssignedCaseDto
                {
                    LawyerId = g.Key,
                    AssignedCaseInfo = g.Select(caseInfo => new CaseDetailResponse
                    {
                        Id = caseInfo.CaseId,
                        FTitleType = caseInfo.FirstTitleCode,
                        FirstTitle = caseInfo.FirstTitle,
                        STitleType = caseInfo.SecondTitleCode,
                        SecondTitle = caseInfo.SecondTitle,
                        CaseYear = caseInfo.Year.ToString(),
                        CaseNumber = caseInfo.No,
                        CourtType = caseInfo.CourtType,
                        CourtName = caseInfo.CourtName,
                        CaseTypeName = caseInfo.CaseType,
                        CaseStage = caseInfo.CaseStage,
                        CaseTitle = caseInfo.CaseDetail,
                        NextHearingDate = caseInfo.NextDate != "" ? Convert.ToDateTime(caseInfo.NextDate) : (default)
                    }).ToList()
                }).ToList();

            return await Result<List<LawyerWiseAssignedCaseDto>>.SuccessAsync(fnlResult);
        }
    }
}
