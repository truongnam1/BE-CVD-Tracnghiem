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

namespace Tracnghiem.Rpc.subject
{
    [DisplayName("Subject")]
    public class SubjectRoute : Root
    {
        public const string Parent = Module + "/subject";
        public const string Master = Module + "/subject/subject-master";
        public const string Detail = Module + "/subject/subject-detail";
        public const string Preview = Module + "/subject/subject-preview";
        private const string Default = Rpc + Module + "/subject";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";


        //public static Dictionary<string, long> Filters = new Dictionary<string, long>
        //{
        //    { "SubjectId", FieldTypeEnum.ID.Id },
        //    { nameof(ExamFilter.Code), FieldTypeEnum.STRING.Id },
        //    { nameof(ExamFilter.Name), FieldTypeEnum.STRING.Id },
        //    { nameof(ExamFilter.SubjectId), FieldTypeEnum.ID.Id },
        //    { nameof(ExamFilter.ExamLevelId), FieldTypeEnum.ID.Id },
        //    { nameof(ExamFilter.StatusId), FieldTypeEnum.ID.Id },
        //    { nameof(ExamFilter.CreatorId), FieldTypeEnum.ID.Id },
        //    { nameof(ExamFilter.GradeId), FieldTypeEnum.ID.Id },
        //    { nameof(ExamFilter.ExamStatusId), FieldTypeEnum.ID.Id },
        //    { nameof(ExamFilter.TotalMark), FieldTypeEnum.DECIMAL.Id },
        //    { nameof(ExamFilter.TotalQuestion), FieldTypeEnum.LONG.Id },
        //    { nameof(ExamFilter.ImageId), FieldTypeEnum.ID.Id },
        //    { nameof(ExamFilter.Time), FieldTypeEnum.LONG.Id },
        //};

        //private static List<string> FilterList = new List<string> { 
        //    FilterListAppUser,FilterListExamLevel,FilterListExamStatus,FilterListGrade,FilterListStatus,FilterListSubject,FilterListQuestion,
        //};
        //private static List<string> SingleList = new List<string> { 
        //    SingleListAppUser, SingleListExamLevel, SingleListExamStatus, SingleListGrade, SingleListStatus, SingleListSubject, SingleListQuestion, 
        //};
        //private static List<string> CountList = new List<string> { 
        //    CountQuestion, ListQuestion, 
        //};
        
        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List,
                    Get,  
                }
            },
        };
    }
}
