using System;

namespace CourtApp.Application.DTOs.CaseDetails
{
    public class GetCaseInfoDto
    {
        public Guid Id { get; set; }
        public DateTime InstitutionDate { get; set; }
        public string Reference { get; set; }
        public string CourtType { get; set; }
        public string Court { get; set; }
        public string CaseType { get; set; }
        public string CaseStage { get; set; }
        public string NextDate { get; set; }
        public string CaseDetail { get; set; }
        public DateTime? DisposalDate { get; set; }
        public DateTime OrderByKey { get; set; }
        public string No { get; set; }
        public string Year { get; set; }
        public bool IsCaseAssigned { get; set; }
        public Guid LawyerId { get; set; }
    }
}
