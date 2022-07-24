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
using Tracnghiem.Services.MQuestion;
using System.ComponentModel;
using Tracnghiem.Services.MGrade;
using Tracnghiem.Services.MQuestionGroup;
using Tracnghiem.Services.MQuestionType;
using Tracnghiem.Services.MStatus;
using Tracnghiem.Services.MSubject;
using Tracnghiem.Services.MQuestionContent;

namespace Tracnghiem.Rpc.question
{
    [DisplayName("Question")]
    public class QuestionRoute : Root
    {
        public const string Parent = Module + "/question";
        public const string Master = Module + "/question/question-master";
        public const string Detail = Module + "/question/question-detail";
        public const string Preview = Module + "/question/question-preview";
        private const string Default = Rpc + Module + "/question";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListGrade = Default + "/filter-list-grade";
        public const string FilterListQuestionGroup = Default + "/filter-list-question-group";
        public const string FilterListQuestionType = Default + "/filter-list-question-type";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListSubject = Default + "/filter-list-subject";
        public const string FilterListQuestionContent = Default + "/filter-list-question-content";

        public const string SingleListGrade = Default + "/single-list-grade";
        public const string SingleListQuestionGroup = Default + "/single-list-question-group";
        public const string SingleListQuestionType = Default + "/single-list-question-type";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListSubject = Default + "/single-list-subject";
        public const string SingleListQuestionContent = Default + "/single-list-question-content";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { "QuestionId", FieldTypeEnum.ID.Id },
            { nameof(QuestionFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(QuestionFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(QuestionFilter.SubjectId), FieldTypeEnum.ID.Id },
            { nameof(QuestionFilter.QuestionGroupId), FieldTypeEnum.ID.Id },
            { nameof(QuestionFilter.QuestionTypeId), FieldTypeEnum.ID.Id },
            { nameof(QuestionFilter.Content), FieldTypeEnum.STRING.Id },
            { nameof(QuestionFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(QuestionFilter.CreatorId), FieldTypeEnum.ID.Id },
            { nameof(QuestionFilter.GradeId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListGrade,FilterListQuestionGroup,FilterListQuestionType,FilterListStatus,FilterListSubject,FilterListQuestionContent,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListGrade, SingleListQuestionGroup, SingleListQuestionType, SingleListStatus, SingleListSubject, SingleListQuestionContent, 
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
                    Detail, Create, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Sửa", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, Update, 
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
