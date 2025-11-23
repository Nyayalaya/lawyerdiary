using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.CacheKeys
{
    public class BillingDetailCacheKeys
    {
        public static string ListKey => "LaywerBillingDetail";
        public static string GetKey(Guid Id) => $"LaywerBillingDetail-{Id}";
        public static string GetDetailsKey(Guid Id) => $"LaywerBillingDetail-{Id}";
    }
}
