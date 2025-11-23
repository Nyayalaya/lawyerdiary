using System;
using System.Collections.Generic;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class PendingWorkDataViewModel
    {
        public List<CaseTitleWorkData> PendingWork { get; set; }
    }

    public class CaseTitleWorkData
    {
        public Guid Id { get; set; }
        public string CaseTitle { get; set; }
        public DateTime WorkDate { get; set; }
        public DateTime ProceedingDate { get; set; }
        public List<WorkDt> Works { get; set; }
    }

    public class WorkDt
    {
        public Guid Id { get; set; }
        public string Work { get; set; }
        public Guid WorkId { get; set; }
        public bool Selected { get; set; }
    }
}
