using System;

namespace CourtApp.Web.Areas.Admin.Models
{
    public class WorkInfoViewModel
    {
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid CourtId { get; set; }
        public string Address { get; set; }
    }
}
