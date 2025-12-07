using AuditTrail.Abstrations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace CourtApp.Domain.Entities.Common
{
    [Table("m_lang_dict")]
    public class MultiLangDictEntity :AuditableEntity, IDomainLayer
    {
        public string KeyWord { get; set; }
        public List<MultiLangDictItem> MultiLangs { get; set; }
    }
    
    public class MultiLangDictItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
