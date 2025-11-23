using CourtApp.Application.Features.CaseWork;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Litigation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Litigation.Controllers
{
    [Area("Litigation")]
    public class CaseWorkController : BaseController<CaseWorkController>
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> LoadAll()
        {
            var response = await _mediator.Send(new GetAssignedWorkQuery
            {
                PageSize = 10000,
                PageNumber = 1,
                UserId = CurrentUser.Id
            });
            if (response.Succeeded)
            {
                var model = new PendingWorkDataViewModel();
                List<CaseTitleWorkData> work = new List<CaseTitleWorkData>();

                if (response.Data != null)
                {
                    foreach (var cd in response.Data)
                    {
                        var PWorkCase = new CaseTitleWorkData();
                        PWorkCase.CaseTitle = cd.CaseDetail;
                        PWorkCase.Id = cd.CaseId;
                        PWorkCase.WorkDate = cd.LastWorkingDate;
                        PWorkCase.ProceedingDate = cd.ProceedingDate;
                        List<WorkDt> wdt = new List<WorkDt>();
                        foreach (var w in cd.AWorks)
                        {
                            var wt = new WorkDt();
                            wt.Work = w.WorkDetail;
                            wt.Id = w.Id;
                            wt.WorkId = w.WorkId;
                            wdt.Add(wt);
                        }
                        PWorkCase.Works = wdt;
                        work.Add(PWorkCase);
                    }
                }
                model.PendingWork = work;
                return PartialView("_ViewAll", model);
            }
            return null;
        }

        public async Task<IActionResult> Update(PendingWorkDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PendingWork != null)
                {
                    var CaseDl = model.PendingWork.Select(s => new CaseTitleWorkData
                    {
                        Id = s.Id,
                        CaseTitle = s.CaseTitle,
                        Works = s.Works.Where(s => s.Selected == true).ToList()
                    });
                    var WorkDetails = CaseDl.SelectMany(s => s.Works)
                                        .GroupBy(w => w.Id) // Group by ProcId
                                        .Select(g => new ProcWorks
                                        {
                                            ProcId = g.Key,
                                            WorkIds = g.Select(w => w.WorkId).ToList()
                                        }).ToList();
                    if (WorkDetails != null)
                    {
                        var result = await _mediator.Send(new UpdateCWorkStatusCommand
                        {
                            //CWorkId = null,
                            Status = 1,
                            //ProcId = Guid.Empty,
                            ProcWorksDetails = WorkDetails
                        });
                        if (result.Succeeded) _notify.Information($"Case Work with ID {result.Data} Updated.");
                        else _notify.Error(result.Message);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        //public async Task<JsonResult> OnGetCreateOrEdit(Guid id)
        //{
        //    if (id == Guid.Empty)
        //    {
        //        var ViewModel = new CaseWorkingViewModel();
        //        ViewModel.WorkTypes = await DdlWorks();
        //        //ViewModel.Works = await DdlSubWorks();
        //        return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ViewModel) });
        //    }
        //    else
        //    {
        //        var response = await _mediator.Send(new CaseStageByIdQuery() { Id = id });
        //        if (response.Succeeded)
        //        {
        //            var brandViewModel = _mapper.Map<CaseWorkingViewModel>(response.Data);
        //            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", brandViewModel) });
        //        }
        //        return null;
        //    }
        //}

        //[HttpPost]
        //public async Task<JsonResult> OnPostCreateOrEdit(Guid id, CaseWorkingViewModel ViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (id == Guid.Empty)
        //        {
        //            var cmd = _mapper.Map<CreateCaseWorkCommand>(ViewModel);
        //            var result = await _mediator.Send(cmd);
        //            if (result.Succeeded)
        //            {
        //                id = result.Data;
        //                _notify.Success($"Case work with ID {result.Data} Created.");
        //            }
        //            else _notify.Error(result.Message);
        //        }
        //        else
        //        {
        //            var updateBookCommand = _mapper.Map<UpdateCaseStageCommand>(ViewModel);
        //            var result = await _mediator.Send(updateBookCommand);
        //            if (result.Succeeded) _notify.Information($"Case Nature with ID {result.Data} Updated.");
        //        }
        //        var response = await _mediator.Send(new GetAssignedWorkQuery { PageSize = 10, PageNumber = 1 });
        //        if (response.Succeeded)
        //        {
        //            var viewModel = _mapper.Map<List<CaseWorkingViewModel>>(response.Data);
        //            var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
        //            return new JsonResult(new { isValid = true, html = html });
        //        }
        //        else
        //        {
        //            _notify.Error(response.Message);
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ViewModel);
        //        return new JsonResult(new { isValid = false, html = html });
        //    }
        //}

        //public async Task<JsonResult> UpdateCaseWorkStatus(List<Guid> CWorkId)
        //{
        //    if (CWorkId != null)
        //    {
        //        var result = await _mediator.Send(new UpdateCWorkStatusCommand { CWorkId = CWorkId });
        //        if (result.Succeeded) _notify.Information($"Case Work with ID {result.Data} Updated.");
        //        else _notify.Error(result.Message);
        //    }
        //    var response = await _mediator.Send(new GetAssignedWorkQuery { PageSize = 10, PageNumber = 1 });
        //    if (response.Succeeded)
        //    {
        //        var viewModel = _mapper.Map<List<CaseWorkingViewModel>>(response.Data);
        //        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
        //        return new JsonResult(new { isValid = true, html = html });
        //    }
        //    else
        //    {
        //        _notify.Error(response.Message);
        //        return null;
        //    }
        //}
    }
}
