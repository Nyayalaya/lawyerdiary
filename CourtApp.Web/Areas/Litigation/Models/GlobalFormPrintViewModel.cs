using CourtApp.Web.Areas.LawyerDiary.Models.Lawyer;
using System.Collections.Generic;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class GlobalFormPrintViewModel
    {
        public List<FormPrintData> CasesInfo { get; set; }
        public LawyerLViewModel CaseLawyerInfo { get; set; }
    }
    public class FormPrintData
    {
        public string State { get; set; }
        public string InstitutionDate { get; set; }       
        public string CourtType { get; set; }
        public string CisNoYear { get; set; }
        public string CourtDistrict { get; set; }
        public string CourtComplex { get; set; }
        public string Court { get; set; }
        public string Strength { get; set; }
        public string CaseCategory { get; set; }
        public string CaseType { get; set; }
        public string CaseStage { get; set; }
        public string Petitioner { get; set; }
        public string PetitionerAppearance { get; set; }
        public string Respondent { get; set; }        
        public string RespondantAppearance { get; set; }        
        public string CaseNoYear { get; set; }
        public string CnrNo { get; set; }
        public string NextDate { get; set; }
        public string DisposalDate { get; set; }
        public AgainstCaseDecisionViewModel AgainstCourtDetail { get; set; }
        public List<ApplicantDetailViewModel> SecondPartyDetails { get; set; }
        public List<ApplicantDetailViewModel> FirstPartyDetails { get; set; }
    }
}
