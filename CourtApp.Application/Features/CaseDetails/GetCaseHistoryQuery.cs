using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CaseDetails;
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
    public class GetCaseHistoryQuery : IRequest<Result<CaseHistoryResposnse>>
    {
        public Guid CaseId { get; set; }
    }
    public class GetCaseHistoryQueryHandler : IRequestHandler<GetCaseHistoryQuery, Result<CaseHistoryResposnse>>
    {
        private readonly IUserCaseRepository _CaseRepo;
        private readonly IWorkMasterRepository _WorkRepo;
        private readonly IWorkMasterSubRepository _WorkSRepo;
        private readonly ICaseProceedingRepository _ProceedingRepo;
        private readonly IMapper _mapper;
        private readonly ICaseDocsRepository _CaseDocRepo;
        public GetCaseHistoryQueryHandler(IUserCaseRepository _CaseRepo,
            IMapper _mapper,
            IWorkMasterRepository _WorkRepo,
            ICaseProceedingRepository _ProceedingRepo,
            ICaseDocsRepository caseDocRepo,
            IWorkMasterSubRepository _WorkSRepo)
        {
            this._CaseRepo = _CaseRepo;
            this._mapper = _mapper;
            this._WorkRepo = _WorkRepo;
            this._ProceedingRepo = _ProceedingRepo;
            _CaseDocRepo = caseDocRepo;
            this._WorkSRepo = _WorkSRepo;

        }
        public async Task<Result<CaseHistoryResposnse>> Handle(GetCaseHistoryQuery request, CancellationToken cancellationToken)
        {
            var detail = await _CaseRepo.GetByIdAsync(request.CaseId);
            if (detail != null)
            {
                var mappeddata = _mapper.Map<UserCaseDetailResponse>(detail);
                CaseHistoryResposnse chr = new CaseHistoryResposnse();
                chr.Id = request.CaseId;
                chr.CaseNoYear = detail.CaseNo + "/" + detail.CaseYear;
                chr.Title = (detail.FirstTitle + " Vs " + detail.SecondTitle).ToUpper();
                chr.CourtType = detail.CourtType != null ? detail.CourtType.CourtType.ToUpper() : "";
                chr.Court = detail.CourtBench != null ? detail.CourtBench.CourtBench_En.ToUpper() : "";
                var cprocs = await _ProceedingRepo.GetProceedingByCaseIdAsync(request.CaseId);
                var PWorks = cprocs.GroupBy(pd => pd.ProceedingDate)
                    .Select(g => new
                    {
                        ProceedingDate = g.Key,
                        works = g.Select(s => s.ProcWork)
                                .SelectMany(w => w.Works)
                                .ToList()
                    }).ToList();

                var result = PWorks.Select(pw =>
                {
                    // Group works by WorkId and select the first entry or aggregate them
                    var WDetails = pw.works
                        .Where(w => w.WorkId != Guid.Empty)
                        .GroupBy(w => w.WorkId)
                        .ToDictionary(
                            g => g.Key,
                            g => new
                            {
                                g.First().Status,
                                g.First().AppliedOn,
                                g.First().ReceivedOn,
                                g.First().WorkTypeId
                            });

                    var workDetails = _WorkSRepo.Entities
                        .Where(w => pw.works.Select(x => x.WorkId).Contains(w.Id)) // Filter only matching work IDs
                        .Select(w => new
                        {
                            WorkID = w.Id,
                            WorkName = w.Name_En,
                            WorkType = w.Work.Work_En,
                            WorkStatus = WDetails.ContainsKey(w.Id) ? WDetails[w.Id].Status : 0,
                            WorkDoneDate = WDetails.ContainsKey(w.Id)
                                ? (WDetails[w.Id].Status == 1
                                    ? WDetails[w.Id].AppliedOn.ToString("dd/MM/yyyy")
                                    : (WDetails[w.Id].Status == 2 ? WDetails[w.Id].ReceivedOn.ToString("dd/MM/yyyy") : null))
                                : null,
                            AppliedOn=WDetails.ContainsKey(w.Id)
                                    ? (WDetails[w.Id].Status == 2 ? WDetails[w.Id].AppliedOn.ToString("dd/MM/yyyy") : null):null
                        })
                        .ToList();

                    return new
                    {
                        pw.ProceedingDate,
                        WorkDetails = workDetails
                    };
                }).ToList();

                var cprocdt = cprocs
                    .Where(w => w.CaseId == request.CaseId)
                    .Select(s => new CaseHistoryData
                    {
                        NextDate = s.NextDate?.ToString("dd/MM/yyyy") ?? "",
                        Stage = s.StageId != null ? s.Stage.CaseStage : "",
                        Activity = s.SubHead.Name_En,
                        Type = s.Head.Name_En,
                        Date = (s.ProceedingDate ?? s.CreatedOn),
                        WorkDetail = s.ProcWork != null ? new List<CaseWorkDetail>
                        {
                            new CaseWorkDetail
                            {
                                WorkingDate = s.ProcWork.LastWorkingDate?.ToString("dd/MM/yyyy") ??"",
                                Works = result
                                    .Where(wd => wd.ProceedingDate == s.ProceedingDate) // Match by ProceedingDate
                                    .SelectMany(wd => wd.WorkDetails)
                                    .Select(w => new DTOs.CaseDetails.CaseWork
                                    {
                                        WorkType = w.WorkType,
                                        Work = w.WorkName,
                                        Status=w.WorkStatus==1?"Work Done":w.WorkStatus==2?"Copy Recieved":"",
                                        Date=w.WorkDoneDate,
                                        AppliedOn=w.AppliedOn
                                    })
                                    .ToList()
                            }
                        }
                        : new List<CaseWorkDetail>()
                    })
                    .ToList();

                var Docs = _CaseDocRepo
                    .Entities
                    .Include(d => d.DO)
                    .Where(w => w.CaseId == request.CaseId).Select(s => new CaseUploadedDocument
                    {
                        Id = s.Id,
                        DocType = s.DOTypeId == 1 ? "Drafting" : "Order",
                        DocFilePath = s.Path,
                        DocName = s.DO.Name_En.ToUpper(),
                        DocDate = s.DocDate.ToString("dd/MM/yyyyy")
                    }).ToList();

                var historydt = cprocdt;
                chr.History = historydt.ToList();
                chr.Docs = Docs;
                return Result<CaseHistoryResposnse>.Success(chr);
            }
            return null;
        }
    }
}
