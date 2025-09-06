using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourtApp.Application.DTOs.CaseDetails;

namespace CourtApp.Application.DTOs.FormPrint
{
    public class GlobalFormPrintDto
    {
        public string State { get; set; }
        public string InstitutionDate { get; set; }
        public string CourtType { get; set; }
        public string CourtDistrict { get; set; }
        public string CourtComplex { get; set; }
        public string Court { get; set; }
        public string CaseCategory { get; set; }
        public string CaseType { get; set; }
        public string CaseStage { get; set; }
        public string Petitioner { get; set; }
        public string PetitionerTitle { get; set; }
        public string Respondent { get; set; }
        public string RespondentTitle { get; set; }
        public string CaseNoYear { get; set; }
        public string NextDate { get; set; }
        public string DisposalDate { get; set; }
        public string PetitionerAppearance { get; set; }
        public string RespondantAppearance { get; set; }
        public string CisNoYear { get; set; }
        public string CnrNo { get; set; }
        public string Strength { get; set; }
        public AgainstCaseDetail AgainstCourtDetail { get; set; }
        public List<ApplicantDetailDto> FirstPartyDetails { get; set; }
        public List<ApplicantDetailDto> SecondPartyDetails { get; set; }
    }
}
