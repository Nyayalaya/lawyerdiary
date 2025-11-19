using CourtApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.LawyerDiary.Models
{
    public class CourtTypeViewModel
    {
        public Guid Id { get; set; }
        public string CourtType { get; set; }
        public string Abbreviation { get; set; }
        public List<LanguageViewModel> Language { get; set; }
    }
}
