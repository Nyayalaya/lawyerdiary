using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.Features.Case;
using CourtApp.Application.Features.CaseDetails;
using CourtApp.Application.Features.CaseProceeding;
using CourtApp.Application.Features.Clients.Commands;
using CourtApp.Application.Features.UserCase;
using CourtApp.Application.Interfaces.Shared;
using CourtApp.Infrastructure.DbContexts;
using CourtApp.Infrastructure.Identity.Models;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Admin.Models;
using CourtApp.Web.Areas.Client.Model;
using CourtApp.Web.Areas.Litigation.Models;
using CourtApp.Web.Extensions;
using CourtApp.Web.Helpers;
using CourtApp.Web.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Litigation.Controllers
{
    [Area("Litigation")]
    public class CaseManageController : BaseController<CaseManageController>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const long MaxFileSize = 30 * 1024 * 1024; // 200MB
        //private readonly BlobService _blobService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDocumentUploadService _documentUploadService;
        private readonly UploadSettings _settings;
        private readonly IdentityContext _identityDbContext;
        private readonly ActionRenderHelper _viewRenderHelper;

        public CaseManageController(IWebHostEnvironment _webHostEnvironment, /*BlobService _blobService,*/
            UserManager<ApplicationUser> _userManager, IDocumentUploadService _documentUploadService,
            IdentityContext identityDbContext, IOptions<UploadSettings> options,
            ActionRenderHelper _viewRenderHelper)
        {
            this._webHostEnvironment = _webHostEnvironment;
            //this._blobService = _blobService;
            this._userManager = _userManager;
            this._documentUploadService = _documentUploadService;
            _settings = options.Value;
            _identityDbContext = identityDbContext;
            this._viewRenderHelper = _viewRenderHelper;
        }

        #region Case Management Area
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadAllAsync([FromForm] DataTableRequest request)
        {
            int pageSize = request.length;
            int start = request.start;
            int pageNumber = (start / pageSize) + 1;

            var response = await _mediator.Send(new GetCaseInfoQuery
            {
                LinkedIds = User.GetUserLinkedIds(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = request.search?.value ?? "",   // Optional: for search
                SortColumn = request.columns?[request.order[0].column].data,
                SortDirection = request.order[0].dir
            });

            if (response.Succeeded)
            {

                List<GetCaseInfoViewModel> dataModels = new List<GetCaseInfoViewModel>();
                foreach (var item in response.Data)
                {
                    var actionModel = new CaseActionViewModel
                    {
                        CaseId = item.Id,
                        Reference = item.Reference,
                        IsCaseAssigned = item.IsCaseAssigned,
                        LawyerId = item.LawyerId
                    };
                    dataModels.Add(new GetCaseInfoViewModel
                    {
                        Id = item.Id,
                        CaseDetail = item.CaseDetail,
                        CaseStage = item.CaseStage,
                        CaseType = item.CaseType,
                        Court = item.Court,
                        CourtType = item.CourtType,
                        IsCaseAssigned = item.IsCaseAssigned,
                        NextDate = item.NextDate,
                        No = item.No,
                        Reference = item.Reference,
                        Year = item.Year,
                        ActionHtml = await _viewRenderHelper.RenderPartialViewToStringAsync("_CaseGroupActionPartial", actionModel)
                    });
                }

                _logger.LogInformation("Load all the user's cases successfully!");
                var dataTableResponse = new
                {
                    draw = request.draw,
                    recordsTotal = response.TotalCount,
                    recordsFiltered = response.TotalCount, // Filtered = Total when no filtering logic used
                    data = dataModels
                };

                return Json(dataTableResponse);
            }

            return Json(new { error = "Unable to load data" });

        }

        public async Task<IActionResult> LoadAll()
        {
            var response = await _mediator.Send(new GetCaseInfoQuery()
            {
                LinkedIds = User.GetUserLinkedIds(),
                PageSize = 10000,
                PageNumber = 1
            });
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<GetCaseInfoViewModel>>(response.Data);
                _logger.LogInformation("Load all the user's cases successfully!");
                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<IActionResult> CreateOrUpdateAsync(Guid id, string from)
        {
            _logger.LogInformation("Case create or edit form!");
            try
            {
                bool showHighCourt = false;
                bool AgIsHighCourt = false;
                var caseViewModel = new CaseUpseartViewModel();
                caseViewModel.from = from;
                caseViewModel.ClientId = TempData["ClientId"] != null ? (Guid)TempData["ClientId"] : Guid.Empty;
                CaseAgainstModel cam = new CaseAgainstModel();
                cam.AStates = await LoadStates();
                cam.ACourtTypes = await LoadCourtTypes();
                cam.AYears = DdlYears();
                cam.ACaseStatusList = await DdlCaseStages();
                cam.ALinkedBy = await UserCaseTitle(Guid.Empty);
                cam.ACadres = await DdlCadres();
                cam.AStrengths = DdlStrength();
                cam.ACaseNatures = await LoadCaseNature();
                if (from == "repeat") id = Guid.Empty;
                if (id == Guid.Empty && (from == null || from == ""))
                {
                    ViewBag.Title = "Add New";
                    caseViewModel.InstitutionDate = DateTime.Now;
                    caseViewModel.States = await LoadStates();
                    caseViewModel.CourtTypes = await LoadCourtTypes();
                    caseViewModel.CaseNatures = await LoadCaseNature();
                    caseViewModel.FirstTitleList = await DdlFSTypes(1);
                    caseViewModel.SecondTitleList = await DdlFSTypes(2);
                    caseViewModel.Years = DdlYears();
                    caseViewModel.CaseStatusList = await DdlCaseStages();
                    caseViewModel.LinkedBy = await UserCaseTitle(Guid.Empty);
                    caseViewModel.Cadres = await DdlCadres();
                    caseViewModel.Strengths = DdlStrength();
                    ViewBag.ActionType = "Save";
                    ViewBag.ShowHighCourt = showHighCourt;
                    ViewBag.AgIsHighCourt = AgIsHighCourt;
                    caseViewModel.ClientList = await DdlClient(CurrentUser.Id);
                    caseViewModel.Appearences = await DdlFSTypes(0);
                    var agcl = new List<CaseAgainstModel>();
                    agcl.Add(cam);
                    caseViewModel.AgainstCaseDetails = agcl;
                    ViewBag.Id = id.ToString();
                    _logger.LogInformation("Case entry form loaded successfully!");
                    return View("_CreateOrEdit", caseViewModel);
                }
                else
                {
                    var response = await _mediator.Send(new GetUserCaseDetailByIdQuery
                    {
                        CaseId = id,
                        LinkedIds = User.GetUserLinkedIds(),
                    });
                    if (response.Succeeded)
                    {
                        var CaseDetail = _mapper.Map<CaseUpseartViewModel>(response.Data);
                        if (from == "cp")
                        {
                            ViewBag.Title = "Add New";
                            CaseDetail.UserCaseId = id;
                        }
                        else
                            ViewBag.Title = "Update";
                        CaseDetail.ClientList = await DdlClient(CurrentUser.Id);
                        CaseDetail.States = await LoadStates();
                        CaseDetail.CourtTypes = await LoadCourtTypes();
                        CaseDetail.CaseNatures = await LoadCaseNatureByCourtType(CaseDetail.CourtTypeId); ;
                        CaseDetail.TypeOfCases = await CaseTypes(CaseDetail.CaseCategoryId);
                        CaseDetail.Years = DdlYears();
                        CaseDetail.FirstTitleList = await DdlFSTypes(1);
                        CaseDetail.SecondTitleList = await DdlFSTypes(2);
                        CaseDetail.CaseStatusList = await DdlCaseStages();
                        CaseDetail.LinkedBy = await UserCaseTitle(id);
                        CaseDetail.Appearences = await DdlFSTypes(0);
                        if (CaseDetail.IsHighCourt == true)
                        {
                            CaseDetail.Courts = await LoadBenches(CaseDetail.CourtTypeId, CaseDetail.StateId, Guid.Empty, Guid.Empty);
                            CaseDetail.Strengths = DdlStrength();
                            showHighCourt = true;
                        }
                        else
                        {
                            CaseDetail.CourtDistricts = await DdlLoadCourtDistricts(CaseDetail.StateId);
                            CaseDetail.ComplexBenchs = await GetCourtComplex(CaseDetail.CourtDistrictId.Value);
                            CaseDetail.Courts = await LoadBenches(CaseDetail.CourtTypeId, CaseDetail.StateId, CaseDetail.ComplexId.Value, CaseDetail.CourtDistrictId.Value);
                        }
                        if (CaseDetail.AgainstCaseDetails.Count > 0)
                        {
                            foreach (var item in CaseDetail.AgainstCaseDetails)
                            {
                                item.CaseId = id;
                                cam.StateId = item.StateId;
                                cam.ImpugedOrderDate = item.ImpugedOrderDate;
                                cam.CourtTypeId = item.CourtTypeId;
                                if (item.IsAgHighCourt == true)
                                {
                                    cam.ACourts = await LoadBenches(item.CourtTypeId.Value, item.StateId.Value, Guid.Empty, Guid.Empty);
                                    cam.AStrengths = DdlStrength();
                                    AgIsHighCourt = true;
                                    cam.BenchId = item.BenchId;
                                }
                                else
                                {
                                    cam.CourtDistrictId = item.CourtDistrictId;
                                    cam.ComplexId = item.ComplexId;
                                    cam.CourtId = item.CourtId;
                                    cam.ACourtDistricts = await DdlLoadCourtDistricts(item.StateId.Value);
                                    cam.AComplexBenchs = await GetCourtComplex(item.CourtDistrictId.Value);
                                    cam.ACourts = await LoadBenches(item.CourtTypeId.Value, item.StateId.Value, item.ComplexId.Value, item.CourtDistrictId.Value);
                                }
                                cam.CaseCategoryId = item.CaseCategoryId;
                                cam.CaseTypeId = item.CaseTypeId;
                                cam.CaseNo = item.CaseNo;
                                cam.CaseYear = item.CaseYear;
                                cam.CisNo = item.CisNo;
                                cam.CisYear = item.CisYear;
                                cam.CnrNo = item.CnrNo;
                                cam.OfficerName = item.OfficerName;
                                cam.CadreId = item.CadreId;
                                cam.ACaseNatures = await LoadCaseNatureByCourtType(item.CourtTypeId.Value);
                                cam.ATypeOfCases = await CaseTypes(item.CaseCategoryId.Value);
                            }
                        }
                        var agcl = new List<CaseAgainstModel>();
                        agcl.Add(cam);
                        CaseDetail.AgainstCaseDetails = agcl;
                        ViewBag.ShowHighCourt = showHighCourt;
                        ViewBag.AgIsHighCourt = AgIsHighCourt;
                        CaseDetail.Cadres = await DdlCadres();
                        if (from != "repeat" && from != "cp")
                        {
                            ViewBag.ActionType = "Update";
                            ViewBag.Id = CaseDetail.Id.ToString();
                        }
                        else
                        {
                            _logger.LogInformation("Case form is access by Repeat");
                            ViewBag.ActionType = "Save";
                            ViewBag.Title = "Add New";
                            ViewBag.Id = Guid.Empty.ToString();
                            CaseDetail.Id = Guid.Empty;
                            CaseDetail.LinkedCaseId = Guid.Empty;
                            ViewBag.from = from;
                        }
                        return View("_CreateOrEdit", CaseDetail);
                    }
                    ViewBag.ShowHighCourt = showHighCourt;
                    ViewBag.AgIsHighCourt = AgIsHighCourt;
                    caseViewModel.ClientList = await DdlClient(CurrentUser.Id);
                    caseViewModel.InstitutionDate = DateTime.Now;
                    return View("_CreateOrEdit", caseViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateOrEdit(Guid Id, CaseUpseartViewModel ViewModel)
        {
            bool showHighCourt = false;
            bool AgIsHighCourt = false;
            if (ModelState.IsValid)
            {
                if (Id == Guid.Empty)
                {
                    var createCommand = _mapper.Map<CreateCaseCommand>(ViewModel);
                    createCommand.LinkedIds = User.GetUserLinkedIds();
                    createCommand.AgainstCaseDetails = _mapper.Map<List<UpseartAgainstCaseDto>>(ViewModel.AgainstCaseDetails);
                    var result = await _mediator.Send(createCommand);
                    if (result.Succeeded)
                    {
                        ViewModel.StatusMessage = "Case information created successfully!";
                        _notify.Success($"Case created with ID {result.Data} Created.");
                    }
                    else
                    {
                        _notify.Error(result.Message);
                        ViewBag.from = "repeat";
                        if (ViewModel.BenchId != null)
                            showHighCourt = true;
                        if (ViewModel.AgainstCaseDetails[0].BenchId != null)
                        {
                            AgIsHighCourt = true;
                        }
                    }
                    ViewBag.ShowHighCourt = showHighCourt;
                    ViewBag.AgIsHighCourt = AgIsHighCourt;
                    return RedirectToAction("CreateOrUpdate", "CaseManage", new { area = "Litigation" });
                }
                else
                {
                    var updateCommand = _mapper.Map<UpdateCaseDetailCommand>(ViewModel);
                    updateCommand.AgainstCaseDetails = _mapper.Map<List<UpseartAgainstCaseDto>>(ViewModel.AgainstCaseDetails);
                    var result = await _mediator.Send(updateCommand);
                    if (result.Succeeded)
                    {
                        _notify.Information($"Case information with ID {result.Data} Updated.");
                        return RedirectToAction("getcasedetail", new { id = Id, reft = TempData["referenceType"] });
                    }
                }
            }

            ViewModel.InstitutionDate = DateTime.Now;
            ViewModel.CaseNatures = await LoadCaseNature();
            ViewModel.CaseKinds = await LoadCaseKinds();
            ViewModel.CourtTypes = await LoadCourtTypes();
            ViewModel.CaseStages = await DdlCaseStages();
            ViewModel.FirstTitleList = FirstTtitleList();
            ViewModel.SecondTitleList = SecondTtitleList();
            ViewModel.Years = DdlYears();
            ViewModel.CaseStatusList = DdlCaseStatus();
            ViewModel.LinkedBy = DdlClient(CurrentUser.Id).Result;
            ViewBag.ShowHighCourt = showHighCourt;
            ViewBag.AgIsHighCourt = AgIsHighCourt;
            var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ViewModel);
            return new JsonResult(new { isValid = false, html = html });
        }

        [HttpPost]
        public async Task<JsonResult> OnPostDelete(Guid id)
        {
            var deleteCommand = await _mediator.Send(new DeleteCaseDetailCommand { Id = id });
            if (deleteCommand.Succeeded)
            {
                _notify.Information($"Case with Id {id} is deleted.");
                var response = await _mediator.Send(new GetCaseInfoQuery()
                {
                    LinkedIds = User.GetUserLinkedIds(),
                    PageSize = 10000,
                    PageNumber = 1
                });
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<GetCaseInfoViewModel>>(response.Data);
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
        #endregion

        #region Case Proceeding Area    
        public async Task<IActionResult> LoadCaseProceeding()
        {
            var response = await _mediator.Send(new GetCaseProceedingQuery());
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<GetCaseViewModel>>(response.Data);
                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }
        #endregion

        #region Case History
        public async Task<JsonResult> OnGetCaseHistory(Guid CaseId)
        {
            var response = await _mediator.Send(new GetCaseHistoryQuery() { CaseId = CaseId });
            if (response.Succeeded)
            {
                var docs = response.Data.Docs;
                List<CaseDoc> UDocs = new List<CaseDoc>();
                foreach (var item in docs)
                {
                    UDocs.Add(new CaseDoc
                    {
                        DocFilePath = item.DocFilePath,
                        DocName = item.DocName,
                        DocType = item.DocType,
                        DocDate = item.DocDate
                    });
                }
                var model = _mapper.Map<CaseHistoryViewModel>(response.Data);
                var ProcDts = response.Data.History
                    .Select(s => s.Date).Distinct().OrderByDescending(o => o.Date);
                var ProcHis = new List<ProcHistory>();
                foreach (var h in ProcDts)
                {
                    var ph = new ProcHistory();
                    ph.Date = h.Date.ToString("dd/MM/yyyy");
                    var Hisdt = response.Data.History.Where(w => w.Date == h.Date);
                    var ProcWiseHis = new List<HistoryDetail>();
                    foreach (var hd in Hisdt)
                    {
                        var his = new HistoryDetail();
                        his.Activity = hd.Activity;
                        his.NextDate = hd.NextDate;
                        his.Date = hd.Date.ToString("dd/MM/yyyy");
                        his.Stage = hd.Stage;
                        his.Type = hd.Type;
                        his.WorkDetail = _mapper.Map<List<Models.CaseWorkDetail>>(hd.WorkDetail);
                        ProcWiseHis.Add(his);
                        ph.History = ProcWiseHis;
                    }
                    ProcHis.Add(ph);
                }
                model.Docs = UDocs;
                model.ProcHis = ProcHis;
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CaseHistory", model) });
            }
            return null;
        }
        #endregion

        #region Document Upload 
        public async Task<IActionResult> GetFileUploadModel(Guid CaseId, string w, string reft)
        {
            var response = await _mediator.Send(new GetCaseHistoryQuery() { CaseId = CaseId });
            List<CaseDoc> UDocs = new List<CaseDoc>();
            var model = new CaseAttacheDocumentViewModel();
            if (response.Succeeded)
            {
                var docs = response.Data.Docs;
                var CaseInfo = response.Data;
                foreach (var item in docs)
                {
                    //string ext = item.DocFilePath.Split(".")[1];
                    //string Icon = "";
                    //if (ext == "doc" || ext == "docx") Icon = "fa fa-file-word-o";
                    //else Icon = "fa fa-file-pdf";
                    UDocs.Add(new CaseDoc
                    {
                        //DocFilePath = item.DocFilePath,
                        DocFilePath = item.DocFilePath,
                        DocName = item.DocName,
                        DocType = item.DocType,
                        DocDate = item.DocDate,
                        Id = item.Id,
                        FIcon = "fa-solid fa-file-zip",
                        Reference = reft
                    });
                }

                model.CaseId = CaseId;
                model.DocTypes = DOTypes();
                model.Docs = UDocs;
                model.CaseNoYear = CaseInfo.CaseNoYear;
                model.Title = CaseInfo.Title;
                model.Court = CaseInfo.Court;
                model.Reference = reft;
                model.Where = w;
            }
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_UploadCaseDoc", model) });
        }

        public async Task<IActionResult> GetUpdatedDocs(Guid caseId, string reft)
        {
            var response = await _mediator.Send(new CaseDocumentQuery() { CaseId = caseId });
            if (response.Succeeded)
            {

                var caseDocs = response.Data.Select(s => new CaseDoc
                {
                    Reference = reft,
                    DocDate = s.DocDate,
                    DocFilePath = s.DocFilePath,
                    DocName = s.DocName,
                    DocType = s.DocType,
                    FIcon = "fa-solid fa-file-zip",
                    //FIcon = s.DocFilePath.Split("_")[1].Contains("doc") ? "fa fa-file-word-o" : "fa fa-file-pdf",
                    Id = s.Id
                }).ToList();
                return PartialView("_CaseDocuments", caseDocs);
            }
            return null;
        }

        [RequestSizeLimit(MaxFileSize)]
        public async Task<IActionResult> UploadCaseDocs(CaseAttacheDocumentViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = "failed", message = "Invalid request data, uplaoded file size may be > 30MB or some information is missin! " });

            List<CaseDocumentModel> ddoc = new List<CaseDocumentModel>();
            if (model.Documents.Count > 0)
            {
                foreach (var f in model.Documents)
                {
                    // Step 1: Validate File
                    var fileExtension = Path.GetExtension(f.Document.FileName).ToLower();
                    if (fileExtension != ".pdf" && fileExtension != ".docx" && fileExtension != ".doc")
                        return Json(new { success = "failed", message = "Only PDF, doc and DOCX are allowed." });

                    if (f.Document.Length > MaxFileSize)
                        return Json(new { success = "failed", message = "Uploaded document has exeed size(>30mb), please compress and upload it again " + f.Document.FileName });

                    try
                    {
                        // Step 2: Generate Unique File Name
                        string uniqueFileName = $"{Path.GetFileNameWithoutExtension(f.Document.FileName)}_{Guid.NewGuid()}{Path.GetExtension(f.Document.FileName)}";
                        string documentType = f.TypeId == 1 ? "Draft" : "Order";

                        // Step 3: Compress the File into a ZIP
                        using var docStream = f.Document.OpenReadStream();
                        using var memoryStream = new MemoryStream();
                        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            var zipEntry = zipArchive.CreateEntry(f.Document.FileName);
                            using var entryStream = zipEntry.Open();
                            await docStream.CopyToAsync(entryStream);
                        }
                        // Step 4: Prepare the compressed stream for upload
                        memoryStream.Seek(0, SeekOrigin.Begin);  // Reset the memory stream position to the start

                        // Step 5: Upload the compressed file to Azure Blob Storage
                        string compressedFileName = $"{Path.GetFileNameWithoutExtension(f.Document.FileName)}_{Guid.NewGuid()}.zip";
                        string filePath = await _documentUploadService.UploadFileAsync(memoryStream, compressedFileName, documentType);
                        //string filePath = await _blobService.UploadOrUpdateFileAsync(memoryStream, compressedFileName, "application/zip", containerName, CancellationToken.None);

                        // Step 6: Map Data for Database Entry
                        ddoc.Add(new CaseDocumentModel
                        {
                            DocId = f.DocId,
                            TypeId = f.TypeId,
                            DocPath = filePath,  // Cloud file URL
                            DocDate = f.DocDate
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error uploading file {f.Document.FileName}: {ex.Message}");
                        return StatusCode(500, "Internal Server Error while processing the file.");
                    }
                }
            }
            // Step 7: Map and Save to Database
            var docMapper = _mapper.Map<List<DocumentAttachmentModel>>(ddoc);
            var response = await _mediator.Send(new CaseDocsCreateCommand()
            {
                CaseId = model.CaseId,
                Documents = docMapper
            });
            if (response.Succeeded)
            {
                if (model.Where == "mc")
                    return Json(new { success = response.Succeeded, caseId = model.CaseId, caseType = model.Reference });
                //return RedirectToAction("Index");
                else
                    return RedirectToAction("institionregister", "register", new { area = "report" });
            }
            return new JsonResult(new { isValid = false, message = "Failed to process the request." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoc(Guid id, string fPath)
        {
            var respose = await _mediator.Send(new DeleteCaseDocumentCommand()
            {
                DocId = id
            });
            if (respose.Succeeded)
            {
                var isDeleted = await _documentUploadService.DeleteFileAsync(fPath);
                if (isDeleted)
                    return Json(new { success = true });
            }
            return Json(new { success = respose.Failed, respose.Message });

        }


        [HttpPost]
        [RequestSizeLimit(MaxFileSize)]
        public async Task<IActionResult> ReplaceDocument(Guid docId, string fileId, string documentType, IFormFile newDocument)
        {
            if (newDocument == null || newDocument.Length == 0)
                return Json(new { success = false, message = "No document provided." });

            var fileExtension = Path.GetExtension(newDocument.FileName).ToLower();
            if (fileExtension != ".pdf" && (fileExtension != ".docx" || fileExtension != ".doc"))
                return Json(new { success = false, message = "Only PDF, doc and DOCX are allowed." });

            if (newDocument.Length > MaxFileSize)
                return Json(new { success = false, message = "File size exceeds 30MB." });

            try
            {
                //step 1: Delete existing document from cloud
                var isDeleted = await _documentUploadService.DeleteFileAsync(fileId);
                if (!isDeleted)
                    return Json(new { success = false, message = "The file is not exist for replace!" });

                //Step 2: Generate the unique file Id and compressed newly file.
                string compressedFileName = $"{Path.GetFileNameWithoutExtension(newDocument.FileName)}_{Guid.NewGuid()}.zip";
                using var docStream = newDocument.OpenReadStream();
                using var memoryStream = new MemoryStream();
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var zipEntry = zipArchive.CreateEntry(newDocument.FileName);
                    using var entryStream = zipEntry.Open();
                    await docStream.CopyToAsync(entryStream);
                }
                memoryStream.Seek(0, SeekOrigin.Begin);

                //Step 3: Upload the new file 
                string filePath = await _documentUploadService.UploadFileAsync(memoryStream, compressedFileName, documentType);

                //Step 4: Update the file path in the database
                var dbUpdateResponse = await _mediator.Send(new UpdateCaseDocumentCommand
                {
                    Id = docId,
                    DocId = fileId
                });
                if (dbUpdateResponse.Succeeded)
                    return Json(new { success = true, message = "Document replaced successfully." });

                return Json(new { success = false, message = "Failed to update document information." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error replacing document: {ex.Message}");
                return StatusCode(500, "Internal Server Error while replacing the file.");
            }
        }

        #endregion

        #region Case Detail
        public async Task<IActionResult> GetCaseDetailAsync(Guid id, string reft)
        {
            var response = await _mediator.Send(new GetCaseDetailInfoQuery() { CaseId = id });
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<CaseDetailInfoViewModel>(response.Data);
                viewModel.Reference = reft;
                TempData["referenceType"] = reft;
                return View(viewModel);
            }
            return null;
        }
        #endregion

        #region Case and Client 
        public async Task<IActionResult> OnCreateClientInfoAsync()
        {
            var ViewModel = new ClientViewModel();
            //ViewModel.OppositCounsels = await DdlLawyerAsync();
            //ViewModel.Appearences = await DdlFSTypes(0);
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateCaseClient", ViewModel) });
        }
        public async Task<JsonResult> OnGetClientInfo(Guid CaseId)
        {
            var ViewModel = new ClientViewModel();
            ViewModel.CaseId = CaseId;
            //ViewModel.OppositCounsels = await DdlLawyerAsync();
            //ViewModel.Appearences = await DdlFSTypes(0);
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_ClientInfoDetail", ViewModel) });
        }
        [HttpPost]
        public async Task<IActionResult> OnPostClientInfo(Guid id, ClientViewModel btViewModel)
        {
            if (ModelState.IsValid)
            {
                if (id == Guid.Empty)
                {
                    var createClientCommand = _mapper.Map<CreateClientCommand>(btViewModel);
                    var result = await _mediator.Send(createClientCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        TempData["ClientId"] = id;
                        //var updateClientInfo = await _mediator.Send(new CaseClientInfoUpdateCommand()
                        //{
                        //    ClientId = id,
                        //    CaseId = btViewModel.CaseId,
                        //});
                        _notify.Success($"Client with ID {result.Data} Created.");
                    }
                    else _notify.Error(result.Message);
                    return RedirectToAction("CreateOrUpdate");
                }
                return null;
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", btViewModel);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
        #endregion

        #region Selected Lawyer case detail
        public async Task<IActionResult> BindLawyerCaseDetail(Guid id)
        {
            bool showHighCourt = false;
            bool AgIsHighCourt = false;
            ViewBag.Title = "Add New";
            if (ModelState.IsValid)
            {
                var response = await _mediator.Send(new GetUserCaseDetailByIdQuery
                {
                    CaseId = id,
                    LinkedIds = User.GetUserLinkedIds()
                });
                if (response.Succeeded)
                {
                    _notify.Success($"Client with ID {response.Data} Created.");
                    var CaseDetail = _mapper.Map<CaseUpseartViewModel>(response.Data);
                    //Set the primary case detail to against in upper case
                    var agnstVM = new CaseAgainstModel();
                    var agnstVML = new List<CaseAgainstModel>();
                    agnstVM.ImpugedOrderDate = CaseDetail.DisposalDate != null ? CaseDetail.DisposalDate : null;
                    agnstVM.StateId = CaseDetail.StateId;
                    agnstVM.CourtTypeId = CaseDetail.CourtTypeId;
                    agnstVM.CourtDistrictId = CaseDetail.CourtDistrictId;
                    agnstVM.ComplexId = CaseDetail.ComplexId;
                    agnstVM.CourtId = CaseDetail.CourtId;
                    agnstVM.CaseCategoryId = CaseDetail.CaseCategoryId;
                    agnstVM.CaseTypeId = CaseDetail.CaseTypeId;
                    agnstVM.CaseNo = CaseDetail.CaseNo;
                    agnstVM.CaseYear = CaseDetail.CaseYear;
                    agnstVM.CisNo = CaseDetail.CisNumber;
                    agnstVM.CisYear = CaseDetail.CisYear;
                    agnstVM.CnrNo = CaseDetail.CnrNumber;
                    if (CaseDetail.IsHighCourt == true)
                    {
                        agnstVM.ACourts = await LoadBenches(CaseDetail.CourtTypeId, CaseDetail.StateId, Guid.Empty, Guid.Empty);
                        agnstVM.AStrengths = DdlStrength();
                        showHighCourt = true;
                    }
                    else
                    {
                        agnstVM.ACourtDistricts = await DdlLoadCourtDistricts(CaseDetail.StateId);
                        agnstVM.AComplexBenchs = await GetCourtComplex(CaseDetail.CourtDistrictId.Value);
                        agnstVM.ACourts = await LoadBenches(CaseDetail.CourtTypeId, CaseDetail.StateId, CaseDetail.ComplexId.Value, CaseDetail.CourtDistrictId.Value);
                    }
                    agnstVM.AStates = await LoadStates();
                    agnstVM.ACourtTypes = await LoadCourtTypes();
                    agnstVM.AYears = DdlYears();
                    agnstVM.ACaseStatusList = await DdlCaseStages();
                    agnstVM.ALinkedBy = await UserCaseTitle(Guid.Empty);
                    agnstVM.ACadres = await DdlCadres();
                    agnstVM.ACaseNatures = await LoadCaseNatureByCourtType(CaseDetail.CourtTypeId);
                    agnstVM.ATypeOfCases = await CaseTypes(CaseDetail.CaseCategoryId);
                    agnstVML.Add(agnstVM);
                    CaseDetail.AgainstCaseDetails = agnstVML;
                    ViewBag.ShowHighCourt = showHighCourt;
                    ViewBag.AgIsHighCourt = AgIsHighCourt;
                    ViewBag.ActionType = "Save";
                    //Reset the main case variable. 
                    CaseDetail.ClientList = await DdlClient(CurrentUser.Id);
                    CaseDetail.Appearences = await DdlFSTypes(0);
                    CaseDetail.States = await LoadStates();
                    CaseDetail.CourtTypes = await LoadCourtTypes();
                    CaseDetail.CaseNatures = await LoadCaseNatureByCourtType(CaseDetail.CourtTypeId);
                    CaseDetail.TypeOfCases = await CaseTypes(CaseDetail.CaseCategoryId);
                    CaseDetail.Years = DdlYears();
                    CaseDetail.FirstTitleList = await DdlFSTypes(1);
                    CaseDetail.SecondTitleList = await DdlFSTypes(2);
                    CaseDetail.CaseStatusList = await DdlCaseStages();
                    CaseDetail.LinkedBy = await UserCaseTitle(id);
                    CaseDetail.Cadres = await DdlCadres();
                    CaseDetail.Strengths = DdlStrength();
                    CaseDetail.Id = Guid.Empty;
                    CaseDetail.LCaseId = id;
                    CaseDetail.InstitutionDate = DateTime.Now;
                    CaseDetail.StateId = 0;
                    CaseDetail.CourtTypeId = Guid.Empty;
                    CaseDetail.CourtDistrictId = Guid.Empty;
                    CaseDetail.ComplexId = Guid.Empty;
                    CaseDetail.CourtId = Guid.Empty;
                    CaseDetail.CaseCategoryId = Guid.Empty;
                    CaseDetail.CaseTypeId = Guid.Empty;
                    CaseDetail.CaseStageId = Guid.Empty;
                    CaseDetail.CaseNo = string.Empty;
                    CaseDetail.CaseYear = 0;
                    CaseDetail.CisNumber = string.Empty;
                    CaseDetail.CisYear = 0;
                    CaseDetail.CnrNumber = string.Empty;
                    CaseDetail.NextDate = null;
                    return View("_CreateOrEdit", CaseDetail);
                }

            }
            return null;
        }
        #endregion

        #region Without Next Date Case List
        public async Task<IActionResult> GetCaseWoh()
        {
            var response = await _mediator.Send(new GetCaseWohDateQuery()
            {
                LinkedIds = User.GetUserLinkedIds(),
                PageSize = 10000,
                PageNumber = 1
            });
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<GetCaseInfoViewModel>>(response.Data);
                return View("_WohDateCases", viewModel);
            }
            return null;
        }
        [HttpPost]
        public async Task<JsonResult> UpdateHearingDate(List<UpdateHearingDtViewModel> casedts)
        {
            var response = await _mediator.Send(new UpdateCaseHearingDatesCommand()
            {
                UserId=CurrentUser.Id,
                CasesHearingDt = _mapper.Map<List<CaseHearingDto>>(casedts)
            });
            return Json(response);
        }
        #endregion

        #region Assign Case To Other Lawyer
        public async Task<JsonResult> OnGetAssignCase(Guid CaseId)
        {
            var model = new AssignCaseViewModel();
            model.IsAssignAction = true;
            var userType = new List<string>();
            userType.Add("LAWYER");
            userType.Add("CORPORATE");
            var allUsersExceptCurrentUser = await _userManager.Users
                .Where(a => !User.GetUserLinkedIds().Contains(a.Id)
                            && a.IsActive == true
                            && userType.Contains(a.UserType)).ToListAsync();
            var modelUser = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
            var lawyerSelectList = modelUser.Select(x => new
            {
                Id = x.Id,
                FullDisplay = $"{x.FirstName} {x.LastName}"
            });
            model.Lawyers = new SelectList(lawyerSelectList, "Id", "FullDisplay");
            model.Id = CaseId;
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignCase", model) });
        }

        [HttpPost]
        public async Task<JsonResult> OnPostAssignCase(Guid Id, AssignCaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsAssignAction == true)
                {
                    try
                    {
                        var cmd = _mapper.Map<CreateCaseAssignedCommand>(model);
                        cmd.UserId = Guid.Parse(CurrentUser.Id);
                        cmd.CaseId = Id;
                        var result = await _mediator.Send(cmd);
                        if (result.Succeeded)
                        {
                            Id = result.Data;
                            _notify.Success($"Case is assigned to the selected lawyer successfully!");
                            return new JsonResult(new { isValid = true, html = "" });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message.ToString());
                        Console.WriteLine(ex);
                    }
                }
                else
                {
                    var cmd = _mapper.Map<CaseDeAssignedCommnad>(model);
                    var result = await _mediator.Send(cmd);
                    if (result.Succeeded)
                    {
                        Id = result.Data;
                        _notify.Success($"Case is de-assigned successfully!");
                        return new JsonResult(new { isValid = true, html = "" });
                    }
                }
            }
            return new JsonResult(new { isValid = false, html = "html" });
        }


        public async Task<JsonResult> DeAssignedCase(Guid CaseId, Guid LawyerId)
        {
            var model = new AssignCaseViewModel();
            model.IsAssignAction = false;
            model.Id = CaseId;
            model.LawyerId = LawyerId;
            var lawyerInfo = await _userManager.Users
                .Where(a => a.IsActive == true
                            && a.Id == LawyerId.ToString()).FirstOrDefaultAsync();
            string lawyerFullName = lawyerInfo.FirstName + " " + lawyerInfo.LastName;
            model.LawyerInfo = lawyerFullName;
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_AssignCase", model) });
        }
        #endregion

        #region Load searchable officer's name 
        public async Task<JsonResult> GetOfficerName(Guid CourtId, string officerName)
        {
            var result = await _mediator.Send(new GetOfficerQuery { CourtId = CourtId, OfficerName = officerName });
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

        #region Case DropDown By Lawyer
        public async Task<JsonResult> ddlCaseInfoByLawyer(string UserId)
        {
            try
            {
                Console.WriteLine("UserId" + UserId);
                List<string> LinkedIds = new List<string>();
                LinkedIds = await _identityDbContext.LawyerUsers
                            .Where(w => w.LawyerId == UserId)
                            .Select(s => s.Id.ToString())
                            .ToListAsync();
                LinkedIds.Add(UserId);
                Console.WriteLine("LinkedUserId" + string.Join(",", LinkedIds));
                var response = await _mediator.Send(new GetCasesByUserQuery()
                {
                    LinkIds = LinkedIds,
                    CallingFrom = "Bring"
                });
                if (response.Succeeded)
                {
                    Console.WriteLine("After success" + string.Join(",", LinkedIds));
                    var dt = response.Data;
                    var ViewModel = _mapper.Map<List<DropDownGViewModel>>(dt);
                    Console.WriteLine("After success" + string.Join(",", LinkedIds));
                    return Json(ViewModel);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        #region Copy Case Detail
        public async Task<IActionResult> CopyCaseDetail()
        {
            CopyCaseViewModel model = new CopyCaseViewModel();
            model.Cases = await UserCaseTitle(Guid.Empty);
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CopyCaseInfo", model) });
        }

        [HttpPost]
        public IActionResult CopyCaseDetail(CopyCaseViewModel model)
        {
            // Redirect to the target action with id and from=cp
            return RedirectToAction("CreateOrUpdate", new { id = model.CaseId, from = "cp" });
        }
        #endregion

    }
}
