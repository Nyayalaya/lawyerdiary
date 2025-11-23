using AuditTrail.Abstrations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourtApp.Domain.Entities.Account
{

    [Table("m_court_fee_type", Schema = "account")]
    public class CourtFeeTypeEntity : AuditableEntity
    {        
        public required string CourtFeeType { get; set; }
    }
}
