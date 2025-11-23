using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.FormPrint;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.FormPrint
{
    public class GetShowCauseNoticeQuery : IRequest<Result<List<ShowCauseNoticeResponse>>>
    {
        public List<Guid> CaseIds { get; set; }
        public string ApplicantNo { get; set; }
    }
    public class GetShowCauseNoticeQueryHandler : IRequestHandler<GetShowCauseNoticeQuery, Result<List<ShowCauseNoticeResponse>>>
    {
        private readonly IUserCaseRepository _CaseRepo;
        private readonly ICaseTitleRepository _TitleRepo;
        public GetShowCauseNoticeQueryHandler(IUserCaseRepository _CaseRepo, ICaseTitleRepository _TitleRepo)
        {
            this._CaseRepo = _CaseRepo;
            this._TitleRepo = _TitleRepo;
        }

        public async Task<Result<List<ShowCauseNoticeResponse>>> Handle(GetShowCauseNoticeQuery request, CancellationToken cancellationToken)
        {
            List<string> Titles = new List<string>();
            if (request.ApplicantNo != null)
                Titles = request.ApplicantNo.Split(',').ToList();
            var query = _CaseRepo.Entites
                .Include(c => c.CaseCategory)
                .Include(t => t.Titles.Where(t => t.TypeId == 2))
                    .ThenInclude(a => a.CaseApplicants)
                .Where(c => request.CaseIds.Contains(c.Id))
                .Select(s => new ShowCauseNoticeResponse
                {
                    CaseNoYear = s.CaseNo + "/" + s.CaseYear,
                    CaseType = s.CaseCategory.Name_En,
                    Petitioner = s.FirstTitle,
                    Respondent = s.SecondTitle,
                    Applicants = s.Titles.Where(t => t.TypeId == 2).SelectMany(t => t.CaseApplicants)
                                .Where(a => (request.ApplicantNo == null || Titles.Contains(a.ApplicantNo)))
                                .Select(a => new ApplicantDetailDto
                                {
                                    ApplicantNo = a.ApplicantNo,
                                    Applicant = a.ApplicantDetail
                                }).ToList()
                }).ToList();
            return await Result<List<ShowCauseNoticeResponse>>.SuccessAsync(query.ToList());
        }
    }
}

