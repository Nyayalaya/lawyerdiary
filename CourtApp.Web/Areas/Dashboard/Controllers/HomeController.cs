using CourtApp.Application.Features.CaseDetails;
using CourtApp.Application.Features.Dashboard;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Dashboard.Models;
using CourtApp.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomeController : BaseController<HomeController>
    {
        public async Task<IActionResult> Index()
        {
            _notify.Information("Hi There!");

            var response = await _mediator.Send(new DashboardQueryCommand
            {
                LinkedIds = User.GetUserLinkedIds()
            });

            if (response.Succeeded && response.Data != null)
            {
                var dto = response.Data;

                var model = new DashboardViewModel
                {
                    TotalCases = dto.TotalCases,
                    DisposedCases = dto.DisposedCases,
                    PendingCases = dto.PendingCases,
                    AssignedCases = dto.AssignedCases,
                    TodayHearing=dto.TodayHearing,
                    StatusSummaries = dto.StatusSummaries.Select(s => new CaseStatusSummary
                    {
                        Status = s.Status,
                        Count = s.Count
                    }).ToList(),
                    UpcomingHearings = dto.UpcomingHearings.Select(h => new NextHearingItem
                    {
                        CaseId = h.CaseId,
                        CaseTitle = h.CaseTitle,
                        HearingDate = h.HearingDate,
                        CourtName = h.CourtName,
                        OpponentName = h.OpponentName
                    }).ToList(),
                    MonthlyCaseStatuses = dto.MonthlyCaseStatuses.Select(m => new MonthlyCaseStatus
                    {
                        Month = m.Month,
                        Filed = m.Filed,
                        Disposed = m.Disposed
                    }).ToList()
                };

                return View(model);
            }

            return View(null);
        }

        //public async IActionResult Index()
        //{
        //    _notify.Information("Hi There!");
        //    var response = await _mediator.Send(new DashboardQueryCommand { LinkedIds = User.GetUserLinkedIds() });
        //    if (response.Succeeded)
        //    {
        //        var model = new DashboardViewModel
        //        {
        //            TotalCases = 120,
        //            DisposedCases = 45,
        //            PendingCases = 60,
        //            AssignedCases = 15,
        //            StatusSummaries = new List<CaseStatusSummary>
        //        {
        //            new CaseStatusSummary { Status = "Pending", Count = 60 },
        //            new CaseStatusSummary { Status = "Disposed", Count = 45 },
        //            new CaseStatusSummary { Status = "Assigned", Count = 15 }
        //        },
        //            UpcomingHearings = new List<NextHearingItem>
        //        {
        //            new NextHearingItem
        //            {
        //                CaseId = 101,
        //                CaseTitle = "State vs Sharma",
        //                HearingDate = DateTime.Today.AddDays(1),
        //                CourtName = "District Court Bhopal",
        //                OpponentName = "Rakesh Sharma"
        //            },
        //            new NextHearingItem
        //            {
        //                CaseId = 102,
        //                CaseTitle = "Rajeev vs Govt",
        //                HearingDate = DateTime.Today.AddDays(3),
        //                CourtName = "High Court Indore",
        //                OpponentName = "Govt. Advocate"
        //            }
        //        },
        //            MonthlyCaseStatuses = new List<MonthlyCaseStatus>
        //        {
        //                new MonthlyCaseStatus { Month = "Jan", Filed = 20, Disposed = 10 },
        //                new MonthlyCaseStatus { Month = "Feb", Filed = 30, Disposed = 25 },
        //                new MonthlyCaseStatus { Month = "Mar", Filed = 25, Disposed = 20 },
        //                new MonthlyCaseStatus { Month = "Apr", Filed = 15, Disposed = 10 },
        //                new MonthlyCaseStatus { Month = "May", Filed = 40, Disposed = 35 },
        //                new MonthlyCaseStatus { Month = "Jun", Filed = 10, Disposed = 5 }
        //        }
        //        };
        //        return View(model);
        //    }
        //    else {
        //        return View(null);
        //    }
        //}
    }
}