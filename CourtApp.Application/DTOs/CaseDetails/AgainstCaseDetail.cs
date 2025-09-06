using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.DTOs.CaseDetails
{
    public class AgainstCaseDetail
    {
        public Guid Id { get; set; }
        public string ImpugedOrder { get; set; }        
        public string State { get; set; }
        public bool IsHighCourt { get; set; }
        public string CourtType { get; set; }
        public string CourtBench { get; set; }
        public string DistrictCourt { get; set; }
        public string CourtComplex { get; set; }
        public string CaseNo { get; set; }
        public string CaseYear { get; set; }
        public string CaseCategory { get; set; }
        public string CaseType { get; set; }
        public string CisNo { get; set; }
        public string CisYear { get; set; }
        public string CnrNo { get; set; }
        public string OfficerName { get; set; }
        public string Cadre { get; set; }
        public string CourtDistrict { get; set; }
    }
}
