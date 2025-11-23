using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.DTOs.CourtForm
{
    public class CourtFormDto
    {
        public Guid Id { get; set; }
        public string StateName { get; set; }
        public string CaseCategory { get; set; }
        public string CourtType { get; set; }
        public string FormName { get; set; }
        public string FormTemplate { get; set; }
        public string Language { get; set; }
        public string CaseType { get; set; }
    }
}
