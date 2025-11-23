using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace CourtApp.Web.Areas.Admin.Models
{
    public class TemplateViewModel
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateBody { get; set; }
        public SelectList Forms { get; set; }
    }
}
