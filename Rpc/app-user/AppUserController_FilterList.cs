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
using Tracnghiem.Services.MImage;
using Tracnghiem.Services.MRole;
using Tracnghiem.Services.MExamHistory;
using Tracnghiem.Services.MExam;

namespace Tracnghiem.Rpc.app_user
{
    public partial class AppUserController 
    {
        [Route(AppUserRoute.FilterListImage), HttpPost]
        public async Task<List<AppUser_ImageDTO>> FilterListImage([FromBody] AppUser_ImageFilterDTO AppUser_ImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter.Skip = 0;
            ImageFilter.Take = 20;
            ImageFilter.OrderBy = ImageOrder.Id;
            ImageFilter.OrderType = OrderType.ASC;
            ImageFilter.Selects = ImageSelect.ALL;
            ImageFilter.Id = AppUser_ImageFilterDTO.Id;
            ImageFilter.Name = AppUser_ImageFilterDTO.Name;
            ImageFilter.Url = AppUser_ImageFilterDTO.Url;
            ImageFilter.SearchBy = ImageSearch.Name;
            ImageFilter.Search = AppUser_ImageFilterDTO.Search;

            List<Image> Images = await ImageService.List(ImageFilter);
            List<AppUser_ImageDTO> AppUser_ImageDTOs = Images
                .Select(x => new AppUser_ImageDTO(x)).ToList();
            return AppUser_ImageDTOs;
        }
     
        [Route(AppUserRoute.FilterListExamHistory), HttpPost]
        public async Task<List<AppUser_ExamHistoryDTO>> FilterListExamHistory([FromBody] AppUser_ExamHistoryFilterDTO AppUser_ExamHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamHistoryFilter ExamHistoryFilter = new ExamHistoryFilter();
            ExamHistoryFilter.Skip = 0;
            ExamHistoryFilter.Take = 20;
            ExamHistoryFilter.OrderBy = ExamHistoryOrder.Id;
            ExamHistoryFilter.OrderType = OrderType.ASC;
            ExamHistoryFilter.Selects = ExamHistorySelect.ALL;
            ExamHistoryFilter.Id = AppUser_ExamHistoryFilterDTO.Id;
            ExamHistoryFilter.AppUserId = AppUser_ExamHistoryFilterDTO.AppUserId;
            ExamHistoryFilter.ExamId = AppUser_ExamHistoryFilterDTO.ExamId;
            ExamHistoryFilter.Times = AppUser_ExamHistoryFilterDTO.Times;
            ExamHistoryFilter.CorrectAnswerQuantity = AppUser_ExamHistoryFilterDTO.CorrectAnswerQuantity;
            ExamHistoryFilter.TotalQuestionQuantity = AppUser_ExamHistoryFilterDTO.TotalQuestionQuantity;
            ExamHistoryFilter.Mark = AppUser_ExamHistoryFilterDTO.Mark;
            ExamHistoryFilter.ExamedAt = AppUser_ExamHistoryFilterDTO.ExamedAt;

            List<ExamHistory> ExamHistories = await ExamHistoryService.List(ExamHistoryFilter);
            List<AppUser_ExamHistoryDTO> AppUser_ExamHistoryDTOs = ExamHistories
                .Select(x => new AppUser_ExamHistoryDTO(x)).ToList();
            return AppUser_ExamHistoryDTOs;
        }
        [Route(AppUserRoute.FilterListExam), HttpPost]
        public async Task<List<AppUser_ExamDTO>> FilterListExam([FromBody] AppUser_ExamFilterDTO AppUser_ExamFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamFilter ExamFilter = new ExamFilter();
            ExamFilter.Skip = 0;
            ExamFilter.Take = 20;
            ExamFilter.OrderBy = ExamOrder.Id;
            ExamFilter.OrderType = OrderType.ASC;
            ExamFilter.Selects = ExamSelect.ALL;
            ExamFilter.Id = AppUser_ExamFilterDTO.Id;
            ExamFilter.Code = AppUser_ExamFilterDTO.Code;
            ExamFilter.Name = AppUser_ExamFilterDTO.Name;
            ExamFilter.SubjectId = AppUser_ExamFilterDTO.SubjectId;
            ExamFilter.ExamLevelId = AppUser_ExamFilterDTO.ExamLevelId;
            ExamFilter.StatusId = AppUser_ExamFilterDTO.StatusId;
            ExamFilter.CreatorId = AppUser_ExamFilterDTO.CreatorId;
            ExamFilter.GradeId = AppUser_ExamFilterDTO.GradeId;
            ExamFilter.ExamStatusId = AppUser_ExamFilterDTO.ExamStatusId;
            ExamFilter.TotalMark = AppUser_ExamFilterDTO.TotalMark;
            ExamFilter.TotalQuestion = AppUser_ExamFilterDTO.TotalQuestion;
            ExamFilter.ImageId = AppUser_ExamFilterDTO.ImageId;
            ExamFilter.Time = AppUser_ExamFilterDTO.Time;
            ExamFilter.SearchBy = ExamSearch.Code | ExamSearch.Name;
            ExamFilter.Search = AppUser_ExamFilterDTO.Search;

            List<Exam> Exams = await ExamService.List(ExamFilter);
            List<AppUser_ExamDTO> AppUser_ExamDTOs = Exams
                .Select(x => new AppUser_ExamDTO(x)).ToList();
            return AppUser_ExamDTOs;
        }
    }
}

