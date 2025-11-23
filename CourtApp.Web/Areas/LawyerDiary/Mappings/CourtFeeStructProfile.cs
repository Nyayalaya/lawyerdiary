using AutoMapper;
using CourtApp.Application.DTOs.Account;
using CourtApp.Application.Features.Account;
using CourtApp.Web.Areas.LawyerDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.LawyerDiary.Mappings
{
    public class CourtFeeStructProfile : Profile
    {
        public CourtFeeStructProfile()
        {
            CreateMap<CourtFeeStructureByIdDto, CourtFeeStructureViewModel>().ReverseMap();
            CreateMap<CourtFeeStructureDto, CourtFeeStructureViewModel>().ReverseMap();
            CreateMap<CourtFeeStructureCreateCommand, CourtFeeStructureViewModel>().ReverseMap();
            CreateMap<CourtFeeStructureUpdateCommand, CourtFeeStructureViewModel>().ReverseMap();
            CreateMap<CourtFeeStructureDeleteCommand, CourtFeeStructureViewModel>().ReverseMap();
        }
    }
}
