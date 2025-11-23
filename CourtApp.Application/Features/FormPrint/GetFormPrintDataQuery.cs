using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.DTOs.FormPrint;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Domain.Entities.CaseDetails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CourtApp.Application.Constants.Permissions;

namespace CourtApp.Application.Features.FormPrint
{
    public class GetFormPrintDataQuery : IRequest<Result<List<GlobalFormPrintDto>>>
    {
        public List<Guid> CaseIds { get; set; }
    }
    public class GetFormPrintDataQueryHandler : IRequestHandler<GetFormPrintDataQuery, Result<List<GlobalFormPrintDto>>>
    {

        private readonly IUserCaseRepository _CaseRepo;
        private readonly ICaseProceedingRepository _wRepo;
        private readonly IMapper _mapper;
        private readonly IClientRepository _ClientRepo;
        private readonly IFSTitleRepository _AppeareceRepo;
        private readonly ICaseTitleRepository _TitleRepo;
        public GetFormPrintDataQueryHandler(IUserCaseRepository _CaseRepo, IMapper _mapper,
            ICaseProceedingRepository wRepo, IClientRepository clientRepo, IFSTitleRepository _AppeareceRepo, ICaseTitleRepository _TitleRepo)
        {
            this._CaseRepo = _CaseRepo;
            this._mapper = _mapper;
            _wRepo = wRepo;
            _ClientRepo = clientRepo;
            this._AppeareceRepo = _AppeareceRepo;
            this._TitleRepo = _TitleRepo;
        }
        public async Task<Result<List<GlobalFormPrintDto>>> Handle(GetFormPrintDataQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var casesQuery = await _CaseRepo.Entites
                        .Include(s => s.State)
                        .Include(s => s.CourtType)
                        .Include(s => s.CaseCategory)
                        .Include(s => s.CourtDistrict)
                        .Include(s => s.Complex)
                        .Include(s => s.CourtBench)
                        .Include(s => s.CaseType)
                        .Include(s => s.CaseStage)
                        .Include(s => s.FTitle)
                        .Include(s => s.STitle)
                        .Include(s => s.Titles).ThenInclude(t => t.CaseApplicants)
                        .Include(a => a.CaseAgainstEntities).ThenInclude(s => s.State)
                        .Include(a => a.CaseAgainstEntities).ThenInclude(s => s.CourtType)
                        .Include(a => a.CaseAgainstEntities).ThenInclude(s => s.CourtDistrict)
                        .Include(a => a.CaseAgainstEntities).ThenInclude(s => s.Complex)
                        .Include(a => a.CaseAgainstEntities).ThenInclude(s => s.CourtBench)
                        .Include(a => a.CaseAgainstEntities).ThenInclude(s => s.CaseType)
                        .Include(a => a.CaseAgainstEntities).ThenInclude(s => s.CaseCategory)
                        .Include(a => a.CaseAgainstEntities).ThenInclude(s => s.Cadre)
                        .AsNoTracking()
                        .Where(w => request.CaseIds.Contains(w.Id))
                        .ToListAsync();

                var result = casesQuery.Select(x => new GlobalFormPrintDto
                {
                    InstitutionDate = x.InstitutionDate.ToString("dd/MM/yyyy"),                    
                    CaseNoYear = string.IsNullOrWhiteSpace(x.CaseNo) || x.CaseNo == "0"
                                ? $"{x.CaseYear}"
                                : $"{x.CaseNo}/{x.CaseYear}",                    
                    CisNoYear = string.IsNullOrWhiteSpace(x.CisNumber) || x.CisNumber == "0"
                                ? $"{x.CisYear}"
                                : $"{x.CisNumber}/{x.CisYear}",                    
                    FirstPartyDetails = x.Titles.Where(s => s.TypeId == 1)
                    .SelectMany(s => s.CaseApplicants.Select(app => new ApplicantDetailDto
                    {
                        Applicant = app.ApplicantDetail,
                        ApplicantNo = app.ApplicantNo
                    })).ToList(),
                    SecondPartyDetails = x.Titles.Where(s => s.TypeId == 2)
                    .SelectMany(s => s.CaseApplicants.Select(app => new ApplicantDetailDto
                    {
                        Applicant = app.ApplicantDetail,
                        ApplicantNo = app.ApplicantNo
                    })).ToList(),
                    State = x.State != null ? x.State.Name_En : "",
                    Strength = x.StrengthId == 1 ? "S.B." : "D.B.",
                    CourtType = x.CourtType != null ? x.CourtType.CourtType : "",
                    CaseCategory = x.CaseCategory != null ? x.CaseCategory.Name_En : "",
                    CaseType = x.CaseType != null ? x.CaseType.Name_En : "",
                    CourtDistrict = x.CourtDistrict != null ? x.CourtDistrict.Name_En : "",
                    CourtComplex = x.Complex != null ? x.Complex.Name_En : "",
                    Court = x.CourtBench != null ? x.CourtBench.CourtBench_En : "",
                    PetitionerAppearance = x.FTitle != null ? x.FTitle.Name_En : "",
                    Petitioner = x.FirstTitle,
                    RespondantAppearance = x.STitle != null ? x.STitle.Name_En : "",
                    Respondent = x.SecondTitle,
                    CaseStage = x.CaseStage != null ? x.CaseStage.CaseStage : "",
                    NextDate = GetLatestNextDate(x),
                    CnrNo = x.CnrNumber,
                    DisposalDate = x.DisposalDate?.ToString("dd/MM/yyyy") ?? "",
                    AgainstCourtDetail = x.CaseAgainstEntities.Select(s => new AgainstCaseDetail
                    {
                        ImpugedOrder = s.ImpugedOrderDate.ToString("dd/MM/yyyy"),
                        State = s.State != null ? s.State.Name_En : "",
                        CourtType = s.CourtType != null ? s.CourtType.CourtType : "",
                        CourtDistrict = s.CourtDistrict != null ? s.CourtDistrict.Name_En : "",
                        CourtComplex = s.Complex != null ? s.Complex.Name_En : "",
                        CourtBench = s.CourtBench != null ? s.CourtBench.CourtBench_En : "",
                        CaseNo = s.CaseNo != null ? s.CaseNo.ToString() : "",
                        CaseYear = s.CaseYear.ToString(),
                        CaseType = s.CaseType != null ? s.CaseType.Name_En : "",
                        CisNo = s.CisNo != null ? s.CisNo.ToString() : "",
                        CisYear = s.CisYear.ToString(),
                        CisNoYear = string.IsNullOrWhiteSpace(s.CisNo) || s.CisNo == "0"
                                ? $"{s.CisYear}"
                                : $"{s.CisNo}/{s.CisYear}",
                        CnrNo = s.CnrNo != null ? s.CnrNo.ToString() : "",
                        Cadre = s.Cadre != null ? s.Cadre.Name_En : "",
                        OfficerName = s.OfficerName ?? "",
                        CaseCategory = s.CaseCategory != null ? s.CaseCategory.Name_En : "",
                        DistrictCourt = s.CourtDistrict != null ? s.CourtDistrict.Name_En : ""
                    }).FirstOrDefault(),
                }).ToList();
                return await Result<List<GlobalFormPrintDto>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at Get Query Form:" + ex);
                return null;
            }
        }

        private AgainstCaseDetail GetAgainsCaseDetail(CaseDetailEntity caseInfo)
        {
            if (caseInfo?.CaseAgainstEntities == null || !caseInfo.CaseAgainstEntities.Any())
            {
                return null;
            }

            var agstCaseDetail = caseInfo.CaseAgainstEntities.Select(s => new AgainstCaseDetail
            {
                ImpugedOrder = s.ImpugedOrderDate.ToString("dd/MM/yyyy"),
                State = s.State?.Name_En,
                CourtType = s.CourtType?.CourtType.ToString(),
                CourtDistrict = s.CourtDistrict?.Name_En ?? "",
                CourtComplex = s.Complex?.Name_En ?? "",
                CourtBench = s.CourtBench?.CourtBench_En ?? "",
                CaseNo = s.CaseNo?.ToString() ?? "",
                CaseYear = s.CaseYear.ToString(),
                CaseType = s.CaseType?.Name_En ?? "",
                CisNo = s.CisNo?.ToString() ?? "",
                CisYear = s.CisYear.ToString(),
                CnrNo = s.CnrNo?.ToString() ?? "",
                Cadre = s.Cadre?.Name_En ?? "",
                OfficerName = s.OfficerName ?? "",
                CaseCategory = s.CaseCategory?.Name_En ?? "",
                DistrictCourt = s.CourtDistrict?.Name_En ?? "",
            }).FirstOrDefault();
            return agstCaseDetail;
        }


        private string GetLatestNextDate(CaseDetailEntity caseInfo)
        {
            var procDates = caseInfo.CaseProcEntities?
                .Where(p => p.NextDate.HasValue)
                .Select(p => p.NextDate.Value)
                .ToList();

            var maxProcDate = procDates?.Any() == true ? procDates.Max() : DateTime.MinValue;

            if (caseInfo.NextDate.HasValue && caseInfo.NextDate > maxProcDate)
                return caseInfo.NextDate.Value.ToString("dd/MM/yyyy");

            return maxProcDate != DateTime.MinValue ? maxProcDate.ToString("dd/MM/yyyy") : "";
        }

    }
}
