using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourtApp.Web.Areas.LawyerDiary.Models.CourtForm
{
    public class CourtFormAddUpdateViewModel
    {
        public Guid Id { get; set; }
        public SelectList States { get; set; }
        public SelectList CourtTypes { get; set; }
        public SelectList CaseCategory { get; set; }
        public SelectList Languages { get; set; }
        public SelectList CaseTypes { get; set; }
        public int StateId { get; set; }
        public string LanguageCode { get; set; }
        public Guid? CaseTypeId { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid CaseCategoryId { get; set; }
        public string FormName { get; set; }
        public string FormTemplate { get; set; }
    }
}
