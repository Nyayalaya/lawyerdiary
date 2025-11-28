using CourtApp.Application.DTOs.FormBuilder;
using CourtApp.Application.Features.FormBuilder;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CourtApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GenerateFormController : BaseController<GenerateFormController>
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> LoadAll()
        {
            var response = await _mediator.Send(new GetFormBuilderCachedQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<GenFormAttrViewModel>>(response.Data);
                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }
        public async Task<JsonResult> OnGetCreateOrEdit(Guid id,string ntd)
        {
            if (id == Guid.Empty)
            {
                var ViewModel = new GenerateFormViewModel();
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ViewModel) });
            }
            else
            {
                var result = await _mediator.Send(new GetFormBuilderCachedByIdQuery() { Id = id,AccessFrom="MST" });
                if (result.Succeeded)
                {
                    var ViewModel = _mapper.Map<GenerateFormViewModel>(result.Data);
                    FormViewModel fm = new FormViewModel();
                    fm.Fields = _mapper.Map<List<FormFields>>(result.Data.FieldDetails);
                    ViewModel.Form = fm;
                    ViewModel.Mode = ntd;
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ViewModel) });
                }
            }
            return null;
        }

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEdit(Guid id,GenerateFormViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ViewModel);
                return new JsonResult(new { isValid = false, html });
            }

            try
            {
                if (id == Guid.Empty || ViewModel.Mode== "nwfd")
                {
                    var command = _mapper.Map<CreateFormBuilderCommand>(ViewModel);
                    command.Id = Guid.Empty;
                    command.Form = new FormFieldsDto
                    {
                        Fields = ViewModel.Form.Fields.Select(f => new FieldDetailsDto
                        {
                            Key = Guid.NewGuid(),
                            Name = f.Name,
                            Type = f.Type,
                            DefaultVal = f.DefaultVal,
                            Tag = f.Tag
                        }).ToList()
                    };

                    var result = await _mediator.Send(command);

                    if (result.Succeeded)
                        _notify.Success("Template data fields saved successfully!");
                    else
                        _notify.Error(result.Message);
                }
                else
                {
                    var command = _mapper.Map<UpdateFormBuilderCommand>(ViewModel);
                    var result = await _mediator.Send(command);

                    if (result.Succeeded)
                        _notify.Information("Template Attribute updated successfully.");
                    else
                        _notify.Error(result.Message);
                }

                var response = await _mediator.Send(new GetFormBuilderCachedQuery());

                if (!response.Succeeded)
                {
                    _notify.Error(response.Message);
                    return null;
                }

                var viewModel = _mapper.Map<List<GenFormAttrViewModel>>(response.Data);
                var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);

                return new JsonResult(new { isValid = true, html });
            }
            catch (Exception ex)
            {
                // Prefer logging over console in production scenarios
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostDelete(Guid id)
        {
            var deleteCommand = await _mediator.Send(new DeleteFormBuilderQueryCommand { Id = id });
            if (deleteCommand.Succeeded)
            {
                _notify.Information($"Selected template is deleted successfully!");
                var response = await _mediator.Send(new GetFormBuilderCachedQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<GenFormAttrViewModel>>(response.Data);
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
