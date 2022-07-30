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
using System.Dynamic;
using System.Net;
using NGS.Templater;
using Tracnghiem.Entities;
using Tracnghiem.Services.MExamHistory;
using Tracnghiem.Services.MAppUser;
using Tracnghiem.Services.MExam;

namespace Tracnghiem.Rpc.exam_history
{
    public partial class ExamHistoryController : RpcController
    {
        private IAppUserService AppUserService;
        private IExamService ExamService;
        private IExamHistoryService ExamHistoryService;
        private ICurrentContext CurrentContext;
        public ExamHistoryController(
            IAppUserService AppUserService,
            IExamService ExamService,
            IExamHistoryService ExamHistoryService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.ExamService = ExamService;
            this.ExamHistoryService = ExamHistoryService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ExamHistoryRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ExamHistory_ExamHistoryFilterDTO ExamHistory_ExamHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamHistoryFilter ExamHistoryFilter = ConvertFilterDTOToFilterEntity(ExamHistory_ExamHistoryFilterDTO);
            ExamHistoryFilter = await ExamHistoryService.ToFilter(ExamHistoryFilter);
            int count = await ExamHistoryService.Count(ExamHistoryFilter);
            return count;
        }

        [Route(ExamHistoryRoute.List), HttpPost]
        public async Task<ActionResult<List<ExamHistory_ExamHistoryDTO>>> List([FromBody] ExamHistory_ExamHistoryFilterDTO ExamHistory_ExamHistoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamHistoryFilter ExamHistoryFilter = ConvertFilterDTOToFilterEntity(ExamHistory_ExamHistoryFilterDTO);
            ExamHistoryFilter = await ExamHistoryService.ToFilter(ExamHistoryFilter);
            List<ExamHistory> ExamHistories = await ExamHistoryService.List(ExamHistoryFilter);
            List<ExamHistory_ExamHistoryDTO> ExamHistory_ExamHistoryDTOs = ExamHistories
                .Select(c => new ExamHistory_ExamHistoryDTO(c)).ToList();
            return ExamHistory_ExamHistoryDTOs;
        }

        [Route(ExamHistoryRoute.Get), HttpPost]
        public async Task<ActionResult<ExamHistory_ExamHistoryDTO>> Get([FromBody]ExamHistory_ExamHistoryDTO ExamHistory_ExamHistoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ExamHistory_ExamHistoryDTO.Id))
                return Forbid();

            ExamHistory ExamHistory = await ExamHistoryService.Get(ExamHistory_ExamHistoryDTO.Id);
            return new ExamHistory_ExamHistoryDTO(ExamHistory);
        }

        private async Task<bool> HasPermission(long Id)
        {
            ExamHistoryFilter ExamHistoryFilter = new ExamHistoryFilter();
            ExamHistoryFilter = await ExamHistoryService.ToFilter(ExamHistoryFilter);
            if (Id == 0)
            {

            }
            else
            {
                ExamHistoryFilter.Id = new IdFilter { Equal = Id };
                int count = await ExamHistoryService.Count(ExamHistoryFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ExamHistory ConvertDTOToEntity(ExamHistory_ExamHistoryDTO ExamHistory_ExamHistoryDTO)
        {
            ExamHistory_ExamHistoryDTO.TrimString();
            ExamHistory ExamHistory = new ExamHistory();
            ExamHistory.Id = ExamHistory_ExamHistoryDTO.Id;
            ExamHistory.AppUserId = ExamHistory_ExamHistoryDTO.AppUserId;
            ExamHistory.ExamId = ExamHistory_ExamHistoryDTO.ExamId;
            ExamHistory.Times = ExamHistory_ExamHistoryDTO.Times;
            ExamHistory.CorrectAnswerQuantity = ExamHistory_ExamHistoryDTO.CorrectAnswerQuantity;
            ExamHistory.TotalQuestionQuantity = ExamHistory_ExamHistoryDTO.TotalQuestionQuantity;
            ExamHistory.Mark = ExamHistory_ExamHistoryDTO.Mark;
            ExamHistory.ExamedAt = ExamHistory_ExamHistoryDTO.ExamedAt;
            ExamHistory.AppUser = ExamHistory_ExamHistoryDTO.AppUser == null ? null : new AppUser
            {
                Id = ExamHistory_ExamHistoryDTO.AppUser.Id,
                Username = ExamHistory_ExamHistoryDTO.AppUser.Username,
                DisplayName = ExamHistory_ExamHistoryDTO.AppUser.DisplayName,
                Password = ExamHistory_ExamHistoryDTO.AppUser.Password,
                RefreshToken = ExamHistory_ExamHistoryDTO.AppUser.RefreshToken,
                RoleId = ExamHistory_ExamHistoryDTO.AppUser.RoleId,
                ImageId = ExamHistory_ExamHistoryDTO.AppUser.ImageId,
            };
            ExamHistory.Exam = ExamHistory_ExamHistoryDTO.Exam == null ? null : new Exam
            {
                Id = ExamHistory_ExamHistoryDTO.Exam.Id,
                Code = ExamHistory_ExamHistoryDTO.Exam.Code,
                Name = ExamHistory_ExamHistoryDTO.Exam.Name,
                SubjectId = ExamHistory_ExamHistoryDTO.Exam.SubjectId,
                ExamLevelId = ExamHistory_ExamHistoryDTO.Exam.ExamLevelId,
                StatusId = ExamHistory_ExamHistoryDTO.Exam.StatusId,
                CreatorId = ExamHistory_ExamHistoryDTO.Exam.CreatorId,
                GradeId = ExamHistory_ExamHistoryDTO.Exam.GradeId,
                ExamStatusId = ExamHistory_ExamHistoryDTO.Exam.ExamStatusId,
                TotalMark = ExamHistory_ExamHistoryDTO.Exam.TotalMark,
                TotalQuestion = ExamHistory_ExamHistoryDTO.Exam.TotalQuestion,
                ImageId = ExamHistory_ExamHistoryDTO.Exam.ImageId,
                Time = ExamHistory_ExamHistoryDTO.Exam.Time,
            };
            ExamHistory.BaseLanguage = CurrentContext.Language;
            return ExamHistory;
        }

        private ExamHistoryFilter ConvertFilterDTOToFilterEntity(ExamHistory_ExamHistoryFilterDTO ExamHistory_ExamHistoryFilterDTO)
        {
            ExamHistoryFilter ExamHistoryFilter = new ExamHistoryFilter();
            ExamHistoryFilter.Selects = ExamHistorySelect.ALL;
            ExamHistoryFilter.SearchBy = ExamHistorySearch.ALL;
            ExamHistoryFilter.Skip = ExamHistory_ExamHistoryFilterDTO.Skip;
            ExamHistoryFilter.Take = ExamHistory_ExamHistoryFilterDTO.Take;
            ExamHistoryFilter.OrderBy = ExamHistory_ExamHistoryFilterDTO.OrderBy;
            ExamHistoryFilter.OrderType = ExamHistory_ExamHistoryFilterDTO.OrderType;

            ExamHistoryFilter.Id = ExamHistory_ExamHistoryFilterDTO.Id;
            ExamHistoryFilter.AppUserId = ExamHistory_ExamHistoryFilterDTO.AppUserId;
            ExamHistoryFilter.ExamId = ExamHistory_ExamHistoryFilterDTO.ExamId;
            ExamHistoryFilter.Times = ExamHistory_ExamHistoryFilterDTO.Times;
            ExamHistoryFilter.CorrectAnswerQuantity = ExamHistory_ExamHistoryFilterDTO.CorrectAnswerQuantity;
            ExamHistoryFilter.TotalQuestionQuantity = ExamHistory_ExamHistoryFilterDTO.TotalQuestionQuantity;
            ExamHistoryFilter.Mark = ExamHistory_ExamHistoryFilterDTO.Mark;
            ExamHistoryFilter.ExamedAt = ExamHistory_ExamHistoryFilterDTO.ExamedAt;
            return ExamHistoryFilter;
        }
    }
}

