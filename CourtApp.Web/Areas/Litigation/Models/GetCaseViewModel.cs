using System;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class GetCaseViewModel
    {
        public Guid Id { get; set; }
        public string CaseTypeName { get; set; }
        public string CourtType { get; set; }
        public string CaseNumber { get; set; }
        public string CaseYear { get; set; }
        public string NextHearingDate { get; set; }
        public string CaseStage { get; set; }
        public string Status { get; set; }
        public string FirstTitle { get; set; }
        public string SecondTitle { get; set; }
        public string CourtName { get; set; }
        public string CaseTitle { get; set; }
        public string Appearence { get; set; }
        public bool IsProceedingDone { get; set; }
        public string Reference { get; set; }
        public bool IsCaseAssigned { get; set; }
        public Guid LawyerId { get; set; }
        public bool HasChild { get; set; }
    }
}
