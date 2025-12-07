using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Domain.Entities.FormBuilder
{
    
    public class FormFieldValueEntity
    {
        public string Tag { get; set; }
        public string Value { get; set; }
    }
}
