using System;
using System.Collections.Generic;
namespace CourtApp.Domain.Entities.CaseDetails
{
    public class ProceedingWorkEntity
    {
        public DateTime? LastWorkingDate { get; set; }
        public List<ProcWorkEntity> Works { get; set; }
    }
    public class ProcWorkEntity
    {
        public Guid WorkTypeId { get; set; }
        public Guid WorkId { get; set; }
        public int Status { get; set; }
        public DateTime AppliedOn { get; set; }
        public DateTime ReceivedOn { get; set; }
        //public virtual WorkMasterEntity WorkType { get; set; }
        //public virtual WorkMasterSubEntity Work { get; set; }
    }
}
