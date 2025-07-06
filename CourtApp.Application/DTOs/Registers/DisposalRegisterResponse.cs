using CourtApp.Application.DTOs.CaseDetails;
namespace CourtApp.Application.DTOs.Registers
{
    public class DisposalRegisterResponse:CaseDetailDto
    {       
        public string Reason { get; set; }
        public string DisposalDate { get; set; }
        
    }
}
