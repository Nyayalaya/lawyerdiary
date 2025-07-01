using AuditTrail.Abstrations;
using CourtApp.Domain.Entities.LawyerDiary;
using CourtApp.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourtApp.Domain.Entities.CaseDetails
{

    [Table("case_detail_against", Schema = "ld")]
    public class CaseDetailAgainstEntity
    {
        #region Common Properties

        public Guid Id { get; set; }
        /// <summary>
        /// This field is linked with CaseDetail Entity. 
        /// And one case may have multiple against case.
        /// </summary>
        public Guid CaseId { get; set; }
        public DateTime ImpugedOrderDate { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid CaseCategoryId { get; set; }
        public Guid CaseTypeId { get; set; }
        public int StateId { get; set; }
        public string CaseNo { get; set; }
        public int CaseYear { get; set; }
        public int CisYear { get; set; }
        public string OfficerName { get; set; }
        public Guid? CadreId { get; set; }
        public string CisNo { get; set; }
        public string CnrNo { get; set; }

        /// <summary>
        /// This property is used commonly for High court bench. 
        /// And Other Court also, linked with CourtBenchEntity
        /// </summary>
        public Guid CourtBenchId { get; set; }
        #endregion

        #region Other than High Court Propeties
        public Guid? CourtDistrictId { get; set; }
        public Guid? ComplexId { get; set; }
        
        #endregion

        #region HighCourt Properties        
        public int? StrengthId { get; set; }
        #endregion

        #region Navigational Properties
        public virtual StateEntity State { get; set; }
        public virtual CourtBenchEntity CourtBench { get; set; }
        public virtual CourtDistrictEntity CourtDistrict { get; set; }
        public virtual CourtComplexEntity Complex { get; set; }
        public virtual CaseDetailEntity Case { get; set; }
        public virtual CourtTypeEntity CourtType { get; set; }
        public virtual NatureEntity CaseCategory { get; set; }
        public virtual TypeOfCasesEntity CaseType { get; set; }
        public virtual CadreMasterEntity Cadre { get; set; }
        #endregion

    }
}
