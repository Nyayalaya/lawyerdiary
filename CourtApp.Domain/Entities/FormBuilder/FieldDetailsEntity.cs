using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourtApp.Domain.Entities.FormBuilder
{
    
    public class FieldDetailsEntity
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string DefaultVal { get; set; }
        public string Tag { get; set; }
        public bool IsRequire { get; set; }
        public int DispOrder { get; set; }
        public string Placeholder { get; set; }       
        public FieldSizeEntity FieldSize { get; set; }
    }
}
