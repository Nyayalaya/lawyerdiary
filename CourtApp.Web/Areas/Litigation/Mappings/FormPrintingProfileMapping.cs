using AutoMapper;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.DTOs.FormPrint;
using CourtApp.Web.Areas.Litigation.Models;

namespace CourtApp.Web.Areas.Litigation.Mappings
{
    public class FormPrintingProfileMapping : Profile
    {
        public FormPrintingProfileMapping()
        {
            CreateMap<InspectionResponse, InspectionViewModel>();
            CreateMap<PermissionSlipResponse, PermissionSlipDataModel>();
            CreateMap<ShowCauseNoticeResponse, ShowCauseViewModel>();
            CreateMap<NoticeOfStayAppResponse, NoticeOfStayApplication>();
            CreateMap<NoticeOfAdmissionResponse, NoticeAdmissionViewModel>();
            CreateMap<CopyingAppResponse, CopyingAppViewModel>();
            CreateMap<ApplicantDetailDto, ApplicantDetailViewModel>();
            CreateMap<GlobalFormPrintDto, FormPrintData>();
            
        }
    }
}
