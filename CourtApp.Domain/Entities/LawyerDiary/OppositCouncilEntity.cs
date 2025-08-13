using AuditTrail.Abstrations;
using System;

namespace CourtApp.Domain.Entities.LawyerDiary
{
    public class OppositCouncilEntity:AuditableEntity, IDomainLayer
    {
        public Guid LawyerId { get; set; }
        public Guid CaseId { get; set; }
        public LawyerMasterEntity Lawyer { get; set; }
    }
}
