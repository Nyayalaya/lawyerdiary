using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Features.BookTypes.Command;
using CourtApp.Application.Features.Languages;
using CourtApp.Domain.Entities.Common;
using CourtApp.Domain.Entities.LawyerDiary;

namespace CourtApp.Application.Mappings
{
    public class LanguageProfileMapping:Profile
    {
        public LanguageProfileMapping()
        {
            CreateMap<CreateLanguageCommand, LanguageEntity>();
            CreateMap<LangEntity, LangDto>().ReverseMap();
           
        }
    }
}
