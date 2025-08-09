using System;
using System.Collections.Generic;

namespace CourtApp.Web.Areas.Report.Models
{
    public class InstitutionRegisterViewMode
    {
        public List<InstituteModel> dtmodel { get; set; }
    }
    public class InstituteModel : CaseDetailViewModel
    {
        
        public bool IsCaseAssigned { get; set; }
        public Guid LawyerId { get; set; }
    }
}
