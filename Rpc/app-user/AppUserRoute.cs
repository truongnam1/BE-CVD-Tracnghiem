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
using Tracnghiem.Services.MAppUser;
using System.ComponentModel;
using Tracnghiem.Services.MImage;
using Tracnghiem.Services.MRole;
using Tracnghiem.Services.MExamHistory;
using Tracnghiem.Services.MExam;

namespace Tracnghiem.Rpc.app_user
{
    [DisplayName("AppUser")]
    public class AppUserRoute : Root
    {
        public const string Parent = Module + "/app-user";
        public const string Master = Module + "/app-user/app-user-master";
        public const string Detail = Module + "/app-user/app-user-detail";
        public const string Preview = Module + "/app-user/app-user-preview";
        private const string Default = Rpc + Module + "/app-user";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string UserCreate = Default + "/user-create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListImage = Default + "/filter-list-image";
        public const string FilterListRole = Default + "/filter-list-role";
        public const string FilterListExamHistory = Default + "/filter-list-exam-history";
        public const string FilterListExam = Default + "/filter-list-exam";

        public const string SingleListImage = Default + "/single-list-image";
        public const string SingleListRole = Default + "/single-list-role";
        public const string SingleListExamHistory = Default + "/single-list-exam-history";
        public const string SingleListExam = Default + "/single-list-exam";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { "AppUserId", FieldTypeEnum.ID.Id },
            { nameof(AppUserFilter.Username), FieldTypeEnum.STRING.Id },
            { nameof(AppUserFilter.DisplayName), FieldTypeEnum.STRING.Id },
            { nameof(AppUserFilter.Password), FieldTypeEnum.STRING.Id },
            { nameof(AppUserFilter.RefreshToken), FieldTypeEnum.STRING.Id },
            { nameof(AppUserFilter.RoleId), FieldTypeEnum.ID.Id },
            { nameof(AppUserFilter.ImageId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListImage,FilterListRole,FilterListExamHistory,FilterListExam,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListImage, SingleListRole, SingleListExamHistory, SingleListExam, 
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
