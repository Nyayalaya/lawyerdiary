using AuditTrail.Abstrations;
using CourtApp.Domain.Entities.LawyerDiary;
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace CourtApp.Domain.Entities.Account
{
    [Table("case_fee", Schema = "account")]
    public class CaseFeeEntity : AuditableEntity
    {        
        public Guid ClientId { get; set; }
        public Decimal SettledAmount { get; set; }
        public Decimal AdvanceAmount { get; set; }
        public virtual ClientEntity Client { get; set; }
    }
}
