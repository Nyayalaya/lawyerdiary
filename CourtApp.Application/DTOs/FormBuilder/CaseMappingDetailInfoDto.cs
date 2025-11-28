using System;
using System.Collections.Generic;
namespace CourtApp.Application.DTOs.FormBuilder
{
    public class CaseMappingDetailInfoDto
    {
        public Guid CaseId { get; set; }
        //public string CaseTitle { get; set; }
        //public string CaseNoYear { get; set; }
        //public string CaseType { get; set; }
        //public string TemplateName { get; set; }
        //public string TemplatePath { get; set; }
        public string TemplateBody { get; set; }
        //public string Strength { get; set; }
        //public string FirstTitle { get; set; }
        //public string FirstTitleType { get; set; }
        //public string SecondTitle { get; set; }
        //public string SecondTitleType { get; set; }
        //public string ImpungedOrderDt { get; set; }
        //public string Office { get; set; }
        //public string Cadre { get; set; }
        //public string AgCourt { get; set; }
        //public string AgCaseNoYear { get; set; }
        //public string AgCNR { get; set; }
        //public string AgCisNoYear { get; set; }
        //public string State { get; set; }
        //public List<CaseCompleteTitle> CaseCompleteTitles { get; set; }
        public List<MappingDetails> TagValues { get; set; }
    }
    public class CaseCompleteTitle
    {
        public string Title { get; set; }
    }
    public class MappingDetails
    {
        public string Tag { get; set; }
        public string Value { get; set; }
    }
}
