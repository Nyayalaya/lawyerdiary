using System;
namespace CourtApp.Application.DTOs.FormBuilder
{
    public class CaseDarftingDtoResponse
    {
        public Guid Id { get; set; }
        public string LangCode { get; set; } 
        public string CaseTitle { get; set; }
        public string DraftingForm { get; set; }
        public string TemplateName { get; set; }
    }   
}
