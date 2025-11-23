using System;

namespace CourtApp.Web.Models
{
    // Models/CaseActionViewModel.cs
    public class CaseActionViewModel
    {
        public Guid CaseId { get; set; }
        public string Reference { get; set; }
        public bool IsCaseAssigned { get; set; }
        public Guid LawyerId { get; set; }
    }

}
