using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.CacheKeys
{
    public class AppCacheKeys
    {
        public static string CourtMasterKey => "CourtList";
        public static string CourtDistrictKey => "CourtMaster";
        public static string CourtComplexKey => "CourtComplex";       
        public static string ProcHeadKey => "ProcHead";       
        public static string SubProcHeadKey => "SubProcHead";
        public static string WorkKey => "Work";
        public static string SubWorkKey => "SubWork";
        public static string CourtFeeKey => "CourtFee";
    }
}
