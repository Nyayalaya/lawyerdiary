using System;

namespace CourtApp.Web.Areas.Report.Models
{
    public abstract class CaseDetailViewModel
    {
        public Guid Id { get; set; }
        public string InsititutionDate { get; set; }
        public string Court { get; set; }
        public string CaseType { get; set; }
        public string Stage { get; set; }
        public string No { get; set; }
        public string Year { get; set; }
        public string FirstTitle { get; set; }
        public string SecondTitle { get; set; }
        public string Reference { get; set; }
    }
}
