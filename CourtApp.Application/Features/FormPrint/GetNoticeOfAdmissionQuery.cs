using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.FormPrint;
using CourtApp.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.FormPrint
{
    public class GetNoticeOfAdmissionQuery : IRequest<Result<List<NoticeOfAdmissionResponse>>>
    {
        public List<Guid> CaseIds { get; set; }
    }
    public class GetNoticeOfAdmissionQueryHandler : IRequestHandler<GetNoticeOfAdmissionQuery, Result<List<NoticeOfAdmissionResponse>>>
    {
        private readonly IUserCaseRepository _CaseRepo;
        private readonly ICaseTitleRepository _TitleRepo;
        public GetNoticeOfAdmissionQueryHandler(IUserCaseRepository _CaseRepo,
            ICaseTitleRepository _TitleRepo)
        {
            this._CaseRepo = _CaseRepo;
            this._TitleRepo = _TitleRepo;
        }
        public async Task<Result<List<NoticeOfAdmissionResponse>>> Handle(GetNoticeOfAdmissionQuery request, CancellationToken cancellationToken)
        {
            var Results = from c in _CaseRepo.Entites.Where(w => request.CaseIds.Contains(w.Id))
                          join t in _TitleRepo.Titles on c.Id equals t.CaseId into CompTitles
                          from ct in CompTitles.DefaultIfEmpty()
                          let caseApplicant = ct.TypeId == 1 ? ct.CaseApplicants.ToList() : null
                          select new NoticeOfAdmissionResponse
                          {
                              Applent = c.FirstTitle,
                              Respondent = c.SecondTitle,
                              NoYear = c.CaseNo + "/" + c.CaseYear,
                              CaseCategory = c.CaseCategory.Name_En,
                              CaseType = c.CaseType.Name_En,
                              CivilNoYear = c.CaseNo,
                              AgainstCourt = "",
                              Bench = c.CourtBench.CourtBench_En,
                              CompleteTitle = ct.CaseApplicants.FirstOrDefault().ApplicantDetail,
                              TitleNo = ct.CaseApplicants.FirstOrDefault().ApplicantNo,
                              Date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy")
                          };

            return await Result<List<NoticeOfAdmissionResponse>>.SuccessAsync(Results.ToList());
        }
    }
}
