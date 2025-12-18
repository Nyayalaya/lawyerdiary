using AutoMapper;
using CourtApp.Application.DTOs.Common;
using CourtApp.Domain.Entities.Common;
using System.Linq;
namespace CourtApp.Application.Mappings
{
    public class MultiLangDictProfileMapping:Profile
    {
        public MultiLangDictProfileMapping()
        {
            // Entity → DTO
            CreateMap<MultiLangDictEntity, MultiLangDictDto>()
                .ForMember(dest => dest.KeyWord, opt => opt.MapFrom(src => src.KeyWord))
                .ForMember(dest => dest.MultiLangs, opt => opt.MapFrom(src => src.MultiLangs));

            // DTO → Entity
            CreateMap<MultiLangDictDto, MultiLangDictEntity>()
                .ForMember(dest => dest.KeyWord, opt => opt.MapFrom(src => src.KeyWord))
                .ForMember(dest => dest.MultiLangs, opt => opt.MapFrom(src => src.MultiLangs));

            // Item mapping
            CreateMap<MultiLangDictItem, MultiLangDictItemDto>().ReverseMap();

            

        }
    }
}
