using System;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class GetCaseInfoViewModel
    {
        public Guid Id { get; set; }
        public string Reference { get; set; }
        public string CourtType { get; set; }
        public string Court { get; set; }
        public string CaseType { get; set; }
        public string CaseStage { get; set; }
        public string NextDate { get; set; }
        public string ProceedingDate { get; set; }
        public string CaseDetail { get; set; }
        public string No { get; set; }
        public string Year { get; set; }
        public bool IsProceedingDone { get; set; }
        public bool IsCaseAssigned { get; set; }
        public Guid LawyerId { get; set; }
        public string ActionHtml { get; set; }
    }
}
