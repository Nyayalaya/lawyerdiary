using System;

namespace CourtApp.Web.Areas.LawyerDiary.Models.CourtForm
{
    public class CourtFormGetViewModel
    {
        public Guid Id { get; set; }
        public string StateName { get; set; }
        public string CaseCategory { get; set; }
        public string CourtType { get; set; }
        public string FormName { get; set; }
        public string Language { get; set; }
        public string CaseType { get; set; }
    }
}
