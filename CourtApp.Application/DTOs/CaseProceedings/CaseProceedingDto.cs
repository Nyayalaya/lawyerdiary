using CourtApp.Application.DTOs.CaseDetails;
using System;
using System.Collections.Generic;

namespace CourtApp.Application.DTOs.CaseProceedings
{
    public class CaseProceedingDto : CaseBasicInfoDto
    {
        public Guid CaseId { get; set; }
        public List<Guid> ParentChildCaseIds { get; set; }
        public Guid HeadId { get; set; }
        public Guid SubHeadId { get; set; }
        public Guid? StageId { get; set; }
        public DateTime? NextDate { get; set; }
        public string Remark { get; set; }
        public ProceedingWorkDto ProcWork { get; set; }

    }
}
