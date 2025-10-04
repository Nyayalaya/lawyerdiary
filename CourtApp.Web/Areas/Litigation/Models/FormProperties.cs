using System;

namespace CourtApp.Web.Areas.Litigation.Models
{
    public class FormProperties
    {
        public Guid Key { get; set; }
        public string Tag { get; set; }
        public string Type { get; set; }
        public bool IsRequired { get; set; }       
        public string Name { get; set; }
        public string Value { get; set; }
        public string DefaultVal { get; set; }
    }
}
