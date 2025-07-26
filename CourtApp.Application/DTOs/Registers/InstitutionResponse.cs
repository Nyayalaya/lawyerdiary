using System;
using CourtApp.Application.DTOs.CaseDetails;
namespace CourtApp.Application.DTOs.Registers
{
    public class InstitutionResponse : CaseBasicInfoDto
    {
        public bool IsCaseAssigned { get; set; }
        public string Reference { get; set; }
        public Guid LawyerId { get; set; }
    }
}
