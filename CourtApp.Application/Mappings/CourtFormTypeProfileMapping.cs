using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CourtApp.Application.DTOs.CourtForm;
using CourtApp.Application.Features.CourtForm;
using CourtApp.Domain.Entities.FormBuilder;

namespace CourtApp.Application.Mappings
{
    public class CourtFormTypeProfileMapping:Profile
    {
        public CourtFormTypeProfileMapping()
        {

            CreateMap<CreateCourtFormCommand, CourtFormTypeEntity>();
            CreateMap<CourtFormTypeEntity, CourtFormByIdDto>();
            CreateMap<UpdateCourtFormCommand, CourtFormTypeEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
