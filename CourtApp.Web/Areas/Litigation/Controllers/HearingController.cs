using CourtApp.Application.Features.CaseDetails;
using CourtApp.Application.Features.CaseProceeding;
using CourtApp.Application.Features.CaseWork;
using CourtApp.Application.Features.UserCase;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Litigation.Models;
using CourtApp.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Litigation.Controllers
{
    [Area("Litigation")]
    public class HearingController : BaseController<HearingController>
    {

        #region Case Hearing 
        public IActionResult Index(string SelectedDate)
        {
            var model = new BringTodayCaseViewModel();
            //var SelectedDate = TempData["SelectedDate"] != null ? TempData["SelectedDate"].ToString() : "";
            model.HearingDate = SelectedDate != null ? Convert.ToDateTime(SelectedDate) : DateTime.Now;
            return View(model);
        }
        public async Task<IActionResult> LoadAll(string seleDate)
        {
            if (seleDate == null)
                seleDate = DateTime.Now.Date.ToString();
            var response = await _mediator.Send(new GetCaseDetailsQuery()
            {
                CallingFrm = "HTD",
                HearingDate = Convert.ToDateTime(seleDate),
                LinkedIds = User.GetUserLinkedIds(),
                PageSize = 10000,
                PageNumber = 1
            });
            TempData["SelectedDate"] = seleDate;
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<HearingViewModel>>(response.Data);
                return PartialView("_ViewAll", TodayCaseList(viewModel, seleDate));
            }
            return null;
        }
        public async Task<JsonResult> LoadCaseData(string SDate)
        {
            var response = await _mediator.Send(new GetCaseDetailsQuery()
            {
                CallingFrm = "HTD",
                HearingDate = Convert.ToDateTime(SDate),
                LinkedIds = User.GetUserLinkedIds(),
                PageSize = 10000,
                PageNumber = 1
            });
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<HearingViewModel>>(response.Data);
                return Json(TodayCaseList(viewModel, SDate));
            }
            return null;
        }

        #endregion

        #region Bring Today Hearing Case
        public async Task<JsonResult> GetCaseHearing()
        {
            var response = await _mediator.Send(new GetCaseDetailsQuery()
            {
                CallingFrm = "BTD",
                LinkedIds = User.GetUserLinkedIds(),
                PageSize = 10000,
                PageNumber = 1
            });
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<GetCaseViewModel>>(response.Data);
                string SelectedDate = string.Empty;
                if (TempData["SelectedDate"] != null)
                {
                    SelectedDate = TempData["SelectedDate"].ToString();
                    TempData.Keep();
                }
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_BringToday", TodayCaseList(viewModel, SelectedDate)) });
            }
            return null;
        }
        private BringTodayCaseViewModel TodayCaseList(dynamic viewModel, string selectedDate)
        {
            var model = new BringTodayCaseViewModel();
            List<HearingViewModel> cdt = new List<HearingViewModel>();
            foreach (var item in viewModel)
            {
                var hearing = new HearingViewModel();
                hearing.Id = item.Id;
                hearing.CaseTitle = item.CaseTitle;
                hearing.CaseNumber = item.CaseNumber;
                hearing.CaseYear = item.CaseYear;
                hearing.CaseStage = item.CaseStage;
                hearing.CourtName = item.CourtName;
                hearing.CaseTypeName = item.CaseTypeName;
                hearing.IsProceedingDone = item.IsProceedingDone;
                hearing.Reference = item.Reference;
                hearing.IsCaseAssigned = item.IsCaseAssigned;
                hearing.HasChild = item.HasChild;
                cdt.Add(hearing);
            }
            model.CaseList = cdt;
            model.HearingDate = selectedDate != null ? Convert.ToDateTime(selectedDate) : DateTime.Now;
            return model;
        }

        #endregion

        #region Add the case in today's hearing register select and save.

        public async Task<IActionResult> UpdateCaseDate(BringTodayCaseViewModel model)
        {
            if (model.CaseList != null)
            {
                var CaseIds = model.CaseList.Where(s => s.Selected == true).Select(s => s.Id).ToList();
                var result = await _mediator.Send(new UpdateCaseNextDateCommand
                {
                    CaseIds = CaseIds,
                    NextHearingDate = model.HearingDate
                });
                if (result.Succeeded) _notify.Information($"Case Next hearing date with ID {result.Data} Updated.");
                else _notify.Error(result.Message);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Assign Work to the case
        public async Task<JsonResult> AssignWorkCase(Guid CaseId)
        {
            var ViewModel = new CaseWorkingViewModel();
            ViewModel.WorkTypes = await DdlWorks();
            //ViewModel.CaseId = CaseId;
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignWork", ViewModel) });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAssignWork(CaseWorkingViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                var cmd = _mapper.Map<CreateCaseWorkCommand>(ViewModel);
                var result = await _mediator.Send(cmd);
                if (result.Succeeded)
                    _notify.Success($"Case work with ID {result.Data} Created.");
                else _notify.Error(result.Message);
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region Today's Proceeding to the case
        public async Task<JsonResult> CaseProceeding(Guid CaseId)
        {
            string SelectedDate = DateTime.Now.ToString();
            if (TempData["SelectedDate"] != null)
            {
                SelectedDate = TempData["SelectedDate"].ToString();
                TempData.Keep();
            }

            var response = await _mediator.Send(new GetCaseProceedingByIdQuery()
            {
                CaseId = CaseId,
                SelectedDate = Convert.ToDateTime(SelectedDate)
            });
            var model = new CaseProceedingViewModel();
            model.CaseId = CaseId;
            model.IsUpdate = false;
            if (response.Succeeded)
            {
                var dt = response.Data;
                model.FirstTitle = dt.FirstTitle;
                model.SecondTitle = dt.SecondTitle;
                model.No = dt.No;
                model.Year = dt.Year;
                model.Court = dt.Court;
                model.CaseType = dt.CaseType;
                model.Stage = dt.Stage;
                model.MCasIds = dt.ParentChildCaseIds;
                if (dt.HeadId != Guid.Empty)
                {
                    model = _mapper.Map<CaseProceedingViewModel>(dt);
                    model.ProceedingTypes = await DdlProcHeads();
                    model.Proceedings = await DdlSubProc(model.HeadId);
                    model.ProcWork.WorkTypes = await DdlWorks();
                    var WorkTypeId = model.ProcWork.Workdt.Select(s => s.WorkTypeId).FirstOrDefault();
                    for (int i = 0; i < model.ProcWork.Workdt.Count(); i++)
                    {
                        model.ProcWork.Workdt[i].Works = await DdlSubWork(model.ProcWork.Workdt[i].WorkTypeId.Value);
                    }
                    model.Stages = await DdlCaseStages();
                    model.IsUpdate = true;
                }
                else
                {
                    var wmodel = new CaseWorkingViewModel();
                    wmodel.Workdt = new List<ProcWork> { new ProcWork() };
                    wmodel.WorkTypes = await DdlWorks();
                    model.ProceedingTypes = await DdlProcHeads();
                    model.Stages = await DdlCaseStages();
                    model.ProcWork = wmodel;
                }
            }
            model.ProceedingDate = Convert.ToDateTime(SelectedDate).ToString("dd/MM/yyyy");
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CaseProceeding", model) });

        }
        public async Task<JsonResult> BulkProceeding(string CaseIds)
        {
            var SelectedDate = TempData["SelectedDate"].ToString();
            TempData.Keep();
            var model = new CaseProceedingViewModel();
            model.MCasIds = CaseIds.Split(',').Select(Guid.Parse).ToList();
            var wmodel = new CaseWorkingViewModel();
            wmodel.WorkTypes = await DdlWorks();
            model.ProceedingTypes = await DdlProcHeads();
            model.Stages = await DdlCaseStages();
            model.ProcWork = wmodel;
            model.ProceedingDate = Convert.ToDateTime(SelectedDate).ToString("dd/MM/yyyy");
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CaseProceeding", model) });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCaseProceeding(CaseProceedingViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsUpdate)
                {
                    var up = _mapper.Map<UpdateCaseProceedingCommand>(model);
                    up.ProceedingDate = Convert.ToDateTime(TempData["SelectedDate"].ToString());
                    bool hasValues = up.ProcWork.GetType().GetProperties()
                                       .Any(prop => prop.GetValue(up.ProcWork) != null);
                    if (!hasValues)
                        up.ProcWork = null;
                    var result = await _mediator.Send(up);
                    if (result.Succeeded)
                        _notify.Success($"Case proceeding updated successfull!");
                }
                else
                {
                    var cmd = _mapper.Map<CreateCaseProceedingCommand>(model);
                    cmd.UserId = CurrentUser.Id;
                    cmd.ProceedingDate = Convert.ToDateTime(TempData["SelectedDate"].ToString());
                    bool hasValues = cmd.ProcWork.GetType().GetProperties()
                                       .Any(prop => prop.GetValue(cmd.ProcWork) != null);
                    if (!hasValues)
                        cmd.ProcWork = null;
                    var result = await _mediator.Send(cmd);
                    if (result.Succeeded)
                        _notify.Success($"Case proceeding done successfull!");
                }
            }
            return RedirectToAction("Index", new { SelectedDate = TempData["SelectedDate"].ToString() });
        }
        #endregion

    }
}
