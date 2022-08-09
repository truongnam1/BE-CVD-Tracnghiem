using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Enums
{
    public class GenericEnumCustom
    {
        public long Id { get; set; }
        public string Code { get; set; }    
        public string Name { get; set; }
        public string Avatar { get; set; }

    }
    public class SubjectEnum

    {
        public static GenericEnumCustom Economics = new GenericEnumCustom { Id = 1, Code = "Economics", Name = "kinh tế học", Avatar = "https://i.ibb.co/jDPXP6w/china-huangshan-drama-mist.jpg" };
        public static GenericEnumCustom Literature = new GenericEnumCustom{Id = 2, Code = "Literature", Name = "ngữ văn", Avatar = "https://i.ibb.co/jDPXP6w/china-huangshan-drama-mist.jpg" };
        public static GenericEnumCustom Geography = new GenericEnumCustom{Id = 3, Code = "Geography", Name = "địa lý", Avatar = "https://i.ibb.co/jDPXP6w/china-huangshan-drama-mist.jpg" };
        public static GenericEnumCustom History  = new GenericEnumCustom{Id = 4, Code = "History ", Name = "lịch sử", Avatar = "https://i.ibb.co/jDPXP6w/china-huangshan-drama-mist.jpg" };
        public static GenericEnumCustom CivicEducation = new GenericEnumCustom{Id = 5, Code = "CivicEducation", Name = "Giáo dục công dân", Avatar = "https://i.ibb.co/jDPXP6w/china-huangshan-drama-mist.jpg" };
        public static GenericEnumCustom Ethics = new GenericEnumCustom{Id = 6, Code = "Ethics", Name = "Đạo đức", Avatar = "https://i.ibb.co/jDPXP6w/china-huangshan-drama-mist.jpg" };

        public static List<GenericEnumCustom> SubjectEnumList = new List<GenericEnumCustom>
        {
            Economics, Literature, Geography, History, CivicEducation, Ethics
        };
    }
}
