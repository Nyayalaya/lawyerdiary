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
                // Phase 1 - SQL query
                var rawCases = await (
                    from caseInfo in _CaseRepo.Entites
                        .AsNoTracking()
                        .Where(w => request.CaseIds.Contains(w.Id))
                    join title in _TitleRepo.Titles
                        .AsNoTracking()
                        .Where(t => t.TypeId == 2)
                        on caseInfo.Id equals title.CaseId into CompleteTitle
                    from ct in CompleteTitle.DefaultIfEmpty()
                    select new
                    {
                        CaseInfo = caseInfo,
                        Title = ct,
                        State = caseInfo.State != null ? caseInfo.State.Name_En : "",
                        Strength = caseInfo.StrengthId == 1 ? "S.B." : "D.B.",
                        CourtType = caseInfo.CourtType != null ? caseInfo.CourtType.CourtType : "",
                        Category = caseInfo.CaseCategory != null ? caseInfo.CaseCategory.Name_En : "",
                        Type = caseInfo.CaseType != null ? caseInfo.CaseType.Name_En : "",
                        CourtDistrict = caseInfo.CourtDistrict != null ? caseInfo.CourtDistrict.Name_En : "",
                        CourtComplex = caseInfo.Complex != null ? caseInfo.Complex.Name_En : "",
                        Court = caseInfo.CourtBench != null ? caseInfo.CourtBench.CourtBench_En : "",
                        PetiAppearence=caseInfo.FTitle!=null? caseInfo.FTitle.Name_En:"",
                        ResAppearence=caseInfo.STitle!=null? caseInfo.STitle.Name_En:"",
                        Stage=caseInfo.CaseStage!=null? caseInfo.CaseStage.CaseStage:"",
                    }
                ).ToListAsync();

                // Phase 2 - In-memory mapping
                var caseData = rawCases.Select(x => new GlobalFormPrintDto
                {
                    InstitutionDate = x.CaseInfo.InstitutionDate.ToString("dd/MM/yyyy"),
                    State = x.State,
                    CourtType = x.CourtType,
                    CourtDistrict = x.CourtDistrict,
                    CourtComplex = x.CourtComplex,
                    Court = x.Court,
                    Strength = x.Strength,
                    CaseNoYear = x.CaseInfo.CaseNo + "/" + x.CaseInfo.CaseYear,
                    CaseCategory = x.Category,
                    CaseType = x.Type,
                    CisNoYear = x.CaseInfo.CisNumber + "/" + x.CaseInfo.CisYear,
                    PetitionerAppearance = x.PetiAppearence,
                    Petitioner = x.CaseInfo.FirstTitle,
                    RespondantAppearance = x.ResAppearence,
                    Respondent = x.CaseInfo.SecondTitle,
                    CaseStage = x.Stage,
                    CnrNo = x.CaseInfo.CnrNumber,
                    DisposalDate = x.CaseInfo.DisposalDate?.ToString("dd/MM/yyyy") ?? "",

                    // Custom helpers in memory
                    NextDate = GetLatestNextDate(x.CaseInfo),
                    AgainstCourtDetail = GetAgainsCaseDetail(x.CaseInfo),
                    Applicants = (x.Title != null)
                        ? x.Title.CaseApplicants.Select(s => new ApplicantDetailDto
                        {
                            Applicant = s.ApplicantDetail,
                            ApplicantNo = s.ApplicantNo
                        }).ToList()
                        : new List<ApplicantDetailDto>()
                }).ToList();

                return await Result<List<GlobalFormPrintDto>>.SuccessAsync(caseData);



            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at Get Query Form:" + ex);
                return null;
            }

            //try
            //{
            //    var caseData = (
            //                from caseInfo in _CaseRepo.Entites
            //                    .AsNoTracking()
            //                    .Include(c => c.CaseCategory)
            //                    .Include(c => c.State)
            //                    .Include(c => c.CaseStage)
            //                    .Include(c => c.CourtType)
            //                    .Include(c => c.CourtBench)
            //                    .Include(c => c.FTitle)
            //                    .Include(c => c.STitle)
            //                    .Include(c => c.CaseProcEntities)
            //                    .Include(ac => ac.CaseAgainstEntities).ThenInclude(c => c.CourtBench)
            //                    .Include(ac => ac.CaseAgainstEntities).ThenInclude(c => c.CourtDistrict)
            //                    .Include(ac => ac.CaseAgainstEntities).ThenInclude(c => c.CaseCategory)
            //                    .Include(ac => ac.CaseAgainstEntities).ThenInclude(c => c.CaseType)
            //                    .Include(ac => ac.CaseAgainstEntities).ThenInclude(c => c.CourtType)
            //                    .Include(ac => ac.CaseAgainstEntities).ThenInclude(c => c.CourtBench)
            //                    .Include(ac => ac.CaseAgainstEntities).ThenInclude(c => c.Complex)
            //                    .Include(ac => ac.CaseAgainstEntities).ThenInclude(c => c.Cadre)
            //                    .Where(w => request.CaseIds.Contains(w.Id))
            //                join title in _TitleRepo.Titles.AsNoTracking().Where(t => t.TypeId == 2)
            //                    .Include(t => t.CaseApplicants)
            //                    on caseInfo.Id equals title.CaseId into CompleteTitle
            //                from ct in CompleteTitle.DefaultIfEmpty()
            //                let cd = caseInfo
            //                select new
            //                {
            //                    CaseInfo = caseInfo,
            //                    Title = ct,
            //                    AgainstCaseDetail = caseInfo.CaseAgainstEntities,
            //                    CaseApplicants = ct != null && ct.TypeId == 2 ? ct.CaseApplicants : null
            //                }
            //            )
            //            .AsEnumerable()
            //            .Select(data => new GlobalFormPrintDto
            //            {
            //                InstitutionDate = data.CaseInfo.InstitutionDate.ToString("dd/MM/yyyy"),
            //                State = data.CaseInfo.State.Name_En,
            //                CourtType = data.CaseInfo.CourtType?.CourtType ?? "",
            //                CourtDistrict = data.CaseInfo.CourtDistrict?.Name_En ?? "",
            //                CourtComplex = data.CaseInfo.Complex?.Name_En ?? "",
            //                Court = data.CaseInfo.CourtBench?.CourtBench_En ?? "",
            //                Strength = "",
            //                CaseNoYear = data.CaseInfo.CaseNo + "/" + data.CaseInfo.CaseYear,
            //                CaseCategory = data.CaseInfo.CaseCategory?.Name_En ?? "",
            //                CaseType = data.CaseInfo.CaseType?.Name_En ?? "",
            //                CisNoYear = data.CaseInfo.CisNumber + "/" + data.CaseInfo.CisYear,
            //                PetitionerAppearance = data.CaseInfo.FTitle?.Name_En ?? "",
            //                Petitioner = data.CaseInfo.FirstTitle,
            //                RespondantAppearance = data.CaseInfo.STitle?.Name_En ?? "",
            //                Respondent = data.CaseInfo.SecondTitle,
            //                NextDate = GetLatestNextDate(data.CaseInfo),
            //                CaseStage = data.CaseInfo.CaseStage?.CaseStage ?? "",
            //                CnrNo = data.CaseInfo.CnrNumber,
            //                DisposalDate = data.CaseInfo.DisposalDate?.ToString("dd/MM/yyyy") ?? "",
            //                AgainstCourtDetail = GetAgainsCaseDetail(data.CaseInfo),
            //                Applicants = data.CaseApplicants?.Select(s => new ApplicantDetailDto
            //                {
            //                    Applicant = s.ApplicantDetail,
            //                    ApplicantNo = s.ApplicantNo
            //                }).ToList(),


            //            }).ToList();

            //    return await Result<List<GlobalFormPrintDto>>.SuccessAsync(caseData);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Error at Get Query Form:" + ex);
            //    return null;
            //}
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
