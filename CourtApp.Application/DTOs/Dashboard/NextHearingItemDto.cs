using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.DTOs.Dashboard
{
    public class NextHearingItemDto
    {
        public Guid CaseId { get; set; }
        public string CaseTitle { get; set; }
        public string HearingDate { get; set; }
        public string CourtName { get; set; }
        public string OpponentName { get; set; }
    }
}
