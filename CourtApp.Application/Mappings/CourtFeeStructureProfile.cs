using AutoMapper;
using CourtApp.Application.DTOs.Account;
using CourtApp.Application.Features.Account;
using CourtApp.Domain.Entities.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.Mappings
{
    public class CourtFeeStructureProfile : Profile
    {
        public CourtFeeStructureProfile()
        {
            CreateMap<CourtFeeStructureEntity, CourtFeeStructureByIdDto>()
                .ForPath(d => d.StateName, opt => opt.MapFrom(src => src.State.Name_En));
            CreateMap<CourtFeeStructureCreateCommand, CourtFeeStructureEntity>()
                .ForPath(d => d.State.Id, opt => opt.MapFrom(src => src.StateCode));
        }
    }
}
