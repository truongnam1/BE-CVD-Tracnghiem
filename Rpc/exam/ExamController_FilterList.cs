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
    public partial class ExamController 
    {
        [Route(ExamRoute.FilterListAppUser), HttpPost]
        public async Task<List<Exam_AppUserDTO>> FilterListAppUser([FromBody] Exam_AppUserFilterDTO Exam_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Exam_AppUserFilterDTO.Id;
            AppUserFilter.Username = Exam_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = Exam_AppUserFilterDTO.DisplayName;
            AppUserFilter.Password = Exam_AppUserFilterDTO.Password;
            AppUserFilter.RefreshToken = Exam_AppUserFilterDTO.RefreshToken;
            AppUserFilter.RoleId = Exam_AppUserFilterDTO.RoleId;
            AppUserFilter.ImageId = Exam_AppUserFilterDTO.ImageId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Exam_AppUserDTO> Exam_AppUserDTOs = AppUsers
                .Select(x => new Exam_AppUserDTO(x)).ToList();
            return Exam_AppUserDTOs;
        }
        [Route(ExamRoute.FilterListExamLevel), HttpPost]
        
        [AllowAnonymous]
        public async Task<List<Exam_ExamLevelDTO>> FilterListExamLevel([FromBody] Exam_ExamLevelFilterDTO Exam_ExamLevelFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamLevelFilter ExamLevelFilter = new ExamLevelFilter();
            ExamLevelFilter.Skip = 0;
            ExamLevelFilter.Take = int.MaxValue;
            ExamLevelFilter.Take = 20;
            ExamLevelFilter.OrderBy = ExamLevelOrder.Id;
            ExamLevelFilter.OrderType = OrderType.ASC;
            ExamLevelFilter.Selects = ExamLevelSelect.ALL;

            List<ExamLevel> ExamLevels = await ExamLevelService.List(ExamLevelFilter);
            List<Exam_ExamLevelDTO> Exam_ExamLevelDTOs = ExamLevels
                .Select(x => new Exam_ExamLevelDTO(x)).ToList();
            return Exam_ExamLevelDTOs;
        }
        [Route(ExamRoute.FilterListExamStatus), HttpPost]
        public async Task<List<Exam_ExamStatusDTO>> FilterListExamStatus([FromBody] Exam_ExamStatusFilterDTO Exam_ExamStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamStatusFilter ExamStatusFilter = new ExamStatusFilter();
            ExamStatusFilter.Skip = 0;
            ExamStatusFilter.Take = int.MaxValue;
            ExamStatusFilter.Take = 20;
            ExamStatusFilter.OrderBy = ExamStatusOrder.Id;
            ExamStatusFilter.OrderType = OrderType.ASC;
            ExamStatusFilter.Selects = ExamStatusSelect.ALL;

            List<ExamStatus> ExamStatuses = await ExamStatusService.List(ExamStatusFilter);
            List<Exam_ExamStatusDTO> Exam_ExamStatusDTOs = ExamStatuses
                .Select(x => new Exam_ExamStatusDTO(x)).ToList();
            return Exam_ExamStatusDTOs;
        }

        [AllowAnonymous]
        [Route(ExamRoute.FilterListGrade), HttpPost]
        public async Task<List<Exam_GradeDTO>> FilterListGrade([FromBody] Exam_GradeFilterDTO Exam_GradeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GradeFilter GradeFilter = new GradeFilter();
            GradeFilter.Skip = 0;
            GradeFilter.Take = int.MaxValue;
            GradeFilter.Take = 20;
            GradeFilter.OrderBy = GradeOrder.Id;
            GradeFilter.OrderType = OrderType.ASC;
            GradeFilter.Selects = GradeSelect.ALL;

            List<Grade> Grades = await GradeService.List(GradeFilter);
            List<Exam_GradeDTO> Exam_GradeDTOs = Grades
                .Select(x => new Exam_GradeDTO(x)).ToList();
            return Exam_GradeDTOs;
        }
        [Route(ExamRoute.FilterListStatus), HttpPost]
        public async Task<List<Exam_StatusDTO>> FilterListStatus([FromBody] Exam_StatusFilterDTO Exam_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Exam_StatusDTO> Exam_StatusDTOs = Statuses
                .Select(x => new Exam_StatusDTO(x)).ToList();
            return Exam_StatusDTOs;
        }

        [AllowAnonymous]
        [Route(ExamRoute.FilterListSubject), HttpPost]
        public async Task<List<Exam_SubjectDTO>> FilterListSubject([FromBody] Exam_SubjectFilterDTO Exam_SubjectFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SubjectFilter SubjectFilter = new SubjectFilter();
            SubjectFilter.Skip = 0;
            SubjectFilter.Take = int.MaxValue;
            SubjectFilter.Take = 20;
            SubjectFilter.OrderBy = SubjectOrder.Id;
            SubjectFilter.OrderType = OrderType.ASC;
            SubjectFilter.Selects = SubjectSelect.ALL;

            List<Subject> Subjects = await SubjectService.List(SubjectFilter);
            List<Exam_SubjectDTO> Exam_SubjectDTOs = Subjects
                .Select(x => new Exam_SubjectDTO(x)).ToList();
            return Exam_SubjectDTOs;
        }
        [Route(ExamRoute.FilterListQuestion), HttpPost]
        public async Task<List<Exam_QuestionDTO>> FilterListQuestion([FromBody] Exam_QuestionFilterDTO Exam_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionFilter QuestionFilter = new QuestionFilter();
            QuestionFilter.Skip = 0;
            QuestionFilter.Take = 20;
            QuestionFilter.OrderBy = QuestionOrder.Id;
            QuestionFilter.OrderType = OrderType.ASC;
            QuestionFilter.Selects = QuestionSelect.ALL;
            QuestionFilter.Id = Exam_QuestionFilterDTO.Id;
            QuestionFilter.Code = Exam_QuestionFilterDTO.Code;
            QuestionFilter.Name = Exam_QuestionFilterDTO.Name;
            QuestionFilter.SubjectId = Exam_QuestionFilterDTO.SubjectId;
            QuestionFilter.QuestionGroupId = Exam_QuestionFilterDTO.QuestionGroupId;
            QuestionFilter.QuestionTypeId = Exam_QuestionFilterDTO.QuestionTypeId;
            QuestionFilter.Content = Exam_QuestionFilterDTO.Content;
            QuestionFilter.StatusId = Exam_QuestionFilterDTO.StatusId;
            QuestionFilter.CreatorId = Exam_QuestionFilterDTO.CreatorId;
            QuestionFilter.GradeId = Exam_QuestionFilterDTO.GradeId;
            QuestionFilter.SearchBy = QuestionSearch.Code | QuestionSearch.Name;
            QuestionFilter.Search = Exam_QuestionFilterDTO.Search;

            List<Question> Questions = await QuestionService.List(QuestionFilter);
            List<Exam_QuestionDTO> Exam_QuestionDTOs = Questions
                .Select(x => new Exam_QuestionDTO(x)).ToList();
            return Exam_QuestionDTOs;
        }
    }
}

