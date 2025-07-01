using System;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class HearingViewModel
    {
        public Guid Id { get; set; }
        public string CaseNumber { get; set; }
        public string CaseYear { get; set; }
        public string CaseStage { get; set; }
        public string CaseTypeName { get; set; }
        public DateTime NextHearingDate { get; set; }
        public string CourtName { get; set; }
        public string CaseTitle { get; set; }
        public bool Selected { get; set; }
        public bool IsProceedingDone { get; set; }
        public string Reference { get; set; }
        public bool IsCaseAssigned { get; set; }
        public Guid LawyerId { get; set; }
        public bool HasChild { get; set; }

    }
}
