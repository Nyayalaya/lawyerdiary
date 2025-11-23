using AutoMapper;
using CourtApp.Application.Features.Typeofcasess.Commands;
using CourtApp.Application.Features.Typeofcasess.Query;
using CourtApp.Application.Features.CourtType.Command;
using CourtApp.Application.Features.CourtType.Query;
using CourtApp.Domain.Entities.LawyerDiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.Mappings
{
    public class CourtTypeProfile : Profile
    {
        public CourtTypeProfile()
        {
            CreateMap<GetCourtTypeResponse, CourtTypeEntity>().ReverseMap();
            CreateMap<CreateCourtTypeCommand, CourtTypeEntity>().ReverseMap();
            CreateMap<UpdateCourtTypeCommand, CourtTypeEntity>().ReverseMap();
            CreateMap<DeleteCourtTypeCommand, CourtTypeEntity>().ReverseMap();

        }
    }
}
