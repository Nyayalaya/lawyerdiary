using System;
namespace CourtApp.Application.DTOs.Account
{
    public class BillingDetailDto
    {
        public Guid Id { get; set; }
        public string LawyerId { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string Branch { get; set; }
        public string IfscCode { get; set; }
        public string PanNumber { get; set; }
        public string GstNo { get; set; }
    }
}
