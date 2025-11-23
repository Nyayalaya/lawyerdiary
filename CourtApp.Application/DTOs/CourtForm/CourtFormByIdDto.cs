using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourtApp.Domain.Entities.LawyerDiary;
using CourtApp.Entities.Common;

namespace CourtApp.Application.DTOs.CourtForm
{
    public class CourtFormByIdDto
    {
        public Guid Id { get; set; }
        public int StateId { get; set; }
        public string LanguageCode { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid? CaseTypeId { get; set; }
        public Guid? CaseCategoryId { get; set; }
        public string FormName { get; set; }
        public string FormTemplate { get; set; }
       
    }
}
