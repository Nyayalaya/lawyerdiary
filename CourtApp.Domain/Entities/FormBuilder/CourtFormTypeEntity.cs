using System;
using System.ComponentModel.DataAnnotations.Schema;
using AuditTrail.Abstrations;
using CourtApp.Domain.Entities.LawyerDiary;
using CourtApp.Entities.Common;

namespace CourtApp.Domain.Entities.FormBuilder
{
    [Table("m_court_case_template")]
    public class CourtFormTypeEntity:AuditableEntity,IDomainLayer
    {
        public int StateId { get; set; }
        public string LanguageCode { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid? CaseCategoryId { get; set; }
        public string FormName { get; set; }
        public string FormTemplate { get; set; }
        public Guid? CaseTypeId { get; set; }
        public virtual TypeOfCasesEntity CaseType { get; set; }
        public virtual StateEntity State { get; set; }
        public virtual CourtTypeEntity CourtType { get; set; }
        public virtual NatureEntity CaseCategory { get; set; }
    }
}
