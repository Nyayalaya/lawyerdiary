using System;
using System.Collections.Generic;

namespace CourtApp.Application.DTOs.FormBuilder
{
    public class GetTemplateInfoByIdDto
    {
        public Guid FormId { get; set; }
        public string TemplateName { get; set; }
        public string TemplatePath { get; set; }
        public string TemplateBody { get; set; }
        public List<Tags> Tags { get; set; }
    }
    public class Tags
    {
        public string Tag { get; set; }
    }
}
