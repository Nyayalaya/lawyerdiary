using AspNetCoreHero.Results;
using CourtApp.Application.Features.CaseTitle;
using CourtApp.Application.Features.FSTitle;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.LawyerDiary.Models.Title;
using CourtApp.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CourtApp.Web.Areas.LawyerDiary.Controllers
{
    [Area("LawyerDiary")]
    public class TitleController : BaseController<TitleController>
    {
        #region Case Complete Detail Title 
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LoadAll()
        {
            var response = await _mediator.Send(new GetCaseTitleQuery()
            {
                PageNumber = 1,
                PageSize = 5000,
                LinkedIds = User.GetUserLinkedIds(),
            });
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<TitleGetViewModel>>(response.Data);
                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<JsonResult> OnCreatOrUpdateTitle(Guid id)
        {
            TitleViewModel viewModel;

            if (id == Guid.Empty)
            {
                viewModel = new TitleViewModel
                {
                    Types = FSTypes(),
                    CaseApplicants = new List<ApplicantDetailViewModel>
                                    {
                                      new ApplicantDetailViewModel {
                                          ApplicantNo = "1", ApplicantDetail = "" }
                                    },
                    Cases = await UserCaseTitle(Guid.Empty)
                };
            }
            else
            {
                var response = await _mediator.Send(new GetCaseTitleByIdQuery { Id = id });
                if (!response.Succeeded)
                    return new JsonResult(new { isValid = false, html = "Record not found." });

                viewModel = _mapper.Map<TitleViewModel>(response.Data);
                viewModel.Types = FSTypes();
                viewModel.Cases = await UserCaseTitle(Guid.Empty);
            }

            return await RenderForm(viewModel, true, "_CreateOrUpdate");
        }

        [HttpPost]
        public async Task<JsonResult> OnCreatOrUpdateTitle(Guid id, TitleViewModel model)
        {
            if (!ModelState.IsValid)
                return await RenderForm(model, false, "_CreateOrUpdate");

            Result<Guid> result;

            if (id == Guid.Empty)
            {
                var createCommand = _mapper.Map<CreateCaseTitleCommand>(model);
                createCommand.CaseApplicants = _mapper.Map<List<CaseApplicantDetail>>(model.CaseApplicants);

                result = await _mediator.Send(createCommand);

                if (result.Succeeded)
                {
                    id = result.Data;
                    _notify.Success($"Case Title with ID {id} Created.");
                }
                else
                {
                    _notify.Error(result.Message);
                    return await RenderForm(model, false, "_CreateOrUpdate");
                }
            }
            else
            {
                var updateCommand = _mapper.Map<UpdateCaseTitleCommand>(model);
                result = await _mediator.Send(updateCommand);

                if (result.Succeeded)
                {
                    _notify.Information($"Case Title ID {result.Data} Updated.");
                }
                else
                {
                    _notify.Error(result.Message);
                    return await RenderForm(model, false, "_CreateOrUpdate");
                }
            }

            // Refresh view
            var response = await _mediator.Send(new GetCaseTitleQuery());
            if (!response.Succeeded)
            {
                _notify.Error(response.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

            var viewModel = _mapper.Map<List<TitleGetViewModel>>(response.Data);
            var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
            return new JsonResult(new { isValid = true, html });

        }

        [HttpPost]
        public async Task<JsonResult> OnTitleDelete(Guid id)
        {
            var deleteCommand = await _mediator.Send(new DeleteCaseTitleCommand { Id = id });
            if (deleteCommand.Succeeded)
            {
                _notify.Information($"Complete title with Id {id} deleted successfully");
                var response = await _mediator.Send(new GetCaseTitleQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<TitleGetViewModel>>(response.Data);
                    var chtml = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                    return new JsonResult(new { isValid = true, html = chtml });
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

        #endregion 

        #region First & Secound Title 
        public IActionResult FSTitle()
        {
            return View();
        }
        public async Task<IActionResult> LoadFSTitle()
        {
            var response = await _mediator.Send(new FSTitleGetAllQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<FSTitleLViewModel>>(response.Data);
                return PartialView("_FSTitles", viewModel);
            }
            return null;
        }
        public async Task<JsonResult> OnGetFSCreateOrUpdate(Guid id)
        {
            var viewModel = id == Guid.Empty
                                    ? new FSTitleMViewModel()
                                    : await GetFSTitleViewModelById(id);

            if (viewModel == null)
                return new JsonResult(new { isValid = false, html = "Record not found." });

            viewModel.FSTypes = FSTypes();

            return await RenderForm(viewModel, true, "_CreateUpdateFSTitle");
        }


        [HttpPost]
        public async Task<JsonResult> OnPostFSCreateOrUpdate(Guid id, FSTitleMViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return await RenderForm(model, false, "_CreateUpdateFSTitle");
            }

            Result<Guid> result;

            if (id == Guid.Empty)
            {
                var createCommand = _mapper.Map<FSTitleCreateCommand>(model);
                result = await _mediator.Send(createCommand);

                if (result.Succeeded)
                {
                    id = result.Data;
                    _notify.Success($"First & Second with ID {result.Data} Created.");
                }
                else
                {
                    _notify.Error(result.Message);
                    return await RenderForm(model, false, "_CreateUpdateFSTitle");
                }
            }
            else
            {
                var updateCommand = _mapper.Map<FSTitleUpdateCommand>(model);
                result = await _mediator.Send(updateCommand);

                if (result.Succeeded)
                {
                    _notify.Information($"First & Second with ID {result.Data} Updated.");
                }
                else
                {
                    _notify.Error(result.Message);
                    return await RenderForm(model, false, "_CreateUpdateFSTitle");
                }
            }

            var response = await _mediator.Send(new FSTitleGetAllQuery());
            if (!response.Succeeded)
                return null;

            var viewModel = _mapper.Map<List<FSTitleLViewModel>>(response.Data);
            var listHtml = await _viewRenderer.RenderViewToStringAsync("_FSTitles", viewModel);

            return new JsonResult(new { isValid = true, html = listHtml });
        }

        [HttpPost]
        public async Task<JsonResult> OnPostDelete(Guid id)
        {
            var deleteResult = await _mediator.Send(new FSTitleDeleteCommand { Id = id });

            if (!deleteResult.Succeeded)
            {
                _notify.Error(deleteResult.Message);
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

            _notify.Information($"Title with Id {id} Deleted.");

            var response = await _mediator.Send(new FSTitleGetAllQuery());
            if (!response.Succeeded)
            {
                return new JsonResult(new { isValid = false, html = string.Empty });
            }

            var viewModel = _mapper.Map<List<FSTitleLViewModel>>(response.Data);
            var html = await _viewRenderer.RenderViewToStringAsync("_FSTitles", viewModel);

            return new JsonResult(new { isValid = true, html });
        }

        #endregion

        private async Task<JsonResult> LoadFSDataAsync()
        {
            var response = await _mediator.Send(new FSTitleGetAllQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<FSTitleLViewModel>>(response.Data);
                var html = await _viewRenderer.RenderViewToStringAsync("_FSTitles", viewModel);
                return new JsonResult(new { isValid = true, html = html });
            }
            else
            {
                _notify.Error(response.Message);
                return null;
            }
        }

        private async Task<FSTitleMViewModel> GetFSTitleViewModelById(Guid id)
        {
            var response = await _mediator.Send(new FSTitleGetByIdQuery { Id = id });
            if (!response.Succeeded)
                return null;

            return _mapper.Map<FSTitleMViewModel>(response.Data);
        }
    }
}
