using System.Collections.Generic;
using TrueSight.Common;

namespace Tracnghiem.Enums
{
    public class SiteEnum
    {
        public static GenericEnum LANDING => new GenericEnum (Id : 100, Name : "Landing Page", Code : "/landing-page");
        public static GenericEnum PORTAL => new GenericEnum (Id : 1, Name : "PORTAL", Code : "/portal/");
        public static GenericEnum DMS => new GenericEnum (Id : 2, Name : "DMS", Code : "/dms/");
        public static GenericEnum CRM => new GenericEnum (Id : 3, Name : "CRM", Code : "/crm/" );
        public static GenericEnum REPORT => new GenericEnum (Id : 4, Name : "REPORT", Code : "/report/");
        public static GenericEnum MDM => new GenericEnum (Id : 5, Name : "MDM", Code : "/mdm/");
        public static GenericEnum AMS => new GenericEnum (Id : 6, Name : "AMS", Code : "/ams/");
        public static GenericEnum CMS_EXPORT => new GenericEnum (Id : 7, Name : "CMS Export", Code : "/cms-export-admin/");
        public static GenericEnum PPF => new GenericEnum (Id : 8, Name : "PPF", Code : "/ppf/");
        public static GenericEnum SAMS => new GenericEnum (Id : 9, Name : "SAMS", Code : "/sams/");
        public static GenericEnum EOS => new GenericEnum (Id : 10, Name : "EOS", Code : "/eos/");
        public static GenericEnum ORDER_HUB => new GenericEnum (Id : 11, Name : "Order Hub", Code : "/order-hub/");
        public static GenericEnum MY_COMPANY => new GenericEnum (Id : 12, Name : "OffiCheck", Code : "/my-company/");
        public static GenericEnum PMS => new GenericEnum (Id : 13, Name : "PMS", Code : "/pms/");

        public static List<GenericEnum> SiteEnumList = new List<GenericEnum>
        {
            LANDING, PORTAL, REPORT, DMS, CRM, MDM, AMS, CMS_EXPORT, PPF, SAMS, EOS, ORDER_HUB, MY_COMPANY, PMS
        };
    }
}
