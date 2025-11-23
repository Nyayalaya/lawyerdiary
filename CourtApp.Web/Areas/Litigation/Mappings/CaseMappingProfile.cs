using AutoMapper;
using CourtApp.Application.DTOs.Case;
using CourtApp.Application.DTOs.CaseDetails;
using CourtApp.Application.Features.Case;
using CourtApp.Application.Features.CaseDetails;
using CourtApp.Application.Features.Clients.Queries.GetAllCached;
using CourtApp.Web.Areas.Client.Model;
using CourtApp.Web.Areas.Litigation.Models;
using CaseWork = CourtApp.Web.Areas.Litigation.Models.CaseWork;
using CaseWorkDetail = CourtApp.Web.Areas.Litigation.Models.CaseWorkDetail;
namespace CourtApp.Web.Areas.Litigation.Mappings
{
    public class CaseMappingProfile : Profile
    {
        public CaseMappingProfile()
        {

            CreateMap<CaseAgainstModel, CaseAgainstEntityModel>()
                .ForPath(d => d.CourtId, s => s.MapFrom(src => src.CourtId))
                .ReverseMap();
            CreateMap<CaseDetailResponse, GetCaseViewModel>()
                .ForPath(d => d.NextHearingDate,
                            s => s.MapFrom(src => src.NextHearingDate.ToString("dd/MM/yyyy")));
            CreateMap<CaseHistoryResposnse, CaseHistoryViewModel>();
            CreateMap<CaseHistoryData, HistoryDetail>();
            CreateMap<CaseDocumentModel, DocumentAttachmentModel>();
            CreateMap<CaseUploadedDocument, CaseDoc>();
            CreateMap<CaseDetailInfoDto, CaseDetailInfoViewModel>();
            CreateMap<GetCaseInfoDto, GetCaseInfoViewModel>();
            CreateMap<AgainstCaseDetail, AgainstCaseDecisionViewModel>();
            CreateMap<CaseAgainstModel, UpseartAgainstCaseDto>();
            CreateMap<Application.DTOs.CaseDetails.CaseWorkDetail, CaseWorkDetail>();
            CreateMap<CaseHistoryData, HistoryDetail>();
            CreateMap<Application.DTOs.CaseDetails.CaseWork, CaseWork>();
            CreateMap<CaseHistoryData, HistoryDetail>();
            #region CaseUpseartViewModel Model Mapping With Create COmmand & Update COmmand
            CreateMap<CaseUpseartViewModel, CreateCaseCommand>();
            CreateMap<CaseUpseartViewModel, UpdateCaseDetailCommand>();
            CreateMap<CaseAgainstModel, UpseartAgainstCaseDto>();
            CreateMap<UserCaseDetailResponse, CaseUpseartViewModel>();
            CreateMap<UpseartAgainstCaseDto, CaseAgainstModel>();
            #endregion
            CreateMap<UpdateHearingDtViewModel, CaseHearingDto>();
            CreateMap<LinkCaseInfo, LinkCaseInfoViewModel>();
            CreateMap<GetAllClientCachedResponse, ClientViewModel>();
            CreateMap<AssignCaseViewModel, CreateCaseAssignedCommand>();
            CreateMap<AssignCaseViewModel, CaseDeAssignedCommnad>();
        }
    }
}
