using System.Collections.Generic;

namespace CourtApp.Application.DTOs.Common
{
    public class MultiLangDictDto
    {
        public string KeyWord { get; set; }
        public List<MultiLangDictItemDto> MultiLangs { get; set; }
    }
}
