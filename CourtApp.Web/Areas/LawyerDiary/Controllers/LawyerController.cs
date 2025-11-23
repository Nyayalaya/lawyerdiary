using CourtApp.Application.Constants;
using CourtApp.Application.Features.CaseDetails;
using CourtApp.Application.Features.Lawyer;
using CourtApp.Application.Interfaces.Shared;
using CourtApp.Infrastructure.Identity.Models;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Admin.Models;
using CourtApp.Web.Areas.Client.Model;
using CourtApp.Web.Areas.LawyerDiary.Models.Lawyer;
using CourtApp.Web.Areas.Litigation.Models;
using CourtApp.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.LawyerDiary.Controllers
{
    [Area("LawyerDiary")]
    public class LawyerController : BaseController<LawyerController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly BlobService _blobService;
        private readonly IDocumentUploadService _docService;
        public LawyerController(UserManager<ApplicationUser> UserManager,
            IDocumentUploadService docService
            //BlobService blobService
            )
        {
            _userManager = UserManager;
            _docService = docService;
            //_blobService = blobService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> LoadAll()
        {
            var response = await _mediator.Send(new LawyerGetAllQuery()
            {
                PageNumber = 1,
                PageSize = 10000,
                LinkedIds = User.GetUserLinkedIds(),
                Role = CurrentUser.Role
            }
            );
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<LawyerLViewModel>>(response.Data);
                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }
        public async Task<JsonResult> OnGetCreateOrEdit(Guid id)
        {
            if (id == Guid.Empty)
            {
                var model = new LawyerUpsertViewModel();
                model.Genders = new SelectList(StaticDropDownDictionaries.Gender(), "Key", "Value");
                model.Relegions = new SelectList(StaticDropDownDictionaries.Relegions(), "Key", "Value");
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", model) });
            }
            else
            {
                var response = await _mediator.Send(new LawyerGetByIdQuery() { Id = id });
                if (response.Succeeded)
                {
                    var ViewModel = _mapper.Map<LawyerUpsertViewModel>(response.Data);
                    ViewModel.Genders = new SelectList(StaticDropDownDictionaries.Gender(), "Key", "Value");
                    ViewModel.Relegions = new SelectList(StaticDropDownDictionaries.Relegions(), "Key", "Value");
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ViewModel) });
                }
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEdit(LawyerUpsertViewModel ViewModel, IFormFile ProfileImgFile)
        {
            if (ModelState.IsValid)
            {
                bool isUpdating = ViewModel.Id == Guid.Empty ? false : true;
                string oldImagePath = string.Empty;
                string newImagePath = string.Empty;
                //if (ProfileImgFile != null)
                //{
                //    string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(ProfileImgFile.FileName);
                //    using var stream = ProfileImgFile.OpenReadStream();
                //    newImagePath = await _docService.UploadFileAsync(stream, fileName, "ProfileImage");                    
                //}
                if (!isUpdating)
                {
                    var ldModel = _mapper.Map<LawyerCreateCommand>(ViewModel);
                    ldModel.ProfileImgPath = newImagePath;
                    var result = await _mediator.Send(ldModel);
                    if (result.Succeeded)
                        _notify.Success($"Lawyer with ID {result.Data} Created.");
                    else _notify.Error(result.Message);
                }
                else
                {
                    var upModel = _mapper.Map<LawyerUpdateCommand>(ViewModel);
                    upModel.ProfileImgPath = newImagePath;
                    var result = await _mediator.Send(upModel);
                    if (result.Succeeded)
                    {
                        if (result.Data != null)
                            await _docService.DeleteFileAsync(upModel.ProfileImgPath);
                        _notify.Information($"Lawyer with ID {result.Data} Updated.");
                    }
                }
                var response = await _mediator.Send(new LawyerGetAllQuery
                {
                    Role = CurrentUser.Role,
                    LinkedIds = User.GetUserLinkedIds()
                });
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<LawyerLViewModel>>(response.Data);
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
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ViewModel);
                return new JsonResult(new { isValid = false, html = html });
            }
        }

        [HttpPost]
        public async Task<JsonResult> OnPostDelete(Guid id)
        {
            var result = await _mediator.Send(new LawyerDeleteCommand { Id = id });
            if (result.Succeeded)
            {
                await _docService.DeleteFileAsync(result.Message);
                _notify.Information($"Lawyer with Id {id} Deleted.");
                var response = await _mediator.Send(new LawyerGetAllQuery());
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<LawyerLViewModel>>(response.Data);
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
                _notify.Error(result.Message);
                return null;
            }
        }

        #region Bring Case Detail By Lawyer 
        public async Task<IActionResult> GetCaseByLawyer()
        {
            var model = new BringCaseViewModel();
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.Users
                .Where(a => !User.GetUserLinkedIds().Contains(a.Id) && a.IsActive == true).ToListAsync();
            var modelUser = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
            var lawyerSelectList = modelUser.Select(x => new
            {
                Id = x.Id,
                FullDisplay = $"{x.FirstName} {x.LastName}"
            });

            model.Lawyers = new SelectList(lawyerSelectList, "Id", "FullDisplay");
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_BringCaseDetail", model) });
        }
        public async Task<IActionResult> GetClientDetailByCase(Guid CaseId)
        {
            var response = await _mediator.Send(new GetClientDetailByCaseIdQuery
            {
                CaseId = CaseId
            });
            if (response.Succeeded)
            {
                var dt = _mapper.Map<ClientViewModel>(response.Data);
                var data = Json(dt);
                return data;
            }
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCaseByLawyer(BringCaseViewModel vmodel)
        {
            if (ModelState.IsValid)
            {
                _notify.Success("Case detail fetched!");
                return RedirectToAction("BindLawyerCaseDetail", "CaseManage", new { area = "Litigation", id = vmodel.CaseId });
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_BringCaseDetail", vmodel);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
        #endregion

        #region Load Lawyer by name

        public async Task<JsonResult> GetLawyer(string refral)
        {
            var result = await _mediator.Send(new GetLawyerByNameQuery { Referal = refral });
            if (result.Succeeded)
            {
                return Json(result.Data);
            }
            else
            {
                _notify.Error(result.Message);
                return null;
            }
        }
        #endregion
    }
}
