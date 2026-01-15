using CourtApp.Application.DTOs.DropDowns;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.DTOs.CaseTitle
{
    public class CaseTitleDropdownResultDto
    {
        public List<DdlStringStringDto> FirstTitles { get; set; } = new();
        public List<DdlStringStringDto> SecondTitles { get; set; } = new();
    }
}
