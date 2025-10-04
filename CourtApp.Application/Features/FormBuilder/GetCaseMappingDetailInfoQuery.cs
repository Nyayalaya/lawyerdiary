using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.FormBuilder;
using CourtApp.Application.Interfaces.CacheRepositories;
using CourtApp.Application.Interfaces.CacheRepositories.FormBuilder;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Repositories.FormBuilder;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.FormBuilder
{
    public class GetCaseMappingDetailInfoQuery : IRequest<Result<CaseMappingDetailInfoDto>>
    {
        public Guid Id { get; set; }
    }
    public class GetCaseMappingDetailInfoQueryHandler : IRequestHandler<GetCaseMappingDetailInfoQuery, Result<CaseMappingDetailInfoDto>>
    {
        private readonly ICaseNatureCacheRepository _RepoNature;
        private readonly IUserCaseRepository _RepoCase;
        private readonly ICaseDraftingRepository _RepoDrafting;
        private readonly ITemplateInfoCacheRepository _RepoTemplate;
        private readonly IFormTempMappingRepository _RepoMapping;
        public GetCaseMappingDetailInfoQueryHandler(IUserCaseRepository _RepoCase,
            ICaseNatureCacheRepository _RepoNature,
            ICaseStageCacheRepository _RepoStage,
            ICaseDraftingRepository _RepoDrafting,
            ITemplateInfoCacheRepository _RepoTemplate,
            IFormTempMappingRepository _RepoMapping
            )
        {
            this._RepoCase = _RepoCase;
            this._RepoNature = _RepoNature;
            this._RepoNature = _RepoNature;
            this._RepoTemplate = _RepoTemplate;
            this._RepoDrafting = _RepoDrafting;
            this._RepoTemplate = _RepoTemplate;
            this._RepoMapping = _RepoMapping;
        }
        public async Task<Result<CaseMappingDetailInfoDto>> Handle(GetCaseMappingDetailInfoQuery request, CancellationToken cancellationToken)
        {
            var draftingDetail = await _RepoDrafting.GetByIdAsync(request.Id);
            if (draftingDetail == null) return await Result<CaseMappingDetailInfoDto>.FailAsync("No drafting found!");

            var templateDetail = await _RepoTemplate.GetByIdAsync(draftingDetail.TemplateId);
            if (templateDetail == null) return await Result<CaseMappingDetailInfoDto>.FailAsync("No template found!");

            CaseMappingDetailInfoDto infoDto = new CaseMappingDetailInfoDto();
            infoDto.TemplateBody = templateDetail.TemplateBody;
            infoDto.CaseId = draftingDetail.CaseId;
            var tagsData = draftingDetail.FieldDetails.Select(s => new MappingDetails { Tag = s.Tag, Value = s.Value }).ToList();
            infoDto.TagValues = tagsData;
            return await Result<CaseMappingDetailInfoDto>.SuccessAsync(infoDto);

            //var draftingDetail = _RepoDrafting
            //    .Entities
            //    .Include(c => c.Case).ThenInclude(s => s.State)
            //    .Include(c => c.Case).ThenInclude(s => s.CaseAgainstEntities)
            //    .Include(c => c.Case).ThenInclude(s => s.CourtBench)
            //    .Include(c => c.Case).ThenInclude(s => s.FTitle)
            //    .Include(c => c.Case).ThenInclude(s => s.STitle)
            //    .Include(c => c.Case).ThenInclude(s => s.CaseCategory)
            //    .Include(c => c.Case).ThenInclude(s => s.CaseStage)
            //    .Include(c => c.Case).ThenInclude(s => s.CaseType)
            //    .Include(c => c.Case).ThenInclude(s => s.Titles)
            //    .Where(w => w.Id == request.Id).FirstOrDefault();
            //if (draftingDetail != null)
            //{
            //    var caseId = draftingDetail.CaseId;
            //    var TemplateId = draftingDetail.TemplateId;
            //    //var draftId = draftingDetail.DraftingFormId;
            //    var FieldDetails = draftingDetail.FieldDetails;
            //    var TempInfo = await _RepoTemplate.GetByIdAsync(TemplateId);
            //    var agd = draftingDetail.Case.CaseAgainstEntities.FirstOrDefault();

            //    var FieldMapping = _RepoMapping
            //        .Entities
            //        .Where(w => /*w.FormId == draftId &&*/ w.TemplateId == TemplateId)
            //        .FirstOrDefault();
            //    //Getting all the first title of the court. Here we shall be generate a string and
            //    //concate all the applicants in one string with new line corrector.
            //    var fsTitles = draftingDetail.Case.Titles
            //        .Where(t => t.TypeId == 1)
            //        .SelectMany(a => a.CaseApplicants);
            //    string ftils = "";
            //    foreach (var item in fsTitles)
            //        ftils += item.ApplicantNo + ".    " + item.ApplicantDetail + " <br><br><br>";

            //    var ssTitles = draftingDetail.Case.Titles.Where(t => t.TypeId == 2)
            //        .SelectMany(a => a.CaseApplicants);
            //    string stils = "";
            //    foreach (var item in ssTitles)
            //        stils += item.ApplicantNo + ".    " + item.ApplicantDetail + " <br><br><br>";
            //    var tagMappingDetails = (from fm in FieldMapping.FieldsMapping
            //                             join fd in FieldDetails on fm.Key equals fd.Key
            //                             select new MappingDetails
            //                             {
            //                                 Key = fm.Tag,
            //                                 Value = fd.Value
            //                             }).ToList();
            //    var CaseExistingKeys = TempInfo.Tags.Where(w => w.Tag.Contains("DB"));
            //    var AmountAwarded = tagMappingDetails.Where(w => w.Key.Trim().Equals("#AmountAwarded#")).Select(s => s.Value).FirstOrDefault();
            //    var AmountClaimd = tagMappingDetails.Where(w => w.Key.Trim().Equals("#AmountClaimed#")).Select(s => s.Value).FirstOrDefault();
            //    var totalAmount = Convert.ToDouble(AmountClaimd) - Convert.ToDouble(AmountAwarded);
            //    foreach (var it in CaseExistingKeys)
            //    {
            //        var dbm = new MappingDetails();
            //        dbm.Key = it.Tag.Trim();
            //        if (it.Tag.Trim() == "#DBStrength#") dbm.Value = draftingDetail.Case.StrengthId == 1 ? "S.B." : "D.B. <br><br>";
            //        else if (it.Tag.Trim() == "#DBStateName#") dbm.Value = draftingDetail.Case.State.Name_En.ToUpper() + " <br>";
            //        else if (it.Tag.Trim() == "#DBBench#") dbm.Value = draftingDetail.Case.CourtBench.CourtBench_En.ToUpper();
            //        else if (it.Tag.Trim() == "#DBCaseType#") dbm.Value = draftingDetail.Case.CaseType.Name_En;
            //        else if (it.Tag.Trim() == "#DBCaseNoYear#") dbm.Value = draftingDetail.Case.CaseNo + "/" + draftingDetail.Case.CaseYear;
            //        else if (it.Tag.Trim() == "#DBFTCase#") dbm.Value = ftils;//draftingDetail.Case.FirstTitle;
            //        else if (it.Tag.Trim() == "#DBSTCase#") dbm.Value = stils;//draftingDetail.Case.SecondTitle;

            //        else if (it.Tag.Trim() == "#DBFirstTitleFull#") dbm.Value = draftingDetail.Case.FirstTitle;
            //        else if (it.Tag.Trim() == "#DBSecondTitleFull#") dbm.Value = draftingDetail.Case.SecondTitle;

            //        else if (it.Tag.Trim() == "#DBFirstTitle#") dbm.Value = draftingDetail.Case.FTitle.Name_En;
            //        else if (it.Tag.Trim() == "#DBSecondTitle#") dbm.Value = draftingDetail.Case.STitle.Name_En;
            //        else if (it.Tag.Trim() == "#DBImpungedOrder#") dbm.Value = agd.ImpugedOrderDate.ToString("dd/MM/yyyy");
            //        else if (it.Tag.Trim() == "#DBCadre#") dbm.Value = agd != null && agd.Cadre != null ? agd.Cadre.Name_En : "";
            //        else if (it.Tag.Trim() == "#DBOfficerName#") dbm.Value = agd != null ? agd.OfficerName : "";
            //        else if (it.Tag.Trim() == "#DBAgCaseNoYear#") dbm.Value = agd != null ? agd.CaseNo + "/" + agd.CaseYear : "";
            //        else if (it.Tag.Trim() == "#DBAgCISCASENoYear#") dbm.Value = agd != null ? agd.CisNo + "/" + agd.CisYear : "";
            //        else if (it.Tag.Trim() == "#DBAgCNRNO#") dbm.Value = agd != null ? agd.CnrNo : "";
            //        else if (it.Tag.Trim() == "#DBAgainstCourt#") dbm.Value = agd != null && agd.CourtBench != null ? agd.CourtBench.CourtBench_En : "";
            //        else if (it.Tag.Trim().Contains("#DBAmountRecieved#")) dbm.Value = totalAmount.ToString();
            //        else dbm.Value = "";
            //        tagMappingDetails.Add(dbm);
            //    }
            //    CaseMappingDetailInfoDto cmd = new CaseMappingDetailInfoDto();
            //    cmd.CaseTitle = draftingDetail.Case.FirstTitle + " Vs " + draftingDetail.Case.FirstTitle;
            //    cmd.CaseNoYear = draftingDetail.Case.CaseNo + "/" + draftingDetail.Case.CaseYear;
            //    cmd.CaseType = draftingDetail.Case.CaseType.Name_En;
            //    cmd.TagValues = tagMappingDetails.ToList();
            //    cmd.TemplateName = TempInfo.TemplateName;
            //    cmd.TemplatePath = TempInfo.TemplatePath;
            //    cmd.TemplateBody = TempInfo.TemplateBody;
            //    return Result<CaseMappingDetailInfoDto>.Success(cmd);
            //}
            //return null;
        }
    }
}
