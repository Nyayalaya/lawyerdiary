using System;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class UpdateHearingDtViewModel
    {
        public Guid CaseId { get; set; }
        public DateTime HearingDt { get; set; }
        public string ProcDt { get; set; }
        public bool IsParent { get; set; }
        public string CaseNoYear { get; set; }
    }
}
