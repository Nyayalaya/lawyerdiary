using AutoMapper;
using CourtApp.Application.DTOs.CourtForm;
using CourtApp.Application.Features.CourtForm;
using CourtApp.Web.Areas.LawyerDiary.Models.CourtForm;

namespace CourtApp.Web.Areas.LawyerDiary.Mappings
{
    public class CourtFormTypeProfileMapping:Profile
    {
        public CourtFormTypeProfileMapping()
        {
            CreateMap<CourtFormAddUpdateViewModel, CreateCourtFormCommand>();
            CreateMap<CourtFormAddUpdateViewModel, UpdateCourtFormCommand>();
            CreateMap<CourtFormDto, CourtFormGetViewModel>();
            CreateMap<CourtFormByIdDto, CourtFormAddUpdateViewModel>();
        }
    }
}
