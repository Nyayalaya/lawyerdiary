using CourtApp.Application.Features.FormBuilder;
using CourtApp.Application.Features.FormPrint;
using CourtApp.Infrastructure.DbContexts;
using CourtApp.Infrastructure.Identity.Models;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Admin.Models;
using CourtApp.Web.Areas.Litigation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
namespace CourtApp.Web.Areas.Litigation.Controllers
{
    [Area("Litigation")]
    public class DraftingController : BaseController<DraftingController>
    {
        private readonly IdentityContext _identityDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public DraftingController(IdentityContext identityDbContext,
            UserManager<ApplicationUser> userManager)
        {
            _identityDbContext = identityDbContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> LoadAll()
        {
            var response = await _mediator.Send(new GetCaseDarftingQuery()
            {
                PageSize = 10000,
                PageNumber = 1
            });
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<FormCaseMappingViewModel>>(response.Data);
                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }
        public async Task<IActionResult> GetTemplateFields(Guid TemplateId, FormBuilderViewModel ViewModel)
        {
            var response = await _mediator.Send(new GetFormBuilderCachedByIdQuery()
            {
                Id = TemplateId,
                AccessFrom = "DRFT"
            });
            if (response.Succeeded)
            {
                var fieldPropeties = response.Data.FieldDetails;
                var Dt = _mapper.Map<List<FormProperties>>(fieldPropeties);
                ViewModel.FieldDetails = Dt;
                return PartialView("_FormFields", ViewModel);
            }
            return null;
        }
        public async Task<JsonResult> OnGetCreateOrEdit(Guid id)
        {
            if (id == Guid.Empty)
            {
                var ViewModel = new FormBuilderViewModel();
                ViewModel.DraftingForms = await GetDraftings();
                ViewModel.Templates = await GetTemplates();
                ViewModel.Cases = await UserCaseTitle(Guid.Empty);
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ViewModel) });
            }
            else
            {
                return null;
            }
        }
        public async Task<IActionResult> Petition(Guid id, FormBuilderViewModel ViewModel)
        {
            if (id == Guid.Empty)
            {
                ViewModel.Templates = await GetTemplates();
                ViewModel.DraftingForms = await GetDraftings();
                ViewModel.Cases = await UserCaseTitle(Guid.Empty);
                return View("_CreateOrEdit", ViewModel);
            }
            else
            {
                var result = await _mediator.Send(new GetCaseDarftingCachedByIdQuery() { Id = id });
                if (result.Succeeded)
                {
                    ViewModel = _mapper.Map<FormBuilderViewModel>(result.Data);
                    ViewModel.Templates = await GetTemplates();
                    ViewModel.DraftingForms = await GetDraftings();
                    ViewModel.Cases = await UserCaseTitle(Guid.Empty);
                    ViewModel.FieldDetails = _mapper.Map<List<FormProperties>>(ViewModel.FieldDetails);
                    return View("_CreateOrEdit", ViewModel);
                }
            }
            return null;
        }
        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEdit(Guid id, FormBuilderViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == Guid.Empty )
                    {
                        var Command = _mapper.Map<CreateCaseDraftingDetailCommand>(ViewModel);
                        var result = await _mediator.Send(Command);
                        if (result.Succeeded)
                            _notify.Success($"Case Drafting saved successfull!");
                        else
                        {
                            ViewModel.StatusMessage = result.Message;
                            ModelState.AddModelError(string.Empty, result.Message);
                            //TempData["RecordExists"] = true;
                        }
                    }
                    else
                    {
                        var Command = _mapper.Map<UpdateCaseDraftingDetailCommand>(ViewModel);
                        var result = await _mediator.Send(Command);
                        if (result.Succeeded)
                            _notify.Success($"Case drafting information is updated successfully!");
                    }
                    ViewModel.Cases = await UserCaseTitle(Guid.Empty);
                    ViewModel.Templates = await GetDraftings();
                    return View("_CreateOrEdit", ViewModel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ViewModel);
                return new JsonResult(new { isValid = false, html });
            }
        }

        public async Task<IActionResult> GetReport(Guid id,string langCode)
        {
            if (id != Guid.Empty)
            {
                var response = await _mediator.Send(new GetCaseMappingDetailInfoQuery() { Id = id });
                if (response.Succeeded)
                {
                    var dt = response.Data;
                    var Cases = new List<Guid> { dt.CaseId };
                    var caseDataResult = await _mediator.Send(new GetFormPrintDataQuery { CaseIds = Cases,Lang= langCode });
                    if (!caseDataResult.Succeeded || caseDataResult.Data == null)
                        return BadRequest("Unable to retrieve case details.");

                    var casesData = caseDataResult.Data;
                    var caseInfoDetails = _mapper.Map<List<FormPrintData>>(casesData);
                    if (caseInfoDetails == null || !caseInfoDetails.Any())
                        return BadRequest("No case data available.");

                    string FinalContent = string.Empty;
                    var Content = dt.TemplateBody;
                    foreach (var tg in dt.TagValues)
                    {
                        string replacement = tg.Value?.Trim() ?? "";
                        if (DateTime.TryParseExact(replacement, "yyyy-MM-dd",
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None,
                                                   out DateTime dtValue))
                        {
                            replacement = dtValue.ToString("dd/MM/yyyy");
                        }

                        FinalContent = Content.Replace(tg.Tag.Trim(), replacement);
                        Content = FinalContent;
                    }

                    var html = ReplaceFormPlaceholders(Content, caseInfoDetails.FirstOrDefault());
                    string fileName = caseInfoDetails.Select(s => s.CaseNoYear).FirstOrDefault();
                    byte[] wordFile = ConvertHtmlToWord(html);
                    return File(wordFile, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName + ".docx");

                }

            }
            return null;
        }

        private string ReplaceFormPlaceholders(string template, FormPrintData caseInfo)
        {
            var formatted1stApplicants = caseInfo.FirstPartyDetails != null && caseInfo.FirstPartyDetails.Any()
            ? string.Join("<br/>",
                caseInfo.FirstPartyDetails.Select(s => $"{s.ApplicantNo}. {s.Applicant}")
            ) : string.Empty;

            var formatted2stApplicants = caseInfo.SecondPartyDetails != null && caseInfo.SecondPartyDetails.Any()
            ? string.Join("<br/>",
                caseInfo.SecondPartyDetails.Select(s => $"{s.ApplicantNo}. {s.Applicant}")
            ) : string.Empty;
            var agDetail = caseInfo?.AgainstCourtDetail;
            var applicant = caseInfo?.SecondPartyDetails.Select(s => s.ApplicantNo).FirstOrDefault();

            var associates = GetAssociateAsync();
            foreach (var associate in associates.Result)
            {
                string associateName = associate.FirstName + " " + associate.LastName;
                string mobileNo = associate.Mobile;
            }

            var replacements = new Dictionary<string, string>
            {
                ["#InstitutionDate#"] = caseInfo.InstitutionDate ?? "",
                ["#StateName#"] = caseInfo.State.ToUpper() ?? "",
                ["#CourtType#"] = caseInfo.CourtType.ToUpper() ?? "",
                ["#CourtDistrict#"] = caseInfo.CourtDistrict ?? "",
                ["#CourtComplex#"] = caseInfo.CourtComplex ?? "",
                ["#Court#"] = caseInfo.Court.ToUpper() ?? "",
                ["#Strength#"] = caseInfo.Strength.ToUpper() ?? "",
                ["#CaseNoYear#"] = caseInfo.CaseNoYear ?? "",
                ["#CaseCategory#"] = caseInfo.CaseCategory.ToUpper() ?? "",
                ["#CaseType#"] = caseInfo.CaseType.ToUpper() ?? "",
                ["#CisNoYear#"] = caseInfo.CisNoYear ?? "",
                ["#PetitionerAppearance#"] = caseInfo.PetitionerAppearance.ToUpper() ?? "",
                ["#Petitioner#"] = caseInfo.Petitioner.ToUpper() ?? "",
                ["#RespondantAppearance#"] = caseInfo.RespondantAppearance.ToUpper() ?? "",
                ["#Respondant#"] = caseInfo.Respondent.ToUpper() ?? "",
                ["#NextDate#"] = caseInfo.NextDate ?? "",
                ["#CaseStage#"] = caseInfo.CaseStage.ToUpper() ?? "",
                ["#DisposalDate#"] = caseInfo.DisposalDate ?? "",
                ["#CnrNo#"] = caseInfo.CnrNo ?? "",
                ["#CurrentDate#"] = DateTime.Now.ToString("dd/MM/yyyy"),
                ["#ApplicantNo#"] = applicant != null ? applicant.ToString() : "",
                ["#ApplicantDetail#"] = formatted2stApplicants,
                ["#ImpugedOrder#"] = agDetail?.ImpugedOrder ?? "",
                ["#AgState#"] = agDetail?.State ?? "",
                ["#AgCourtType#"] = agDetail?.CourtType ?? "",
                ["#AgCourtDistrict#"] = agDetail?.CourtDistrict ?? "",
                ["#AgCourtComplex#"] = agDetail?.CourtComplex ?? "",
                ["#AgCourtBench#"] = agDetail?.CourtBench ?? "",
                ["#AgCaseNoYear#"] = $"{agDetail?.CaseNo ?? ""}/{agDetail?.CaseYear ?? ""}",
                ["#AgCaseType#"] = agDetail?.CaseType ?? "",
                ["#AgCnrNo#"] = agDetail?.CnrNo ?? "",
                ["#AgCisNo#"] = agDetail?.CisNo ?? "",
                ["#AgCisNoYear#"] = agDetail?.CisNoYear ?? "",
                ["#Cadre#"] = agDetail?.Cadre ?? "",
                ["#OfficerName#"] = agDetail?.OfficerName ?? "",
                ["#AgCaseCategory#"] = agDetail?.CaseCategory ?? "",
                ["#DecisionDate#"] = caseInfo.DisposalDate ?? caseInfo.NextDate ?? "",
                ["#LawyerName#"] = CurrentUser.FirstName + " " + CurrentUser.LastName,
                ["#LawyerMobile#"] = CurrentUser.Mobile,
                ["#LawyerAddress#"] = CurrentUser.Address,
                ["#LawyerEmail#"] = CurrentUser.Email,
                ["#LawyerEnrollment#"] = CurrentUser.EnrollmentNo,
                ["#FirstPartyDetail#"] = formatted1stApplicants
            };

            foreach (var (key, value) in replacements)
            {
                template = template.Replace(key, value);
            }

            return template;
        }


        private async Task<List<UserViewModel>> GetAssociateAsync()
        {

            var otherLoggedUsers = new HashSet<string> { "ASSOCIATE", "CLERK" };
            var operatorIds = await _identityDbContext.LawyerUsers
                .Where(o => o.LawyerId == CurrentUser.Id)
                .Select(o => new { o.Id, o.DateOfJoining })
                .ToListAsync();
            var operatorIdSet = new HashSet<string>(operatorIds.Select(o => o.Id.ToString()));
            var model = await _userManager.Users
                .Where(user => user.Id != CurrentUser.Id
                    && otherLoggedUsers.Contains(user.UserType)
                    && operatorIdSet.Contains(user.Id))
                .Select(user => new UserViewModel
                {
                    Role = user.UserType,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Mobile = user.Mobile,
                    EmailConfirmed = user.EmailConfirmed,
                    ProfileImgPath = "",
                    ProfilePicture = user.ProfilePicture,
                    Id = user.Id,
                    IsActive = user.IsActive
                })
                .ToListAsync();
            return model;
        }
    }
}
