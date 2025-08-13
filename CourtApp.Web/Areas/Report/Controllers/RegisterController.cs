using CourtApp.Application.Constants;
using CourtApp.Application.Features.CaseWork;
using CourtApp.Application.Features.Registers;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Report.Models;
using CourtApp.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Litigation.Controllers
{
    [Area("Report")]
    public class RegisterController : BaseController<RegisterController>
    {
        public IActionResult index()
        {
            return View();
        }
        public async Task<IActionResult> LoadAll(string rt, string f, string to)
        {
            var viewName = ""; dynamic md = null;
            DateTime fromDt; DateTime toDt;
            if (string.IsNullOrEmpty(f)) fromDt = DateTime.Today.AddDays(-7);
            else fromDt = Convert.ToDateTime(f).Date;

            if (string.IsNullOrEmpty(to)) toDt = DateTime.Today;
            else toDt = Convert.ToDateTime(f).Date;

            switch (rt)
            {
                case "dis":
                    break;

                case "copy":
                    break;
                case "other":
                    break;
                default:
                    var response = await _mediator.Send(new InstitutionRegisterQuery()
                    {
                        PageNumber = 1,
                        PageSize = 10000,
                        FromDt = fromDt,
                        ToDt = toDt,
                        LinkedIds = User.GetUserLinkedIds()
                        //UserId = CurrentUser.Id
                    });
                    InstitutionRegisterViewMode model = new InstitutionRegisterViewMode();
                    List<InstituteModel> inmd = new List<InstituteModel>();
                    if (response != null && response.Succeeded)
                    {
                        foreach (var d in response.Data)
                        {
                            InstituteModel rd = new InstituteModel();
                            rd.Id = d.Id;
                            rd.Court = d.Court;
                            rd.CaseType = d.CaseType;
                            rd.Year = d.Year;
                            rd.No = d.No;
                            rd.FirstTitle = d.FirstTitle;
                            rd.SecondTitle = d.SecondTitle;
                            rd.InsititutionDate = d.InsititutionDate;
                            inmd.Add(rd);
                        }
                    }
                    model.dtmodel = inmd;
                    viewName = "_Instition";
                    md = model;
                    break;
            }
            return PartialView(viewName, md);
        }

        public async Task<IActionResult> Search(Guid ClientId, string ReferalBy, string Status)
        {
            var viewName = ""; dynamic md = null;
            var response = await _mediator.Send(new CaseSearchableDataQuery()
            {
                LinkedIds = User.GetUserLinkedIds(),
                ClientId = ClientId,
                ReferalBy = ReferalBy,
                Status = Status,
                PageNumber = 1,
                PageSize = 10000,
            });
            InstitutionRegisterViewMode model = new InstitutionRegisterViewMode();
            List<InstituteModel> inmd = new List<InstituteModel>();
            if (response != null && response.Succeeded)
            {
                foreach (var d in response.Data)
                {
                    InstituteModel rd = new InstituteModel();
                    rd.IsCaseAssigned = d.IsCaseAssigned;
                    rd.LawyerId = d.LawyerId;
                    rd.Reference = d.Reference;
                    rd.Id = d.Id;
                    rd.Court = d.Court;
                    rd.CaseType = d.CaseType;
                    rd.Year = d.Year;
                    rd.No = d.No;
                    rd.FirstTitle = d.FirstTitle;
                    rd.SecondTitle = d.SecondTitle;
                    rd.InsititutionDate = d.InsititutionDate;
                    inmd.Add(rd);
                }
                model.dtmodel = inmd;
                viewName = "_Instition";
                md = model;
            }
            return PartialView(viewName, md);
        }

        #region Disposal Register
        public IActionResult DisposalRegister()
        {
            return View();
        }
        public async Task<IActionResult> LoadDisposalData()
        {
            var response = await _mediator.Send(new DisposalRegisterQuery()
            {
                PageNumber = 1,
                PageSize = 10000,
                FromDt = Convert.ToDateTime("2024-05-01"),
                ToDt = DateTime.Now,
                LinkedIds = User.GetUserLinkedIds(),
            });
            if (response.Succeeded)
            {
                var model = _mapper.Map<List<DisposalRegisterViewModel>>(response.Data);
                return PartialView("_DisposalRegister", model);
            }
            return null;
        }

        #endregion

        #region Copying Register & Its Recieving Status

        public IActionResult CopyingRegister()
        {
            CopyingRegisterViewModel model = new CopyingRegisterViewModel();
            model.Filters = new SelectList(StaticDropDownDictionaries.CopyingFilter(), "Key", "Value");
            return View(model);
        }

        public async Task<IActionResult> LoadCopyingData(int s)
        {
            var response = await _mediator.Send(new CopyingRegisterQuery()
            {
                PageNumber = 1,
                PageSize = 10000,
                FromDt = Convert.ToDateTime("2024-05-01"),
                ToDt = DateTime.Now,
                SearchType = s,
                LinkedIds = User.GetUserLinkedIds(),
            });
            CopyingRegisterViewModel model = new CopyingRegisterViewModel();
            List<CopyingCaseDetailModel> rdd = new List<CopyingCaseDetailModel>();
            if (response != null && response.Succeeded)
            {
                foreach (var d in response.Data)
                {
                    CopyingCaseDetailModel rd = new CopyingCaseDetailModel();
                    rd.Id = d.Id;
                    rd.AppliedOn = d.AppliedOn;
                    rd.ReceivedOn = d.ReceivedOn;
                    rd.FirstTitle = d.FirstTitle;
                    rd.SecondTitle = d.SecondTitle;
                    rd.Court = d.Court;
                    rd.CaseType = d.CaseType;
                    rd.No = d.No;
                    rd.Year = d.Year = d.Year == "0" ? "" : d.Year; ;
                    rdd.Add(rd);
                }
                model.copyingCases = rdd;
                return PartialView("_CopyingRegister", model);
            }
            model.copyingCases = rdd;
            return PartialView("_CopyingRegister", model);
        }

        public async Task<IActionResult> UpdateCopyingStatus(CopyingRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.copyingCases != null)
                {
                    var CopyingCaseId = model.copyingCases.Where(w => w.Selected == true)
                        .Select(s => s.Id).ToList();
                    if (CopyingCaseId != null)
                    {
                        var result = await _mediator.Send(new UpdateCopyingStatusCommand
                        { CaseId = CopyingCaseId, Status = 2 });
                        if (result.Succeeded) _notify.Information($"Case copying receive status update successfull!");
                        else _notify.Error(result.Message);
                    }
                }
            }
            return RedirectToAction("CopyingRegister");
        }
        #endregion

        #region Institution Register
        public async Task<IActionResult> InstitionRegister()
        {
            var response = await _mediator.Send(new InstitutionRegisterQuery()
            {
                PageNumber = 1,
                PageSize = 10000,
                FromDt = Convert.ToDateTime("2000-01-01"),
                ToDt = DateTime.Now,
                LinkedIds = User.GetUserLinkedIds()
            });
            InstitutionRegisterViewMode model = new InstitutionRegisterViewMode();
            List<InstituteModel> inmd = new List<InstituteModel>();
            if (response != null && response.Succeeded)
            {
                foreach (var d in response.Data)
                {
                    InstituteModel rd = new InstituteModel();
                    rd.IsCaseAssigned = d.IsCaseAssigned;
                    rd.Reference = d.Reference.ToUpper();
                    rd.LawyerId = d.LawyerId;
                    rd.Id = d.Id;
                    rd.Court = d.Court.ToUpper();
                    rd.CaseType = d.CaseType.ToUpper();
                    rd.Year = d.Year == "0" ? "" : d.Year;
                    rd.No = d.No;
                    rd.FirstTitle = d.FirstTitle.ToUpper();
                    rd.SecondTitle = d.SecondTitle.ToUpper();
                    rd.InsititutionDate = d.InsititutionDate;
                    inmd.Add(rd);
                }
            }
            model.dtmodel = inmd;
            return View(model);
        }
        #endregion

        #region Other Register
        public IActionResult OtherRegister()
        {
            return View();
        }
        public async Task<IActionResult> LoadOtherData()
        {
            var response = await _mediator.Send(new OtherRegisterQuery()
            {
                PageNumber = 1,
                PageSize = 10000,
                FromDt = Convert.ToDateTime("2024-05-01"),
                ToDt = DateTime.Now,
                LinkedIds = User.GetUserLinkedIds(),
            });
            if (response.Succeeded)
            {
                var model = _mapper.Map<List<OtherRegisterViewModel>>(response.Data);
                return PartialView("_OtherRegister", model);
            }
            var vm = new List<OtherRegisterViewModel>();
            return PartialView("_OtherRegister", vm);
        }
        #endregion
    }

}
