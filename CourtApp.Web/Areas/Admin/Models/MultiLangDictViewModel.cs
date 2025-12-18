using System;
using System.Collections.Generic;

namespace CourtApp.Web.Areas.Admin.Models
{
    public class MultiLangDictViewModel
    {
        public Guid Id { get; set; }
        public string KeyWord { get; set; }
        public Dictionary<string, string> Translations { get; set; } = new();
    }
}
