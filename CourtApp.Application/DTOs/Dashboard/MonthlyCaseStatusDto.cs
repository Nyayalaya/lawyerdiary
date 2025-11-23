using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.DTOs.Dashboard
{
    public class MonthlyCaseStatusDto
    {
        public int Year { get; set; }
        public string Month { get; set; } // e.g. "Jan", "Feb"
        public int Filed { get; set; }
        public int Disposed { get; set; }
    }
}
