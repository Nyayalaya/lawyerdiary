using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class CaseUpseartViewModel
    {
        public Guid UserCaseId { get; set; }
        public string StatusMessage { get; set; }
        public bool IsHighCourt { get; set; }
        public Guid Id { get; set; }
        public string from { get; set; }
        #region Common Properties Among all Court Type
        public DateTime InstitutionDate { get; set; }
        public int StateId { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid CaseCategoryId { get; set; }
        public Guid CaseTypeId { get; set; }
        public string CaseNo { get; set; }
        public int? CaseYear { get; set; }
        public string FirstTitle { get; set; }
        public Guid FTitleId { get; set; }
        public string SecondTitle { get; set; }
        public Guid STitleId { get; set; }
        public string CisNumber { get; set; }
        public int? CisYear { get; set; }
        public string CnrNumber { get; set; }
        public DateTime? NextDate { get; set; }
        public Guid? CaseStageId { get; set; }
        public Guid? LinkedCaseId { get; set; }
        public Guid? ClientId { get; set; }
        public Guid AppearenceID { get; set; }
        public Guid? LCaseId { get; set; }
        public List<CaseAgainstModel> AgainstCaseDetails { get; set; }
        #endregion

        #region Other than High Court Propeties
        public Guid? CourtDistrictId { get; set; }
        public Guid? ComplexId { get; set; }
        public Guid? CourtId { get; set; }

        #endregion

        #region HighCourt Properties        
        public int? StrengthId { get; set; }
        public Guid? BenchId { get; set; }
        #endregion

        #region Select List Region
        public SelectList CaseNatures { get; set; }
        public SelectList TypeOfCases { get; set; }
        public SelectList CourtTypes { get; set; }
        public SelectList Courts { get; set; }
        public SelectList CaseKinds { get; set; }
        public SelectList Years { get; set; }
        public SelectList CaseStages { get; set; }
        public SelectList ClientList { get; set; }
        public SelectList CaseStatusList { get; set; }
        public SelectList FirstTitleList { get; set; }
        public SelectList SecondTitleList { get; set; }
        public SelectList LinkedBy { get; set; }
        public SelectList Cadres { get; set; }
        public SelectList CourtDistricts { get; set; }
        public SelectList ComplexBenchs { get; set; }
        public SelectList States { get; set; }
        public SelectList Strengths { get; set; }
        public SelectList Appearences { get; set; }
        public SelectList UserCases { get; set; }

        #endregion

        #region Case Disposal 
        public DateTime? DisposalDate { get; set; }
        #endregion
    }
}
