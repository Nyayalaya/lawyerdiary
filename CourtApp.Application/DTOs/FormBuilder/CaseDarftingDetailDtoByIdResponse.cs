using CourtApp.Domain.Entities.FormBuilder;
using System;
using System.Collections.Generic;
namespace CourtApp.Application.DTOs.FormBuilder
{
    public class CaseDarftingDetailDtoByIdResponse
    {
        public Guid Id { get; set; }        
        public Guid CaseId { get; set; }
        public Guid TemplateId { get; set; }
        public Guid DraftingFormId { get; set; }
        public string LangCode { get; set; }
        public List<FormFieldDetailValue> FieldDetails { get; set; }
    }
    public class FormFieldDetailValue: FieldDetailsDto
    {
        public string Value { get; set; }
    }
}
