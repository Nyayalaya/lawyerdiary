using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using CourtApp.Application.Constants;
using CourtApp.Application.DTOs.CourtMaster;
using CourtApp.Application.Features.Cadre;
using CourtApp.Application.Features.CaseCategory;
using CourtApp.Application.Features.CaseDetails;
using CourtApp.Application.Features.CaseKinds.Query;
using CourtApp.Application.Features.CaseStages.Query;
using CourtApp.Application.Features.CaseTitle;
using CourtApp.Application.Features.Clients.Queries.GetAllCached;
using CourtApp.Application.Features.CourtComplex;
using CourtApp.Application.Features.CourtDistrict;
using CourtApp.Application.Features.CourtMasters;
using CourtApp.Application.Features.CourtType.Query;
using CourtApp.Application.Features.DOType;
using CourtApp.Application.Features.FormBuilder;
using CourtApp.Application.Features.FSTitle;
using CourtApp.Application.Features.Lawyer;
using CourtApp.Application.Features.ProceedingHead;
using CourtApp.Application.Features.ProceedingSubHead;
using CourtApp.Application.Features.Queries.Districts;
using CourtApp.Application.Features.Queries.States;
using CourtApp.Application.Features.TypeOfCases.Query;
using CourtApp.Application.Features.WorkMaster;
using CourtApp.Application.Features.WorkMasterSub;
using CourtApp.Web.Areas.Admin.Models;
using CourtApp.Web.Areas.Client.Model;
using CourtApp.Web.Areas.LawyerDiary.Models;
using CourtApp.Web.Areas.LawyerDiary.Models.Lawyer;
using CourtApp.Web.Areas.LawyerDiary.Models.Title;
using CourtApp.Web.Areas.Litigation.Models;
using CourtApp.Web.Extensions;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Google.Apis.Drive.v3.Data;
using HtmlAgilityPack;
using HtmlToOpenXml;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CourtApp.Web.Abstractions
{
    [Authorize]
    public abstract class BaseController<T> : Controller
    {
        protected UserViewModel CurrentUser { get; private set; }
        // Override OnActionExecuting to populate user details
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (User.Identity.IsAuthenticated)
            {
                CurrentUser = new UserViewModel
                {
                    Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    UserName = User.FindFirstValue(ClaimTypes.Name),
                    Email = User.FindFirstValue(ClaimTypes.Email),
                    Role = User.FindFirstValue(ClaimTypes.Role),
                    FirstName = User?.FindFirst("FirstName")?.Value,
                    LastName = User?.FindFirst("LastName")?.Value,
                    Address = User.FindFirstValue(ClaimTypes.StreetAddress),
                    Mobile = User.FindFirstValue(ClaimTypes.MobilePhone),
                    // Retrieve other claims as needed
                };
            }
        }

        private IMediator _mediatorInstance;
        private ILogger<T> _loggerInstance;
        private IViewRenderService _viewRenderInstance;
        private IMapper _mapperInstance;
        private INotyfService _notifyInstance;
        protected INotyfService _notify => _notifyInstance ??= HttpContext.RequestServices.GetService<INotyfService>();

        protected IMediator _mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();
        protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
        protected IViewRenderService _viewRenderer => _viewRenderInstance ??= HttpContext.RequestServices.GetService<IViewRenderService>();
        protected IMapper _mapper => _mapperInstance ??= HttpContext.RequestServices.GetService<IMapper>();


        [HttpGet]
        public IActionResult IsSessionActive()
        {
            bool isAuthenticated = User.Identity.IsAuthenticated;
            return Json(new { isActive = isAuthenticated });
        }


        protected async Task<JsonResult> RenderForm<TModel>(TModel model, bool isValid, string viewName)
        {
            var html = await _viewRenderer.RenderViewToStringAsync(viewName, model);
            return new JsonResult(new { isValid, html });
        }


        #region Dropdown Select List
        public async Task<SelectList> LoadStates()
        {
            var response = await _mediator.Send(new GetStateMasterQuery());
            var ViewModel = _mapper.Map<List<StateViewModel>>(response.Data);
            return new SelectList(ViewModel, nameof(StateViewModel.Id), nameof(StateViewModel.Name_En), null, null);
        }
        public async Task<SelectList> LoadCourtTypes()
        {
            var response = await _mediator.Send(new GetCourtTypeQuery());
            var CaseKind = _mapper.Map<List<CourtTypeViewModel>>(response.Data);
            return new SelectList(CaseKind, nameof(CourtTypeViewModel.Id), nameof(CourtTypeViewModel.CourtType), nameof(CourtTypeViewModel.Abbreviation), null);
        }
        public async Task<SelectList> DdlLoadCourtDistricts(int StateID)
        {
            var districts = await _mediator.Send(new GetCourtDistrictCachedQuery() { StateId = StateID });
            var districtViewModel = _mapper.Map<List<CourtDistrictViewModel>>(districts.Data);
            return new SelectList(districtViewModel, nameof(CourtDistrictViewModel.Id), nameof(CourtDistrictViewModel.Name_En), null, null);
        }
        public async Task<SelectList> DdlCourt()
        {
            var response = await _mediator.Send(new GetCourtTypeQuery());
            var ViewModel = _mapper.Map<List<CourtTypeViewModel>>(response.Data);
            return new SelectList(ViewModel, nameof(CourtTypeViewModel.Id), nameof(CourtTypeViewModel.CourtType), null, null);
        }
        public async Task<SelectList> LoadCaseNature()
        {
            var response = await _mediator.Send(new GetQueryCaseCategory());
            var CaseNatures = _mapper.Map<List<CaseNatureViewModel>>(response.Data);
            return new SelectList(CaseNatures, nameof(CaseNatureViewModel.Id), nameof(CaseNatureViewModel.Name_En), null, null);
        }
        public async Task<SelectList> LoadCaseNatureByCourtType(Guid CourtTypeId)
        {
            var response = await _mediator.Send(new GetQueryCaseCategory { CourtTypeId = CourtTypeId });
            var CaseNatures = _mapper.Map<List<CaseNatureViewModel>>(response.Data);
            return new SelectList(CaseNatures, nameof(CaseNatureViewModel.Id), nameof(CaseNatureViewModel.Name_En), null, null);
        }

        public async Task<SelectList> ddlCaseCategory(Guid CourtTypeId)
        {
            var response = await _mediator.Send(new GetQueryCaseCategory());
            var CaseNatures = _mapper.Map<List<CaseNatureViewModel>>(response.Data);
            return new SelectList(CaseNatures, nameof(CaseNatureViewModel.Id), nameof(CaseNatureViewModel.Name_En), null, null);
        }
        public async Task<JsonResult> LoadCaseCategory(Guid CourtTypeId)
        {
            var response = await _mediator.Send(new GetQueryCaseCategory { CourtTypeId = CourtTypeId });
            var CaseNatures = _mapper.Map<List<CaseNatureViewModel>>(response.Data);
            var data = Json(CaseNatures);
            return data;
        }
        public async Task<SelectList> LoadCaseKinds()
        {
            var response = await _mediator.Send(new CaseKindAllCacheQuery());
            var CaseKind = _mapper.Map<List<CaseKindViewModel>>(response.Data);
            return new SelectList(CaseKind, nameof(CaseKindViewModel.Id), nameof(CaseKindViewModel.CaseKind), null, null);

        }
        public async Task<SelectList> DdlLoadDistrict(int StateCode)
        {
            var districts = await _mediator.Send(new GetDistrictQuery() { StateCode = StateCode });
            var districtViewModel = _mapper.Map<List<DistrictViewModel>>(districts.Data);
            return new SelectList(districtViewModel, nameof(DistrictViewModel.Id), nameof(DistrictViewModel.Name_En), null, null);
        }
        public async Task<SelectList> DdlCaseStages()
        {
            var stages = await _mediator.Send(new CaseStageCacheAllQuery());
            var caseStagesViewModel = _mapper.Map<List<CaseStageViewModel>>(stages.Data);
            return new SelectList(caseStagesViewModel, nameof(CaseStageViewModel.Id), nameof(CaseStageViewModel.CaseStage), null, null);
        }
        public async Task<SelectList> DdlClient(string UserId)
        {
            var response = await _mediator.Send(new GetAllClientCachedQuery()
            {
                LinkedIds = User.GetUserLinkedIds(),
                PageSize = 10000,
                PageNumber = 1
            });
            var viewModel = _mapper.Map<List<GClientViewModel>>(response.Data);
            Dictionary<Guid, string> Clients = new Dictionary<Guid, string>();
            foreach (var item in viewModel)
            {
                var name = item.Name + " (" + item.Mobile + " - " + item.Address + ")";
                Clients.Add(item.Id, name);
            }
            return new SelectList(Clients, "Key", "Value");
            //return new SelectList(viewModel, nameof(GClientViewModel.Id), nameof(GClientViewModel.Name), null, null);
        }

        public async Task<SelectList> UserCaseTitle(Guid? CaseId)
        {
            var response = await _mediator.Send(new GetCaseInfoQuery()
            {
                PageSize = 10000,
                PageNumber = 1,
                LinkedIds = User.GetUserLinkedIds(),
            });
            var viewModel = _mapper.Map<List<GetCaseInfoViewModel>>(CaseId != null && CaseId != Guid.Empty ? response.Data.Where(w => w.Id != CaseId.Value) : response.Data);
            var selectListItems = viewModel.Select(v => new
            {
                Id = v.Id,
                CaseDisplay = v.CaseDetail != null ? $"{v.CaseDetail}[{v.No}/{v.Year}]" : "[N/A]"
            }).ToList();
            return new SelectList(selectListItems, "Id", "CaseDisplay");
        }

        public async Task<JsonResult> DdlReferBy()
        {
            var response = await _mediator.Send(new GetClientInfoCacheQuery() { UserId = CurrentUser.Id });
            if (response.Succeeded)
            {
                var dt = response.Data;
                var result = dt.Where(d => d.ReferalBy != string.Empty)
                    .OrderBy(r => r.ReferalBy)
                    .DistinctBy(d => d.ReferalBy).ToList();
                var ViewModel = _mapper.Map<List<DropDownSViewModel>>(result);
                return Json(ViewModel);
            }
            return null;
        }
        public async Task<JsonResult> DdlClients()
        {
            var response = await _mediator.Send(new GetClientInfoCacheQuery() { UserId = CurrentUser.Id });
            if (response.Succeeded)
            {
                var dt = response.Data;
                var result = dt.Where(d => d.Name != string.Empty)
                    .OrderBy(r => r.Name)
                    .DistinctBy(d => d.Name).ToList();
                var ViewModel = _mapper.Map<List<DropDownGViewModel>>(result);
                return Json(ViewModel);
            }
            return null;
        }

        #endregion

        public async Task<JsonResult> LoadDistricts(int StateCode)
        {
            var districts = await _mediator.Send(new GetDistrictQuery() { StateCode = StateCode });
            var data = Json(districts);
            return data;
        }
        public async Task<JsonResult> LoadCourtDistrict(int StateId)
        {
            var districts = await _mediator.Send(new GetCourtDistrictQuery() { StateId = StateId });
            var data = Json(districts);
            return data;
        }

        public async Task<JsonResult> LoadCourtDistrictByState(int StateId)
        {
            var districts = await _mediator.Send(new GetCourtDistrictCachedQuery() { StateId = StateId });
            var data = Json(districts);
            return data;
        }

        public async Task<JsonResult> LoadCourt(Guid CourtTypeId)
        {
            var response = await _mediator.Send(new GetCourtMasterAllQuery()
            {
                CourtTypeId = CourtTypeId

            });
            return Json(response);
        }
        public async Task<SelectList> CourtSelectList(Guid CourtTypeId)
        {
            var response = await _mediator.Send(new GetCourtMasterAllQuery()
            {
                CourtTypeId = CourtTypeId

            });
            if (response.Succeeded)
            {
                var fields = _mapper.Map<List<CourtMasterViewModel>>(response.Data);
                return new SelectList(fields, nameof(CourtMasterViewModel.Id), nameof(CourtMasterViewModel.District), null, null); ;
            }
            return null;
        }
        public async Task<JsonResult> LoadCourtType()
        {
            var response = await _mediator.Send(new GetCourtTypeQuery());
            return Json(response);
        }

        public async Task<JsonResult> LoadCourtComplex(Guid CDistrictId)
        {
            var response = await _mediator.Send(new GetCourtComplexCacheQuery() { CourtDistrictId = CDistrictId });
            return Json(response);
        }
        public async Task<SelectList> GetCourtComplex(Guid CourtDistrictId)
        {
            var response = await _mediator.Send(new GetCourtComplexQuery() { CourtDistrictId = CourtDistrictId, PageNumber = 1, PageSize = 1000 });
            var viewModel = _mapper.Map<List<CourtComplexViewModel>>(response.Data.OrderBy(o => o.Name_En));
            return new SelectList(viewModel, nameof(CourtComplexViewModel.Id), nameof(CourtComplexViewModel.Name_En), null, null);
        }

        public async Task<JsonResult> LoadTypeOfCase(Guid natureId)
        {
            var caseType = await _mediator.Send(new GetAllTypeOfCasesQuery(1, 10000) { CategoryId = natureId });
            var data = Json(caseType);
            return data;
        }
        public async Task<SelectList> CaseTypes(Guid CategoryId)
        {
            var response = await _mediator.Send(new GetAllTypeOfCasesQuery(1, 10000) { CategoryId = CategoryId });
            if (response.Succeeded)
            {
                var fields = _mapper.Map<List<TypeOfCasesViewModel>>(response.Data);
                return new SelectList(fields, nameof(TypeOfCasesViewModel.Id), nameof(TypeOfCasesViewModel.Name_En), null, null); ;
            }
            return null;
        }

        public async Task<JsonResult> LCCByCourtTypeStage(Guid CourtType, int StateId)
        {
            var CCatogories = await _mediator.Send(new GetQueryCaseCategory()
            {
                StateCode = StateId,
                CourtTypeId = CourtType
            });
            if (CCatogories.Succeeded)
            {
                var data = Json(CCatogories);
                return data;
            }
            return null;
        }

        public async Task<JsonResult> LoadCourtBench(Guid CourtTypeId, int StateId, Guid ComplexId, Guid CourtDistrict)
        {
            var dt = await _mediator.Send(new GetCourtBenchQuery(1, 1000) { StateId = StateId, CourtTypeId = CourtTypeId, CourtId = ComplexId, CourtDistrictId = CourtDistrict });
            var data = Json(dt);
            return data;
        }

        public async Task<SelectList> LoadBenches(Guid CourtTypeId, int StateId, Guid ComplexId, Guid CourtDistrict)
        {
            var dt = await _mediator.Send(new GetCourtBenchQuery(1, 1000) { StateId = StateId, CourtTypeId = CourtTypeId, CourtId = ComplexId, CourtDistrictId = CourtDistrict });
            if (dt.Succeeded)
            {
                var fields = dt.Data.OrderBy(o => o.CourtBench_En);
                return new SelectList(fields, nameof(CourtBenchResponse.Id), nameof(CourtBenchResponse.CourtBench_En), null, null); ;
            }
            return null;
        }

        #region Case Proceeding & Sub Proceeding
        public async Task<SelectList> DdlProcHeads()
        {
            var response = await _mediator.Send(new GetProceedingHeadQuery());
            var viewModel = _mapper.Map<List<ProceedingHeadViewModel>>(response.Data);
            return new SelectList(viewModel, nameof(ProceedingHeadViewModel.Id), nameof(ProceedingHeadViewModel.Name_En), null, null);
        }
        public async Task<JsonResult> DdlSubProcHeads(Guid Id)
        {
            var response = await _mediator.Send(new GetProceedingSubHeadQuery
            {
                HeadId = Id,
                PageNumber = 1,
                PageSize = 1500,
            });
            return Json(response);
        }
        public async Task<SelectList> DdlSubProc(Guid Id)
        {
            var response = await _mediator.Send(new GetProceedingSubHeadQuery
            {
                HeadId = Id,
                PageNumber = 1,
                PageSize = 1500,
            });
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<ProceedingSubHeadViewModel>>(response.Data);
                return new SelectList(viewModel, nameof(ProceedingSubHeadViewModel.Id), nameof(ProceedingSubHeadViewModel.Name_En), null, null);
            }
            return null;
        }
        #endregion

        #region Case Work And Sub Work Area
        public async Task<SelectList> DdlWorks()
        {
            var response = await _mediator.Send(new GetWorkMasterCommand());
            var viewModel = _mapper.Map<List<WorkMasterViewModel>>(response.Data);
            return new SelectList(viewModel, nameof(WorkMasterViewModel.Id), nameof(WorkMasterViewModel.Work_En), null, null);
        }
        public async Task<JsonResult> DdlSubWorks(Guid WorkId)
        {
            var response = await _mediator.Send(new GWorkSubMstQuery { WorkId = WorkId });
            return Json(response);
        }
        public async Task<SelectList> DdlSubWork(Guid Id)
        {
            var response = await _mediator.Send(new GWorkSubMstQuery { WorkId = Id });
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<WorkMasterSubViewModel>>(response.Data);
                return new SelectList(viewModel, nameof(WorkMasterSubViewModel.Id), nameof(WorkMasterSubViewModel.Name_En), null, null);
            }
            return null;
        }
        public async Task<SelectList> DdlCourts()
        {
            var response = await _mediator.Send(new GetCourtBenchQueryByName { CourtName = "" });
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<CourtBenchResponse>>(response.Data);
                return new SelectList(viewModel, nameof(CourtBenchResponse.Id), nameof(CourtBenchResponse.CourtBench_En), null, null);
            }
            return null;
        }
        #endregion

        #region DOType
        public SelectList DOTypes()
        {
            return new SelectList(StaticDropDownDictionaries.DOTypes(), "Key", "Value");
        }

        public JsonResult DOCTypes()
        {
            var dtcData = StaticDropDownDictionaries.DOTypes();
            return Json(dtcData);
        }

        public async Task<JsonResult> DdlDOTypes(int TypeId)
        {
            var response = await _mediator.Send(new GetAllDOTypeCachedQuery { TypeId = TypeId });
            return Json(response);
        }
        public async Task<SelectList> DdlCDOTypes(int TypeId)
        {
            var response = await _mediator.Send(new GetAllDOTypeCachedQuery { TypeId = TypeId });
            var ViewModel = _mapper.Map<List<DOTypeViewModel>>(response.Data);
            return new SelectList(ViewModel, nameof(DOTypeViewModel.Id), nameof(DOTypeViewModel.Name_En), null, null);
        }
        #endregion

        #region Static Dropdown Region

        public SelectList DdlYears()
        {
            return new SelectList(StaticDropDownDictionaries.Year(), "Key", "Value");
        }

        public SelectList FirstTtitleList()
        {
            return new SelectList(StaticDropDownDictionaries.FirstTitle(), "Key", "Value");
        }
        public SelectList SecondTtitleList()
        {
            return new SelectList(StaticDropDownDictionaries.SecoundTitle(), "Key", "Value");
        }

        public SelectList DdlCaseStatus()
        {
            return new SelectList(StaticDropDownDictionaries.CaseStatus().OrderBy(v => v.Value), "Key", "Value");
        }
        public SelectList DdlCaseTitle()
        {
            return new SelectList(StaticDropDownDictionaries.CaseTitle().OrderBy(v => v.Value), "Key", "Value");
        }
        public async Task<SelectList> DdlCadres()
        {
            var response = await _mediator.Send(new GetQueryCadre());
            if (response.Succeeded)
            {
                var fields = _mapper.Map<List<CadreMasterViewModel>>(response.Data.OrderBy(o => o.Name_En));
                return new SelectList(fields, nameof(CadreMasterViewModel.Id), nameof(CadreMasterViewModel.Name_En), null, null); ;
            }
            return null;
            //return new SelectList(StaticDropDownDictionaries.Cadres().OrderBy(v => v.Value), "Key", "Value");
        }
        public SelectList DdlStrength()
        {
            return new SelectList(StaticDropDownDictionaries.Stength().OrderBy(v => v.Key), "Key", "Value");
        }
        public SelectList FormPrintingTypes()
        {
            return new SelectList(StaticDropDownDictionaries.FormPrintingTypes(), "Key", "Value");
        }


        #endregion

        #region First & Secound Title
        public SelectList FSTypes()
        {
            return new SelectList(StaticDropDownDictionaries.FSType(), "Key", "Value");
        }

        public JsonResult FSJTypes()
        {
            var dtcData = StaticDropDownDictionaries.DOTypes();
            return Json(dtcData);
        }

        public async Task<JsonResult> DdlFSTitle(int TypeId)
        {
            var response = await _mediator.Send(new FSTitleGetAllCacheQuery { TypeId = TypeId });
            return Json(response);
        }
        public async Task<SelectList> DdlFSTypes(int TypeId)
        {
            var response = await _mediator.Send(new FSTitleGetAllCacheQuery { TypeId = TypeId });
            var ViewModel = _mapper.Map<List<FSTitleLViewModel>>(response.Data);
            return new SelectList(ViewModel, nameof(FSTitleLViewModel.Id), nameof(FSTitleLViewModel.Name_En), null, null);
        }
        #endregion

        #region Lawyer Master
        public async Task<SelectList> DdlLawyerAsync()
        {
            var response = await _mediator.Send(new LawyerGetAllCacheQuery());
            var ViewModel = _mapper.Map<List<LawyerLViewModel>>(response.Data);
            return new SelectList(ViewModel, nameof(LawyerLViewModel.Id), nameof(LawyerLViewModel.FirstName), null, null);
        }
        #endregion

        #region Generilize Dropdowns
        public JsonResult FieldType()
        {
            var years = StaticDropDownDictionaries.FieldType().OrderBy(o => o.Key);
            List<DropDownSViewModel> dt = new List<DropDownSViewModel>();
            foreach (var y in years)
                dt.Add(new DropDownSViewModel { Id = y.Key.ToString(), Name = y.Value });
            return Json(dt);
        }

        public async Task<SelectList> GetForms()
        {
            var response = await _mediator.Send(new GetFormBuilderCachedQuery());
            if (response.Succeeded)
            {
                var fields = _mapper.Map<List<GenFormAttrViewModel>>(response.Data.OrderBy(o => o.FormName));
                return new SelectList(fields, nameof(GenFormAttrViewModel.Id), nameof(GenFormAttrViewModel.FormName), null, null); ;
            }
            return null;
        }
        public async Task<JsonResult> GetFormFieldsById(Guid id)
        {
            var response = await _mediator.Send(new GetFormBuilderCachedByIdQuery() { Id = id });
            List<DropDownGViewModel> dt = new List<DropDownGViewModel>();
            if (response.Succeeded)
            {
                var fields = response.Data.FieldDetails;
                foreach (var y in fields)
                    dt.Add(new DropDownGViewModel { Id = y.Key, Name = y.Name });
            }
            return Json(dt);
        }
        public async Task<JsonResult> TemplateFields(Guid TemplateId)
        {
            var response = await _mediator.Send(new GetTemplateInfoCachedByIdQuery() { Id = TemplateId });
            List<DropDownSViewModel> dt = new List<DropDownSViewModel>();
            if (response.Succeeded)
            {
                var fields = response.Data.Tags;
                foreach (var y in fields)
                    dt.Add(new DropDownSViewModel { Id = y.Tag, Name = y.Tag });
            }
            return Json(dt);
        }
        public JsonResult SearchBy()
        {
            var years = StaticDropDownDictionaries.CaseSearchBy().OrderBy(o => o.Value);
            List<DropDownSViewModel> dt = new List<DropDownSViewModel>();
            foreach (var y in years)
                dt.Add(new DropDownSViewModel { Id = y.Key, Name = y.Value });
            return Json(dt);
        }
        public JsonResult DdJYears()
        {
            var years = StaticDropDownDictionaries.Year();
            List<DropDownIViewModel> dt = new List<DropDownIViewModel>();
            foreach (var y in years)
                dt.Add(new DropDownIViewModel { Id = y.Key, Name = y.Value });
            return Json(dt);
        }
        public async Task<JsonResult> GDdlDOTypes(int TypeId)
        {
            var response = await _mediator.Send(new GetAllDOTypeCachedQuery { TypeId = TypeId });
            if (response.Succeeded)
            {
                var dt = _mapper.Map<List<DropDownGViewModel>>(response.Data);
                return Json(dt);
            }
            return Json(null);
        }
        public async Task<JsonResult> GDdlStages()
        {
            var response = await _mediator.Send(new CaseStageCacheAllQuery());
            if (response.Succeeded)
            {
                var dt = _mapper.Map<List<DropDownGViewModel>>(response.Data);
                return Json(dt);
            }
            return Json(null);
        }
        public async Task<JsonResult> GDdlCaseCategory()
        {
            var response = await _mediator.Send(new GetQueryCaseCategory());
            if (response.Succeeded)
            {
                var dt = _mapper.Map<List<DropDownGViewModel>>(response.Data);
                return Json(dt);
            }
            return Json(null);
        }
        public async Task<JsonResult> GetCompTitlesByCases(List<Guid> caseIds)
        {
            var response = await _mediator.Send(new GetCaseTitleQuery() { CaseIds = caseIds });
            if (response.Succeeded)
            {
                var dtl = response.Data.FirstOrDefault();
                var titles = new List<DropDownSViewModel>();
                if (dtl != null)
                    foreach (var item in dtl.CaseApplicantDetails)
                    {
                        DropDownSViewModel d = new DropDownSViewModel();
                        d.Id = item.ApplicantNo.ToString();
                        d.Name = item.ApplicantDetail;
                        titles.Add(d);
                    }
                ;
                var fn = titles.Distinct().OrderBy(o => o.Name).ToList();
                return Json(fn);
            }
            return Json(null);
        }
        public async Task<SelectList> GetDraftings()
        {
            var response = await _mediator.Send(new GetFormBuilderCachedQuery());
            if (response.Succeeded)
            {
                var dt = _mapper.Map<List<DropDownGViewModel>>(response.Data);
                return new SelectList(dt, nameof(DropDownGViewModel.Id), nameof(DropDownGViewModel.Name), null, null);
            }
            return null;
        }

        public async Task<SelectList> GetTemplates()
        {
            var response = await _mediator.Send(new GetTemplateInfoQuery());
            if (response.Succeeded)
            {
                var dt = _mapper.Map<List<DropDownGViewModel>>(response.Data);
                return new SelectList(dt, nameof(DropDownGViewModel.Id), nameof(DropDownGViewModel.Name), null, null);
            }
            return null;
        }
        #endregion

        #region Read File
        //public string ReadTemplate(string fPath, string fName)
        //{
        //    string DirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "documents", "Templates");
        //    string filePath = Path.Combine(DirPath, fName);
        //    if (!System.IO.File.Exists(filePath))
        //        return "File Not found";
        //    string fileContent = string.Empty;
        //    using (FileStream inputFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //    {
        //        // Load the file stream into a Word document
        //        using (WordDocument document = new WordDocument(inputFileStream, FormatType.Automatic))
        //        {
        //            fileContent = document.GetText();
        //        }
        //    }
        //    return fileContent;
        //}
        public byte[] ConvertHtmlToWord(string htmlContent)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(
                    memoryStream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();

                    // Initialize HtmlConverter and convert HTML to Word
                    HtmlConverter converter = new HtmlConverter(mainPart);
                    converter.ParseBody(htmlContent);

                    // Apply right alignment and indentation
                    // Apply specific formatting based on the document's legal style
                    foreach (var paragraph in mainPart.Document.Body.Elements<Paragraph>())
                    {
                        // Create a new paragraph properties
                        ParagraphProperties paragraphProperties = new ParagraphProperties();
                        // Apply right alignment for all text
                        paragraphProperties.Append(new Justification() { Val = JustificationValues.Left });

                        // Set indentation to start from the middle of the page
                        paragraphProperties.Append(new Indentation() { Left = "3500" }); // 7200 is half of the typical page width in twips (approx 6 inches)
                        paragraphProperties.Append(new SpacingBetweenLines() { Before = "200", After = "200" });


                        // Apply right alignment for headings or important sections
                        //if (IsHeadingOrImportantSection(paragraph.InnerText))
                        //{
                        //    paragraphProperties.Append(new Justification() { Val = JustificationValues.Center });
                        //}
                        //else
                        //{
                        //    // Apply justified alignment for normal text
                        //    paragraphProperties.Append(new Justification() { Val = JustificationValues.Both });
                        //}

                        // Indentation and spacing for paragraphs
                        //paragraphProperties.Append(new Indentation() { Left = "0", Hanging = "360" });
                        //paragraphProperties.Append(new SpacingBetweenLines() { Before = "200", After = "200" });

                        // Apply paragraph properties
                        paragraph.PrependChild(paragraphProperties);
                    }

                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }

                return memoryStream.ToArray();
            }
        }
        // Helper method to determine if the paragraph is a heading or an important section
        private bool IsHeadingOrImportantSection(string text)
        {
            // This is a simple example; refine this method based on your specific headings or markers
            return text.Contains("IN THE HIGH COURT") || text.Contains("S.B. CIVIL MISC. APPEAL") ||
                   text.Contains("VERSUS") || text.Contains("PRAYER") || text.Contains("GROUNDS");
        }

        public void ConvertHtmlToOpenXml(Body body, string htmlContent)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            foreach (var node in htmlDoc.DocumentNode.ChildNodes)
            {
                if (node.Name == "h1")
                {
                    var paragraph = new Paragraph(new Run(new Text(node.InnerText.Replace("&nbsp;", " "))));
                    paragraph.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId() { Val = "Heading1" });
                    body.AppendChild(paragraph);
                }
                else if (node.Name == "p")
                {
                    var paragraph = new Paragraph(new Run(new Text(node.InnerText.Replace("&nbsp;", " "))));
                    body.AppendChild(paragraph);
                }
                // Add more HTML tag handling as needed
            }
        }

        #endregion

        #region File Compression

        /// <summary>
        /// File compression code
        /// </summary>
        /// <param name="file"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public async Task<string> CompressFileAsync(IFormFile file, string destinationPath)
        {
            string zipFilePath = destinationPath + ".zip"; // Adding .zip extension
            using (var fileStream = new FileStream(zipFilePath, FileMode.Create))
            using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                // Create a new zip entry and copy the file into it
                var zipEntry = zipArchive.CreateEntry(file.FileName);
                using (var entryStream = zipEntry.Open())
                {
                    await file.CopyToAsync(entryStream);
                }
            }
            return zipFilePath; // Return the path to the compressed file
        }

        /// <summary>
        /// Validates the uploaded file type.
        /// Allows only PDF and DOCX formats.
        /// </summary>
        public bool IsValidFileType(IFormFile file)
        {
            string[] allowedExtensions = { ".pdf", ".docx" };
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(fileExtension);
        }
        #endregion

    }
}