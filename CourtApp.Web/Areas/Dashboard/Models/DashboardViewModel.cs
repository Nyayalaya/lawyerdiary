using System;
using System.Collections.Generic;

namespace CourtApp.Web.Areas.Dashboard.Models
{
    public class DashboardViewModel
    {
        public int TotalCases { get; set; }
        public int DisposedCases { get; set; }
        public int PendingCases { get; set; }
        public int AssignedCases { get; set; }
        public int TodayHearing { get; set; }

        public List<CaseStatusSummary> StatusSummaries { get; set; } = new();
        public List<NextHearingItem> UpcomingHearings { get; set; } = new();
        public List<MonthlyCaseStatus> MonthlyCaseStatuses { get; set; }
    }
    public class CaseStatusSummary
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
    public class NextHearingItem
    {
        public Guid CaseId { get; set; }
        public string CaseTitle { get; set; }
        public string HearingDate { get; set; }
        public string CourtName { get; set; }
        public string OpponentName { get; set; }
    }

    public class MonthlyCaseStatus
    {
        public string Month { get; set; } // e.g. "Jan", "Feb"
        public int Filed { get; set; }
        public int Disposed { get; set; }
    }
}
