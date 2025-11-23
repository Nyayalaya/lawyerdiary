using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class CopyCaseViewModel
    {
        public Guid CaseId { get; set; }
        public SelectList Cases { get; set; }
    }
}
