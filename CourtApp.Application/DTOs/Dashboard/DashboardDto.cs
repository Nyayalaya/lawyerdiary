using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.DTOs.Dashboard
{
    public class DashboardDto
    {
        public int TotalCases { get; set; }
        public int DisposedCases { get; set; }
        public int PendingCases { get; set; }
        public int AssignedCases { get; set; }
        public int TodayHearing { get; set; }
        public List<CaseStatusSummaryDto> StatusSummaries { get; set; } = new();
        public List<NextHearingItemDto> UpcomingHearings { get; set; } = new();
        public List<MonthlyCaseStatusDto> MonthlyCaseStatuses { get; set; }
    }
}
