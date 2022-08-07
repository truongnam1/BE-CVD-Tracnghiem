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
using Tracnghiem.Services.MExam;
using System.ComponentModel;
using Tracnghiem.Services.MAppUser;
using Tracnghiem.Services.MExamLevel;
using Tracnghiem.Services.MExamStatus;
using Tracnghiem.Services.MGrade;
using Tracnghiem.Services.MImage;
using Tracnghiem.Services.MStatus;
using Tracnghiem.Services.MSubject;
using Tracnghiem.Services.MQuestion;

namespace Tracnghiem.Rpc.exam
{
    [DisplayName("Exam")]
    public class ExamRoute : Root
    {
        public const string Parent = Module + "/exam";
        public const string Master = Module + "/exam/exam-master";
        public const string Detail = Module + "/exam/exam-detail";
        public const string Preview = Module + "/exam/exam-preview";
        private const string Default = Rpc + Module + "/exam";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string PublicGet = Default + "/public-get";

        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string Send = Default + "/send";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string GetDetailExam = Default + "/get-detail-exam";
        public const string SubmitExam = Default + "/submit-exam";
        
        public const string PublicList = Default + "/public-list";
        public const string PublicCount = Default + "/public-count";
        public const string MonthMostTested = Default + "/month-most-tested";
        public const string UploadImage = Default + "/upload-image";


        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListExamLevel = Default + "/filter-list-exam-level";
        public const string FilterListExamStatus = Default + "/filter-list-exam-status";
        public const string FilterListGrade = Default + "/filter-list-grade";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListSubject = Default + "/filter-list-subject";
        public const string FilterListQuestion = Default + "/filter-list-question";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListExamLevel = Default + "/single-list-exam-level";
        public const string SingleListExamStatus = Default + "/single-list-exam-status";
        public const string SingleListGrade = Default + "/single-list-grade";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListSubject = Default + "/single-list-subject";
        public const string SingleListQuestion = Default + "/single-list-question";

        public const string CountQuestion = Default + "/count-question";
        public const string ListQuestion = Default + "/list-question";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { "ExamId", FieldTypeEnum.ID.Id },
            { nameof(ExamFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(ExamFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(ExamFilter.SubjectId), FieldTypeEnum.ID.Id },
            { nameof(ExamFilter.ExamLevelId), FieldTypeEnum.ID.Id },
            { nameof(ExamFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(ExamFilter.CreatorId), FieldTypeEnum.ID.Id },
            { nameof(ExamFilter.GradeId), FieldTypeEnum.ID.Id },
            { nameof(ExamFilter.ExamStatusId), FieldTypeEnum.ID.Id },
            { nameof(ExamFilter.TotalMark), FieldTypeEnum.DECIMAL.Id },
            { nameof(ExamFilter.TotalQuestion), FieldTypeEnum.LONG.Id },
            { nameof(ExamFilter.ImageId), FieldTypeEnum.ID.Id },
            { nameof(ExamFilter.Time), FieldTypeEnum.LONG.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListAppUser,FilterListExamLevel,FilterListExamStatus,FilterListGrade,FilterListStatus,FilterListSubject,FilterListQuestion,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListAppUser, SingleListExamLevel, SingleListExamStatus, SingleListGrade, SingleListStatus, SingleListSubject, SingleListQuestion, 
        };
        private static List<string> CountList = new List<string> { 
            CountQuestion, ListQuestion, 
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
                    Detail, Create, Send, UploadImage, GetDetailExam, SubmitExam
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Sửa", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, Update, Send, UploadImage, GetDetailExam, SubmitExam
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Xoá", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Delete, 
                }.Concat(SingleList).Concat(FilterList) 
            },

            { "Xoá nhiều", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    BulkDelete 
                }.Concat(FilterList) 
            },

            { "Xuất excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Export 
                }.Concat(FilterList) 
            },

            { "Nhập excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    ExportTemplate, Import 
                }.Concat(FilterList) 
            },
        };
    }
}
