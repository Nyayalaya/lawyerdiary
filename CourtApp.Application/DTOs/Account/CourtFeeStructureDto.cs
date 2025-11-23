using System;

namespace CourtApp.Application.DTOs.Account
{
    public class CourtFeeStructureDto
    {
        public Guid Id { get; set; }   
        public string StateName { get; set; }        
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double Rate { get; set; }
        public double FixAmount { get; set; }
    }
}
