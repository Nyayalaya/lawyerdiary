using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuditTrail.Abstrations;

namespace CourtApp.Domain.Entities.Common
{
    [Table("m_state_court_language")]
    public class LanguageEntity:AuditableEntity
    {
        public int StateId { get; set; }
        public List<LangEntity> Languages { get; set; }
    }
    public class LangEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
