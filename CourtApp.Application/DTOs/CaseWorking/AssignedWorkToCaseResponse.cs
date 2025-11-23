using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.DTOs.CaseWorking
{
    public class AssignedWorkToCaseResponse
    {
        public Guid CaseId { get; set; }
        public string CaseDetail { get; set; }
        public DateTime LastWorkingDate { get; set; }
        public DateTime ProceedingDate { get; set; }
        public List<AssignedWork> AWorks { get; set; }
    }
}
