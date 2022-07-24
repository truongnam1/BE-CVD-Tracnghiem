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
using Tracnghiem.Services.MAppUser;
using Tracnghiem.Services.MExam;

namespace Tracnghiem.Rpc.exam_history
{
    public partial class ExamHistoryController 
    {
        [Route(ExamHistoryRoute.FilterListAppUser), HttpPost]
        public async Task<List<ExamHistory_AppUserDTO>> FilterListAppUser([FromBody] ExamHistory_AppUserFilterDTO ExamHistory_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ExamHistory_AppUserFilterDTO.Id;
            AppUserFilter.Username = ExamHistory_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ExamHistory_AppUserFilterDTO.DisplayName;
            AppUserFilter.Password = ExamHistory_AppUserFilterDTO.Password;
            AppUserFilter.RefreshToken = ExamHistory_AppUserFilterDTO.RefreshToken;
            AppUserFilter.RoleId = ExamHistory_AppUserFilterDTO.RoleId;
            AppUserFilter.ImageId = ExamHistory_AppUserFilterDTO.ImageId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ExamHistory_AppUserDTO> ExamHistory_AppUserDTOs = AppUsers
                .Select(x => new ExamHistory_AppUserDTO(x)).ToList();
            return ExamHistory_AppUserDTOs;
        }
        [Route(ExamHistoryRoute.FilterListExam), HttpPost]
        public async Task<List<ExamHistory_ExamDTO>> FilterListExam([FromBody] ExamHistory_ExamFilterDTO ExamHistory_ExamFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamFilter ExamFilter = new ExamFilter();
            ExamFilter.Skip = 0;
            ExamFilter.Take = 20;
            ExamFilter.OrderBy = ExamOrder.Id;
            ExamFilter.OrderType = OrderType.ASC;
            ExamFilter.Selects = ExamSelect.ALL;
            ExamFilter.Id = ExamHistory_ExamFilterDTO.Id;
            ExamFilter.Code = ExamHistory_ExamFilterDTO.Code;
            ExamFilter.Name = ExamHistory_ExamFilterDTO.Name;
            ExamFilter.SubjectId = ExamHistory_ExamFilterDTO.SubjectId;
            ExamFilter.ExamLevelId = ExamHistory_ExamFilterDTO.ExamLevelId;
            ExamFilter.StatusId = ExamHistory_ExamFilterDTO.StatusId;
            ExamFilter.CreatorId = ExamHistory_ExamFilterDTO.CreatorId;
            ExamFilter.GradeId = ExamHistory_ExamFilterDTO.GradeId;
            ExamFilter.ExamStatusId = ExamHistory_ExamFilterDTO.ExamStatusId;
            ExamFilter.TotalMark = ExamHistory_ExamFilterDTO.TotalMark;
            ExamFilter.TotalQuestion = ExamHistory_ExamFilterDTO.TotalQuestion;
            ExamFilter.ImageId = ExamHistory_ExamFilterDTO.ImageId;
            ExamFilter.Time = ExamHistory_ExamFilterDTO.Time;
            ExamFilter.SearchBy = ExamSearch.Code | ExamSearch.Name;
            ExamFilter.Search = ExamHistory_ExamFilterDTO.Search;

            List<Exam> Exams = await ExamService.List(ExamFilter);
            List<ExamHistory_ExamDTO> ExamHistory_ExamDTOs = Exams
                .Select(x => new ExamHistory_ExamDTO(x)).ToList();
            return ExamHistory_ExamDTOs;
        }
    }
}

