using System;
using System.Collections.Generic;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class FormCaseMappingViewModel
    {
        public Guid Id { get; set; }
        public string LangCode { get; set; } 
        public string CaseTitle { get; set; }
        public string TemplateName { get; set; }
        public string DraftingForm { get; set; }
    }
   
}
