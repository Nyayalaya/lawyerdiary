using AuditTrail.Abstrations;
using CourtApp.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourtApp.Domain.Entities.Advocate
{
    [Table("m_act", Schema = "ad")]
    public class ActEntity : AuditableEntity
    {
        // Core Act Information
        [Required]
        public string ActCategory { get; set; }
        
        public required int ActNumber { get; set; }
        public required int SubActNumber { get; set; }
        public int ActYear { get; set; }
        
        // Assent Information
        public string AssentBy { get; set; }
        public DateTime? AssentDate { get; set; }
        
        // Act Details
        [Required]
        [StringLength(500)]
        public string ActName { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Nature { get; set; }
        
        // Gazette Information
        public Guid GazetteTypeId { get; set; }
        public DateTime? GazetteDate { get; set; }
        public int? PageNo { get; set; }
        public DateTime? PublishedGazetteDate { get; set; }
        
        // Enforcement Information
        public string ComeInforce { get; set; }
        
        // Foreign Keys
        [Required]
        public Guid SubjectId { get; set; }
        
        [Required]
        public Guid ActTypeId { get; set; }
        
        [Required]
        public Guid PartId { get; set; }
        
        // Navigation Properties
        public virtual SubjectEntity Subject { get; set; }
        public virtual ActTypeEntity ActType { get; set; }
        public virtual PartEntity Part { get; set; }
        public virtual GazetteTypeEntity GazetteType { get; set; }
        
        // Collections
        public virtual ICollection<ActAmendedEntity> AmendedActs { get; set; } = new List<ActAmendedEntity>();
        public virtual ICollection<ActRepealedEntity> RepealedActs { get; set; } = new List<ActRepealedEntity>();
        public virtual ICollection<ActBookEntity> ActBooks { get; set; } = new List<ActBookEntity>();
    }
}
