using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Web.Areas.Admin.Models;
using System.Linq;

namespace CourtApp.Web.Areas.Admin.Mappings
{
    public class MuliLanMapping:Profile
    {
        public MuliLanMapping()
        {
            // DTO -> ViewModel with dictionary conversion
            CreateMap<MultiLangDictDto, MultiLangDictViewModel>()
                .ForMember(dest => dest.Translations, opt => opt.MapFrom(src =>
                    src.MultiLangs.ToDictionary(m => m.Key, m => m.Value)
                ))
                .ReverseMap()
                .ForMember(dest => dest.MultiLangs, opt => opt.MapFrom(src =>
                    src.Translations.Select(kvp => new MultiLangDictItemDto
                    {
                        Key = kvp.Key,
                        Value = kvp.Value
                    }).ToList()
                ));
        }
    }
}
