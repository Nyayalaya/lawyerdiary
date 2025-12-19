using CourtApp.Application.Features.CourtType.Command;
using CourtApp.Application.Features.CourtType.Query;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.LawyerDiary.Models;
using CourtApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.LawyerDiary.Controllers
{
    [Area("LawyerDiary")]
    public class CourtTypeController : BaseController<CourtTypeController>
    {
        public IActionResult Index()
        {
            var model = new CourtTypeViewModel();
            return View(model);
        }

        public async Task<IActionResult> LoadAll()
        {
            var response = await _mediator.Send(new GetCourtTypeQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<CourtTypeViewModel>>(response.Data);
                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(Guid id)
        {

            if (id == Guid.Empty)
            {

                var ViewModel = new CourtTypeViewModel
                {
                    Language = new List<LanguageViewModel>
                    {
                        new() { Code = "hi" }
                    }
                };
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ViewModel) });
            }
            else
            {
                var response = await _mediator.Send(new GetCourtTypeByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var courtViewModel = _mapper.Map<CourtTypeViewModel>(response.Data);
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", courtViewModel) });
                }
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEdit(Guid id, CourtTypeViewModel btViewModel)
        {
            if (ModelState.IsValid)
            {
                if (id == Guid.Empty)
                {
                    var createCommand = _mapper.Map<CreateCourtTypeCommand>(btViewModel);
                    var result = await _mediator.Send(createCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"Court type with ID {result.Data} Created.");
                    }
                    else
                    {
                        _notify.Error(result.Message);
                        return await RenderForm(btViewModel, false, "_CreateOrEdit");
                    }
                }
                else
                {
                    var updateCommand = _mapper.Map<UpdateCourtTypeCommand>(btViewModel);
                    var result = await _mediator.Send(updateCommand);
                    if (result.Succeeded) _notify.Information($"Court type with ID {result.Data} Updated.");
                    else
                    {
                        _notify.Error(result.Message);
                        return await RenderForm(btViewModel, false, "_CreateOrEdit");
                    }
                }
                var response = await _mediator.Send(new GetCourtTypeQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<CourtTypeViewModel>>(response.Data);
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                    return new JsonResult(new { isValid = true, html = html });
                }
                else
                {
                    _notify.Error(response.Message);
                    return null;
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", btViewModel);
                return new JsonResult(new { isValid = false, html = html });
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostDelete(Guid id)
        {
            var deleteCommand = await _mediator.Send(new DeleteCourtTypeCommand { Id = id });
            if (deleteCommand.Succeeded)
            {
                _notify.Information($"Court Type with Id {id} Deleted.");
                var response = await _mediator.Send(new GetCourtTypeQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<CourtTypeViewModel>>(response.Data);
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                    return new JsonResult(new { isValid = true, html = html });
                }
                else
                {
                    _notify.Error(response.Message);
                    return null;
                }
            }
            else
            {
                _notify.Error(deleteCommand.Message);
                return null;
            }
        }

    }
}
