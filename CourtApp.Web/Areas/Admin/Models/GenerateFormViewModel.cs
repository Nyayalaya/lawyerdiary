using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourtApp.Web.Areas.Admin.Models
{
    public class GenerateFormViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string FormName { get; set; }
        public string Mode { get; set; }
        public FormViewModel Form { get; set; }
    }
    public class FormViewModel
    {
        public List<FormFields> Fields { get; set; }
    }
    public class FormFields
    {
        [Required]
        public Guid Key { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Type { get; set; }
        public SelectList Types { get; set; }
        public string DefaultVal { get; set; }
        public string Tag { get; set; }
        //public bool IsRequire { get; set; }
        //public int DispOrder { get; set; }
        //public string Placeholder { get; set; }
        //public FieldLength FieldSize { get; set; }
    }

    public class FieldLength
    {
        public string Min { get; set; }
        public string Max { get; set; }
    }
}
