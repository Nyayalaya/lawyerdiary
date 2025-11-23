using AuditTrail.Abstrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace CourtApp.Domain.Entities.FormBuilder
{
    [Table("m_template_info")]
    public class TemplateInfoEntity:AuditableEntity
    {
        public Guid FormId { get; set; }
        public string TemplateName { get; set; }
        public string TemplatePath { get; set; }
        public string TemplateBody { get; set; }
        public List<TemplateTagsEntity> Tags { get; set; }
    }
    public class TemplateTagsEntity
    {
        public string Tag { get; set; }
    }
}
