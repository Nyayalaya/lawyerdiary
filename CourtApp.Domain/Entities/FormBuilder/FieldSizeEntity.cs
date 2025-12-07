using System.ComponentModel.DataAnnotations.Schema;

namespace CourtApp.Domain.Entities.FormBuilder
{
    
    public class FieldSizeEntity
    {
        public string Min { get; set; }
        public string Max { get; set; }
        public string Rows { get; set; }
        public string Cols { get; set; }
    }
}
