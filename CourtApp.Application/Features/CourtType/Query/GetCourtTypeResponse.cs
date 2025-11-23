using CourtApp.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.CourtType.Query
{
    public class GetCourtTypeResponse
    {
        public Guid Id { get; set; }
        public string CourtType { get; set; }
        public string Abbreviation { get; set; }
        public string CourtType_Hn { get; set; }
        public List<LangDto> Language { get; set; }
    }
}
