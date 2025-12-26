using AuditTrail.Abstrations;
using CourtApp.Domain.Entities.CaseDetails;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace CourtApp.Domain.Entities.FormBuilder
{
    [Table("case_petition_detail")]
    public class DraftingDetailEntity : AuditableEntity
    {        
              
        public Guid CaseId { get; set; }        
        public Guid TemplateId { get; set; }
        public Guid FormId { get; set; }
        public string LangCode { get; set; } = "en";
        public List<FormFieldValueEntity> FieldDetails { get; set; }
        public virtual CaseDetailEntity Case { get; set; }
        public virtual FormBuilderEntity Form { get; set; }
        public virtual TemplateInfoEntity Template { get; set; }
    }
}
