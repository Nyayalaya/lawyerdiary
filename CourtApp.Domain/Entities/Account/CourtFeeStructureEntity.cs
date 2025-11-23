using AuditTrail.Abstrations;
using CourtApp.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourtApp.Domain.Entities.Account
{

    [Table("court_fee_structure", Schema = "account")]
    public class CourtFeeStructureEntity : AuditableEntity
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double Rate { get; set; }
        public double FixAmount { get; set; }        
        public int StateId { get; set; }
        public virtual StateEntity State { get; set; }
    }
}
