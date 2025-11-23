using AuditTrail.Abstrations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourtApp.Domain.Entities.Account
{

    [Table("court_fee", Schema = "account")]
    public class CourtFeeEntity : AuditableEntity
    {
        public Guid FeeTypeId { get; set; }     
        public float Value { get; set; }
        public virtual CourtFeeTypeEntity FeeType { get; set; }
    }
}