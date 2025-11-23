using System;
namespace CourtApp.Application.DTOs.CaseDetails
{
    public class CaseWithOutHearingDateDto:CaseBasicInfoDto
    {  
        public string ProceedingDate { get; set; }
        public string NextDate { get; set; }
    }
}
