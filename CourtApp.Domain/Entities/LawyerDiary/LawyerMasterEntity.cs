using AuditTrail.Abstrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourtApp.Domain.Entities.LawyerDiary
{
    [Table("m_lawyer", Schema = "common")]
    public class LawyerMasterEntity : AuditableEntity
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EnrollNumber { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? Dob { get; set; }
        public string Gender { get; set; }
        public string RelPerson { get; set; }
        public string Relegion { get; set; }
        public string Caste { get; set; }
        public string ProfileImgPath { get; set; }

        public ICollection<OppositCouncilEntity> OppositeCounsels { get; set; } = new List<OppositCouncilEntity>();
    }
}
