using System.Collections.Generic;
using TrueSight.Common;

namespace Tracnghiem.Enums
{
    public class SiteEnum
    {
        public static GenericEnum LANDING => new GenericEnum (Id : 100, Name : "Landing Page", Code : "/landing-page");
        public static GenericEnum Tracnghiem => new GenericEnum (Id : 1, Name : "Tracnghiem", Code : "/tracnghiem/");


        public static List<GenericEnum> SiteEnumList = new List<GenericEnum>
        {
            LANDING, Tracnghiem
        };
    }
}
