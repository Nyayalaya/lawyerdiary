using AspNetCoreHero.Results;
using AutoMapper;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.DTOs.FormPrint;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.Common;
using CourtApp.Domain.Entities.CaseDetails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using static CourtApp.Application.Constants.Permissions;

namespace CourtApp.Application.Features.FormPrint
{
    public class GetFormPrintDataQuery : IRequest<Result<List<GlobalFormPrintDto>>>
    {
        public string Lang { get; set; }
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
        private readonly IMultiLangWordRepository _langRepo;
        public GetFormPrintDataQueryHandler(IUserCaseRepository _CaseRepo, IMapper _mapper,
            ICaseProceedingRepository wRepo, IClientRepository clientRepo, 
            IFSTitleRepository _AppeareceRepo, ICaseTitleRepository _TitleRepo, IMultiLangWordRepository langRepo)
        {
            this._CaseRepo = _CaseRepo;
            this._mapper = _mapper;
            _wRepo = wRepo;
            _ClientRepo = clientRepo;
            this._AppeareceRepo = _AppeareceRepo;
            this._TitleRepo = _TitleRepo;
            this._langRepo = langRepo;
        }
        public async Task<Result<List<GlobalFormPrintDto>>> Handle(GetFormPrintDataQuery request, CancellationToken cancellationToken)
        {
            try
            {
                string lang = request.Lang ?? "En";
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

                var result = new List<GlobalFormPrintDto>();
                foreach (var x in casesQuery)
                {
                    result.Add(new GlobalFormPrintDto
                    {
                        InstitutionDate = x.InstitutionDate.ToString("dd/MM/yyyy"),

                        CaseNoYear = string.IsNullOrWhiteSpace(x.CaseNo) || x.CaseNo == "0"
                            ? $"{x.CaseYear}"
                            : $"{x.CaseNo}/{x.CaseYear}",

                        CisNoYear = string.IsNullOrWhiteSpace(x.CisNumber) || x.CisNumber == "0"
                            ? $"{x.CisYear}"
                            : $"{x.CisNumber}/{x.CisYear}",

                        State = await T(x.State?.Name_En, lang),
                        CourtType = await T(x.CourtType?.CourtType, lang),
                        CaseCategory = await T(x.CaseCategory?.Name_En, lang),
                        CaseType = await T(x.CaseType?.Name_En, lang),
                        CourtDistrict = await T(x.CourtDistrict?.Name_En, lang),
                        CourtComplex = await T(x.Complex?.Name_En, lang),
                        Court = await T(x.CourtBench?.CourtBench_En, lang),
                        CaseStage = await T(x.CaseStage?.CaseStage, lang),

                        Strength = x.StrengthId == 1 ? "S.B." : "D.B.",

                        PetitionerAppearance = await T(x.FTitle?.Name_En, lang),
                        RespondantAppearance = await T(x.STitle?.Name_En, lang),

                        Petitioner = x.FirstTitle,
                        Respondent = x.SecondTitle,

                        NextDate = GetLatestNextDate(x),
                        CnrNo = x.CnrNumber,
                        DisposalDate = x.DisposalDate?.ToString("dd/MM/yyyy") ?? "",

                        FirstPartyDetails = x.Titles
                            .Where(s => s.TypeId == 1)
                            .SelectMany(s => s.CaseApplicants.Select(a => new ApplicantDetailDto
                            {
                                Applicant = a.ApplicantDetail,
                                ApplicantNo = a.ApplicantNo
                            })).ToList(),

                        SecondPartyDetails = x.Titles
                            .Where(s => s.TypeId == 2)
                            .SelectMany(s => s.CaseApplicants.Select(a => new ApplicantDetailDto
                            {
                                Applicant = a.ApplicantDetail,
                                ApplicantNo = a.ApplicantNo
                            })).ToList(),

                        AgainstCourtDetail = x.CaseAgainstEntities.Select(s => new AgainstCaseDetail
                        {
                            ImpugedOrder = s.ImpugedOrderDate.ToString("dd/MM/yyyy"),
                            State = lang == "Hi" ? GetCompleteWordAsync(s.State?.Name_En, lang).Result : s.State?.Name_En,
                            CourtBench = lang == "Hi" ? GetCompleteWordAsync(s.CourtBench?.CourtBench_En, lang).Result : s.CourtBench?.CourtBench_En,
                            CaseType = lang == "Hi" ? GetCompleteWordAsync(s.CaseType?.Name_En, lang).Result : s.CaseType?.Name_En,
                            CaseNo = s.CaseNo ?? "",
                            CaseYear = s.CaseYear.ToString(),
                            CisNoYear = string.IsNullOrWhiteSpace(s.CisNo) || s.CisNo == "0"
                                ? $"{s.CisYear}"
                                : $"{s.CisNo}/{s.CisYear}",
                            CnrNo = s.CnrNo ?? ""
                        }).FirstOrDefault()
                    });
                }

                //var result = casesQuery.Select(x => new GlobalFormPrintDto
                //{
                //    InstitutionDate = x.InstitutionDate.ToString("dd/MM/yyyy"),                    
                //    CaseNoYear = string.IsNullOrWhiteSpace(x.CaseNo) || x.CaseNo == "0"
                //                ? $"{x.CaseYear}"
                //                : $"{x.CaseNo}/{x.CaseYear}",                    
                //    CisNoYear = string.IsNullOrWhiteSpace(x.CisNumber) || x.CisNumber == "0"
                //                ? $"{x.CisYear}"
                //                : $"{x.CisNumber}/{x.CisYear}",                    
                //    FirstPartyDetails = x.Titles.Where(s => s.TypeId == 1)
                //    .SelectMany(s => s.CaseApplicants.Select(app => new ApplicantDetailDto
                //    {
                //        Applicant = app.ApplicantDetail,
                //        ApplicantNo = app.ApplicantNo
                //    })).ToList(),
                //    SecondPartyDetails = x.Titles.Where(s => s.TypeId == 2)
                //    .SelectMany(s => s.CaseApplicants.Select(app => new ApplicantDetailDto
                //    {
                //        Applicant = app.ApplicantDetail,
                //        ApplicantNo = app.ApplicantNo
                //    })).ToList(),
                //    State = x.State != null ? x.State.Name_En : "",
                //    Strength = x.StrengthId == 1 ? "S.B." : "D.B.",
                //    CourtType = x.CourtType != null ? x.CourtType.CourtType : "",
                //    CaseCategory = x.CaseCategory != null ? x.CaseCategory.Name_En : "",
                //    CaseType = x.CaseType != null ? x.CaseType.Name_En : "",
                //    CourtDistrict = x.CourtDistrict != null ? x.CourtDistrict.Name_En : "",
                //    CourtComplex = x.Complex != null ? x.Complex.Name_En : "",
                //    Court = x.CourtBench != null ? x.CourtBench.CourtBench_En : "",
                //    PetitionerAppearance = x.FTitle != null ? x.FTitle.Name_En : "",
                //    Petitioner = x.FirstTitle,
                //    RespondantAppearance = x.STitle != null ? x.STitle.Name_En : "",
                //    Respondent = x.SecondTitle,
                //    CaseStage = x.CaseStage != null ? x.CaseStage.CaseStage : "",
                //    NextDate = GetLatestNextDate(x),
                //    CnrNo = x.CnrNumber,
                //    DisposalDate = x.DisposalDate?.ToString("dd/MM/yyyy") ?? "",
                //    AgainstCourtDetail = x.CaseAgainstEntities.Select(s => new AgainstCaseDetail
                //    {
                //        ImpugedOrder = s.ImpugedOrderDate.ToString("dd/MM/yyyy"),
                //        State = s.State != null ? s.State.Name_En : "",
                //        CourtType = s.CourtType != null ? s.CourtType.CourtType : "",
                //        CourtDistrict = s.CourtDistrict != null ? s.CourtDistrict.Name_En : "",
                //        CourtComplex = s.Complex != null ? s.Complex.Name_En : "",
                //        CourtBench = s.CourtBench != null ? s.CourtBench.CourtBench_En : "",
                //        CaseNo = s.CaseNo != null ? s.CaseNo.ToString() : "",
                //        CaseYear = s.CaseYear.ToString(),
                //        CaseType = s.CaseType != null ? s.CaseType.Name_En : "",
                //        CisNo = s.CisNo != null ? s.CisNo.ToString() : "",
                //        CisYear = s.CisYear.ToString(),
                //        CisNoYear = string.IsNullOrWhiteSpace(s.CisNo) || s.CisNo == "0"
                //                ? $"{s.CisYear}"
                //                : $"{s.CisNo}/{s.CisYear}",
                //        CnrNo = s.CnrNo != null ? s.CnrNo.ToString() : "",
                //        Cadre = s.Cadre != null ? s.Cadre.Name_En : "",
                //        OfficerName = s.OfficerName ?? "",
                //        CaseCategory = s.CaseCategory != null ? s.CaseCategory.Name_En : "",
                //        DistrictCourt = s.CourtDistrict != null ? s.CourtDistrict.Name_En : ""
                //    }).FirstOrDefault(),
                //}).ToList();
                return await Result<List<GlobalFormPrintDto>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at Get Query Form:" + ex);
                return null;
            }
        }

        private async Task<string> T(string value, string lang)
        {
            return string.IsNullOrWhiteSpace(value)
                ? value
                : await GetCompleteWordAsync(value, lang);
        }

        private async Task<string> GetCompleteWordAsync(string word, string langKey = "hi") 
        {
            if (string.IsNullOrWhiteSpace(word))
                return word;

            var words = word.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var upperWords = words.Select(w => w.ToUpper()).ToList();

            var wordDict = await _langRepo.Entities
                .Where(w => upperWords.Contains(w.KeyWord.ToUpper()))
                .Select(w => new
                {
                    w.KeyWord,
                    Value = w.MultiLangs
                        .Where(m => m.Key == langKey)
                        .Select(m => m.Value)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var translatedWords = words.Select(w =>
                wordDict.FirstOrDefault(d =>
                    d.KeyWord.Equals(w, StringComparison.OrdinalIgnoreCase))
                ?.Value ?? w
            );

            return string.Join(" ", translatedWords);
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
