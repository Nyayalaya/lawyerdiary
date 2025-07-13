using CourtApp.Application.Features.CourtForm;
using CourtApp.Application.Features.FormPrint;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Litigation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CourtApp.Web.Areas.Litigation.Controllers
{
    [Area("Litigation")]
    public class CaseInfoPrintingController : BaseController<CaseInfoPrintingController>
    {

        public async Task<IActionResult> Index()
        {
            FmpViewModel fmpViewModel = new FmpViewModel();
            var formsDataResponse = await _mediator.Send(new CourtFormSearchQuery() { StateId = 1 });
            if (formsDataResponse.Succeeded)
            {
                var formsDt = formsDataResponse.Data.Select(s => new
                {
                    Id = s.Id,
                    FormName = s.FormName.ToUpper()
                });
                fmpViewModel.FormTypes = new SelectList(formsDt, "Id", "FormName");
            }
            fmpViewModel.Cases = await UserCaseTitle(Guid.Empty);
            return View(fmpViewModel);
        }
        public async Task<IActionResult> Index1()
        {
            FmpViewModel fmpViewModel = new FmpViewModel();
            fmpViewModel.FormTypes = FormPrintingTypes();
            fmpViewModel.Cases = await UserCaseTitle(Guid.Empty);
            fmpViewModel.Titles = await UserCaseTitle(Guid.Empty);
            return View(fmpViewModel);
        }
        public async Task<IActionResult> LoadFormPrinting(Guid type, List<Guid> Cases, List<string> AppNo)
        {
            try
            {
                // 1. Fetch Form Template
                var formTemplateResult = await _mediator.Send(new CourtFormSearchQuery { StateId = 1, Id = type });

                var formTemplateEntity = formTemplateResult.Data?.FirstOrDefault();
                var formTemplate = formTemplateEntity?.FormTemplate;
                var formName = formTemplateEntity?.FormName;

                if (!formTemplateResult.Succeeded || string.IsNullOrWhiteSpace(formTemplate))
                    return BadRequest("Form template not found or is empty.");

                // 2. Fetch Case Data
                var caseDataResult = await _mediator.Send(new GetFormPrintDataQuery { CaseIds = Cases });

                if (!caseDataResult.Succeeded || caseDataResult.Data == null)
                    return BadRequest("Unable to retrieve case details.");

                //if applicant no is selected
                var casesData = caseDataResult.Data;

                var caseInfoDetails = _mapper.Map<List<FormPrintData>>(casesData);

                if (caseInfoDetails == null || !caseInfoDetails.Any())
                    return BadRequest("No case data available.");

                // 3. Generate HTML
                var formHtmlList = new List<string>();
                var formNames = new List<string>();
                formNames.Add("Notice");
                formNames.Add("Envalop");
                bool isAddress = !string.IsNullOrWhiteSpace(formName) &&
                 formNames.Any(x => formName.StartsWith(x, StringComparison.OrdinalIgnoreCase));
                bool isNotice = formName.Contains("Notice");

                var vwName = "_GlobalFormPrintPartial";
                foreach (var caseInfo in caseInfoDetails)
                {
                    var againstDetail = caseInfo.AgainstCourtDetail;

                    if (isAddress)
                    {
                        vwName = isNotice != true ? "_Envalop" : vwName;
                        foreach (var applicant in caseInfo.Applicants?.Where(a => a != null && AppNo.Contains(a.ApplicantNo)) ?? Enumerable.Empty<ApplicantDetailViewModel>())
                        {
                            try
                            {
                                var html = ReplaceFormPlaceholders(formTemplate, caseInfo, applicant, againstDetail);
                                formHtmlList.Add(HttpUtility.HtmlDecode(html));
                            }
                            catch (Exception innerEx)
                            {
                                Console.WriteLine($"Error generating form for Applicant {applicant.ApplicantNo}: {innerEx.Message}");
                                // Optionally log
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var html = ReplaceFormPlaceholders(formTemplate, caseInfo, null, againstDetail); ;
                            formHtmlList.Add(HttpUtility.HtmlDecode(html));
                        }
                        catch (Exception innerEx)
                        {
                            Console.WriteLine("Error generating non-applicant form: " + innerEx.Message);
                            // Optionally log
                        }
                    }
                }

                return PartialView(vwName, formHtmlList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled Exception: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, "An internal error occurred while generating the form.");
            }
        }


        private string ReplaceFormPlaceholders(string template, FormPrintData caseInfo, ApplicantDetailViewModel applicant, AgainstCaseDecisionViewModel agDetail)
        {
            var replacements = new Dictionary<string, string>
            {
                ["#InstitutionDate#"] = caseInfo.InstitutionDate ?? "",
                ["#StateName#"] = caseInfo.State.ToUpper() ?? "",
                ["#CourtType#"] = caseInfo.CourtType.ToUpper() ?? "",
                ["#CourtDistrict#"] = caseInfo.CourtDistrict ?? "",
                ["#CourtComplex#"] = caseInfo.CourtComplex ?? "",
                ["#Bench#"] = caseInfo.Court.ToUpper() ?? "",
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
                ["#ApplicantNo#"] = applicant != null ? applicant.ApplicantNo?.ToString() : "",
                ["#ApplicantDetail#"] = applicant != null ? applicant.Applicant.ToUpper() : "",
                ["#ImpugedOrder#"] = agDetail?.ImpugedOrder ?? "",
                ["#AgState#"] = agDetail?.State ?? "",
                ["#AgCourtType#"] = agDetail?.CourtType ?? "",
                ["#AgCourtDistrict#"] = agDetail?.CourtDistrict ?? "",
                ["#AgCourtComplex#"] = agDetail?.CourtComplex ?? "",
                ["#AgCourtBench#"] = agDetail?.CourtBench ?? "",
                ["#AgCaseNoYear#"] = $"{agDetail?.CaseNo ?? ""}/{agDetail?.CaseYear ?? ""}",
                ["#AgCaseType#"] = agDetail?.CaseType ?? "",
                ["#AgCnrNo#"] = agDetail?.CnrNo ?? "",
                ["#Cadre#"] = agDetail?.Cadre ?? "",
                ["#OfficerName#"] = agDetail?.OfficerName ?? "",
                ["#AgCaseCategory#"] = agDetail?.CaseCategory ?? "",
                ["#DecisionDate#"] = caseInfo.DisposalDate ?? caseInfo.NextDate ?? "",
                ["#LawyerName#"] = CurrentUser.FirstName + " " + CurrentUser.LastName,
                ["#LawyerMobile#"] = CurrentUser.Mobile,
                ["#LawyerAddress#"] = CurrentUser.Address
            };

            foreach (var (key, value) in replacements)
            {
                template = template.Replace(key, value);
            }

            return template;
        }

        public async Task<IActionResult> LoadFormPrinting1(string type, List<Guid> Cases, string AppNo)
        {
            //List<Guid> CaseIds = new List<Guid>();
            if (Cases != null && Cases.Count > 0)
            {
                //CaseIds = Cases.Split(',').Select(Guid.Parse).ToList();
                if (type == "FINS") //Inspection form 
                {
                    var response = await _mediator.Send(new GetInspectionQuery() { CaseIds = Cases });
                    if (response.Succeeded)
                    {
                        var viewmodel = _mapper.Map<List<InspectionViewModel>>(response.Data);
                        FmpInspectionFormViewModel fmpViewModel = new FmpInspectionFormViewModel();
                        fmpViewModel.Cases = viewmodel;
                        return PartialView("_InspectionForm", fmpViewModel);
                    }
                }
                else if (type == "FTLW")
                {
                    return PartialView("_TalwanaForm", null);
                }

                else if (type == "FPRS") //permission slip need to modify logic for template
                {
                    var response = await _mediator.Send(new GetPermissionSlipQuery() { CaseIds = Cases });
                    if (response.Succeeded)
                    {
                        var viewmodel = _mapper.Map<List<PermissionSlipDataModel>>(response.Data);
                        FmpPermissionSlipFormViewModel fmpViewModel = new FmpPermissionSlipFormViewModel();
                        fmpViewModel.PerSlipInfo = viewmodel;
                        return PartialView("_PermissionSlip", fmpViewModel);
                    }
                }

                else if (type == "COPA") //Copying application
                {
                    var response = await _mediator.Send(new GetCopyingAppQuery() { CaseIds = Cases });
                    if (response.Succeeded)
                    {
                        var viewmodel = _mapper.Map<List<CopyingAppViewModel>>(response.Data);
                        FmpCopyingAppViewModel fmpViewModel = new FmpCopyingAppViewModel();
                        fmpViewModel.Cases = viewmodel;
                        return PartialView("_CopyingApplication", fmpViewModel);
                    }
                }

                else if (type == "FNOA") //Notice of admission or civil admission
                {
                    var response = await _mediator.Send(new GetNoticeOfAdmissionQuery() { CaseIds = Cases });
                    if (response.Succeeded)
                    {
                        var viewmodel = _mapper.Map<List<NoticeAdmissionViewModel>>(response.Data);
                        FmpNoticeAdmissionViewModel fmpViewModel = new FmpNoticeAdmissionViewModel();
                        fmpViewModel.Cases = viewmodel;
                        var WritCases = viewmodel.Where(x => x.CaseCategory.ToLower() == "writ");
                        var CivilCases = viewmodel.Where(x => x.CaseCategory.ToLower() == "civil");
                        if (WritCases.Count() > 0)
                        {
                            fmpViewModel.Cases = WritCases.ToList();
                            return PartialView("_AdmissionWrit", fmpViewModel);
                        }
                        else
                        {
                            fmpViewModel.Cases = CivilCases.ToList();
                            return PartialView("_AdmissionCivil", fmpViewModel);
                        }
                    }
                }

                else if (type == "FNSA") //Notice of stay application
                {
                    var response = await _mediator.Send(new GetNoticeOfStayAppQuery() { CaseIds = Cases });
                    if (response.Succeeded)
                    {
                        var viewmodel = _mapper.Map<List<NoticeOfStayApplication>>(response.Data);
                        FmpNoticeOfStayApplicationViewModel fmpViewModel = new FmpNoticeOfStayApplicationViewModel();
                        fmpViewModel.NoticeStayApps = viewmodel;
                        return PartialView("_NoticeOfStayApplication", fmpViewModel);
                    }
                }

                else if (type == "FNSC") //Notice of show cause
                {
                    var response = await _mediator.Send(new GetShowCauseNoticeQuery() { CaseIds = Cases, ApplicantNo = AppNo });
                    if (response.Succeeded)
                    {
                        var viewmodel = _mapper.Map<List<ShowCauseViewModel>>(response.Data);
                        FmpShowCauseViewModel fmpViewModel = new FmpShowCauseViewModel();
                        var WritCases = viewmodel.Where(x => x.CaseType.ToLower().Contains("writ"));
                        var CivilCases = viewmodel.Where(x => x.CaseType.ToLower().Contains("civil"));
                        if (WritCases.Count() > 0)
                        {
                            fmpViewModel.ShowCauses = WritCases.ToList();
                            return PartialView("_ShowCauseNoticeWrit", fmpViewModel);
                        }
                        else
                        {
                            fmpViewModel.ShowCauses = CivilCases.ToList();
                            return PartialView("_ShowCauseNoticeCivil", fmpViewModel);
                        }

                    }
                }


                else
                    return null;

            }
            return null;
        }
    }
}
