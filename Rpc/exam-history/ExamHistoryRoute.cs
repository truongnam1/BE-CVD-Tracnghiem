using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracnghiem.Common;
using Tracnghiem.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using Tracnghiem.Entities;
using Tracnghiem.Services.MExamHistory;
using System.ComponentModel;
using Tracnghiem.Services.MAppUser;
using Tracnghiem.Services.MExam;

namespace Tracnghiem.Rpc.exam_history
{
    [DisplayName("ExamHistory")]
    public class ExamHistoryRoute : Root
    {
        public const string Parent = Module + "/exam-history";
        public const string Master = Module + "/exam-history/exam-history-master";
        public const string Detail = Module + "/exam-history/exam-history-detail";
        public const string Preview = Module + "/exam-history/exam-history-preview";
        private const string Default = Rpc + Module + "/exam-history";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListExam = Default + "/filter-list-exam";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListExam = Default + "/single-list-exam";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { "ExamHistoryId", FieldTypeEnum.ID.Id },
            { nameof(ExamHistoryFilter.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(ExamHistoryFilter.ExamId), FieldTypeEnum.ID.Id },
            { nameof(ExamHistoryFilter.Times), FieldTypeEnum.LONG.Id },
            { nameof(ExamHistoryFilter.CorrectAnswerQuantity), FieldTypeEnum.LONG.Id },
            { nameof(ExamHistoryFilter.TotalQuestionQuantity), FieldTypeEnum.LONG.Id },
            { nameof(ExamHistoryFilter.Mark), FieldTypeEnum.DECIMAL.Id },
            { nameof(ExamHistoryFilter.ExamedAt), FieldTypeEnum.DATE.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListAppUser,FilterListExam,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListAppUser, SingleListExam, 
        };
        private static List<string> CountList = new List<string> { 
            
        };
        
        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List,
                    Get,  
                }.Concat(FilterList)
            },
            { "Thêm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Sửa", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Xoá", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                }.Concat(SingleList).Concat(FilterList) 
            },

            { "Xoá nhiều", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                }.Concat(FilterList) 
            },

            { "Xuất excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get
                }.Concat(FilterList) 
            },

            { "Nhập excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get
                }.Concat(FilterList) 
            },
        };
    }
}
