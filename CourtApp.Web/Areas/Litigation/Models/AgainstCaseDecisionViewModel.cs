using System.ComponentModel;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class AgainstCaseDecisionViewModel
    {
        //public string ImpugedOrder { get; set; }
        //public string InstitutionDate { get; set; }
        //public string State { get; set; }
        //public bool IsHighCourt { get; set; }
        //public string CourtType { get; set; }
        //public string CourtBench { get; set; }
        //public string DistrictCourt { get; set; }
        //public string CourtComplex { get; set; }
        //public string CaseNoYear { get; set; }
        //public string CaseCategory { get; set; }
        //public string CaseType { get; set; }
        //public string CisNoYear { get; set; }
        //public string CnrNo { get; set; }
        //public string OfficerName { get; set; }
        //public string Cadre { get; set; }

        [DisplayName("Impuged Order")]
        public string ImpugedOrder { get; set; }
        public string State { get; set; }
        public bool IsHighCourt { get; set; }
        [DisplayName("Type of Court")]
        public string CourtType { get; set; }

        [DisplayName("Court/Bench")]
        public string CourtBench { get; set; }

        [DisplayName("Court District")]
        public string DistrictCourt { get; set; }
        [DisplayName("Court Complex")]
        public string CourtComplex { get; set; }

        [DisplayName("Case Number")]
        public string CaseNo { get; set; }

        [DisplayName("Case Year")]
        public string CaseYear { get; set; }

        [DisplayName("Case Category")]
        public string CaseCategory { get; set; }

        [DisplayName("Case Type")]
        public string CaseType { get; set; }

        [DisplayName("Cis Number")]
        public string CisNo { get; set; }

        [DisplayName("Cis Year")]
        public string CisYear { get; set; }

        [DisplayName("Cnr Number")]
        public string CnrNo { get; set; }

        [DisplayName("Officer Name")]
        public string OfficerName { get; set; }        
        public string Cadre { get; set; }
        public string CourtDistrict { get; set; }
        public string CisNoYear { get; set; }

    }
}
