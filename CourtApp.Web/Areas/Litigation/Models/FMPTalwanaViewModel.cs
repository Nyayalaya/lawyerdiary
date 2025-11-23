using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class FMPTalwanaViewModel
    {
        public List<TalwanaViewModel> TalwanaCases { get; set; }
    }
    public class TalwanaViewModel
    {
        public string State { get; set; }
        public string City { get; set; }
        public string CaseNo { get; set; }
        public string Petitioner { get; set; }
        public string Respondent { get; set; }
        public List<RespondantDetail> RespondantDetails { get; set; }
        public List<AdocateDetail> AdvDetails { get; set; }
    }
    public class AdocateDetail
    {
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Enrollment { get; set; }
        public string Address { get; set; }
    }

    public class RespondantDetail
    {
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string PoliceStation { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
    }
}
