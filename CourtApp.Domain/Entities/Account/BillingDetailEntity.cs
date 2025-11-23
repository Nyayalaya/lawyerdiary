using System.ComponentModel.DataAnnotations.Schema;
using AuditTrail.Abstrations;
namespace CourtApp.Domain.Entities.Account
{
    [Table("billing_detail", Schema = "account")]
    public class BillingDetailEntity:AuditableEntity
    {
        public string LawyerId { get; set; }
        public string BankName { get; set; }        
        public string AccountNo { get; set; }
        public string Branch { get; set; }
        public string IfscCode { get; set; }
        public string PanNumber { get; set; }
        public string GstNo { get; set; }
    }
}
