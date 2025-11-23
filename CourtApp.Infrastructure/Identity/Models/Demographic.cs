using AuditTrail.Abstrations;
using CourtApp.Domain.Entities.LawyerDiary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace CourtApp.Infrastructure.Identity.Models
{
    [Table("demographic", Schema = "Identity")]
    public class Demographic : AuditableEntity
    {

        /// <summary>
        /// Complete contact information of the lawyer.
        /// </summary>
        public ContactInfo ContactInfo { get; set; }

        /// <summary>
        /// Lawyer Address Information
        /// </summary>
        public AddressInfo AddressInfo { get; set; }

        /// <summary>
        /// Work location information of lawyer
        /// </summary>
        public WorkLocation WorkLocInfo { get; set; }

        /// <summary>
        /// Complete professional information of laywer.
        /// </summary>
        public ProfessionalInfo ProfessionalInfo { get; set; }

        /// <summary>
        /// Navigational property for the related user.
        /// </summary>
        public ApplicationUser User { get; set; }
    }

    public class WorkLocation
    {
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public Guid CourtTypeId { get; set; }
        public Guid CourtId { get; set; }
        public string Address { get; set; }
    }

    public class ContactInfo
    {
        public string O_Phone { get; set; }
        public string R_Phone { get; set; }
        public string P_Email { get; set; }
        public string O_Email { get; set; }
    }

    public class ProfessionalInfo
    {

        public string EnrollmentNo { get; set; }
        /// <summary>
        /// Bar associate number of a lawyer
        /// </summary>
        public string BarAssociationNumber { get; set; }

        /// <summary>
        /// Practice License date of a lawyer
        /// </summary>
        public DateTime PracticeLicenseDate { get; set; }

        /// <summary>
        /// Year of practice of a lawyer when started.
        /// </summary>
        public int PracticeSince { get; set; }

        /// <summary>
        /// Lawyer's all the specility in which they practiced.
        /// </summary>
        public List<SpecializationEntity> Specializations { get; set; }
    }
}
