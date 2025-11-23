using CourtApp.Application.DTOs.CaseDetails;
namespace CourtApp.Application.DTOs.Registers
{
    public class CopyDisposalResponse: CaseBasicInfoDto
    {
        public string Reason { get; set; }
        public string AppliedOn { get; set; }
        public string ReceivedOn { get; set; }
    }
}
