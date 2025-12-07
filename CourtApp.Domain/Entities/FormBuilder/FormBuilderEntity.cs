using AuditTrail.Abstrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace CourtApp.Domain.Entities.FormBuilder
{
    [Table("m_frm_types")]
    public class FormBuilderEntity : AuditableEntity
    {
        

        [Column(Order = 2)]
        public string FormName { get; set; }

        [Column(Order = 3)]
        public FormFieldsEntity FieldsDetails { get; set; }
    }
    
    public class FormFieldsEntity
    {
        public List<FieldDetailsEntity> Fields { get; set; }
    }
}
