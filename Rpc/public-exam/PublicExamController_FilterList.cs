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

namespace Tracnghiem.Rpc.public_exam
{
    public partial class PublicExamController 
    {
        [Route(PublicExamRoute.FilterListAppUser), HttpPost]
        public async Task<List<PublicExam_AppUserDTO>> FilterListAppUser([FromBody] PublicExam_AppUserFilterDTO PublicExam_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = PublicExam_AppUserFilterDTO.Id;
            AppUserFilter.Username = PublicExam_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = PublicExam_AppUserFilterDTO.DisplayName;
            AppUserFilter.Password = PublicExam_AppUserFilterDTO.Password;
            AppUserFilter.RefreshToken = PublicExam_AppUserFilterDTO.RefreshToken;
            AppUserFilter.RoleId = PublicExam_AppUserFilterDTO.RoleId;
            AppUserFilter.ImageId = PublicExam_AppUserFilterDTO.ImageId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<PublicExam_AppUserDTO> PublicExam_AppUserDTOs = AppUsers
                .Select(x => new PublicExam_AppUserDTO(x)).ToList();
            return PublicExam_AppUserDTOs;
        }
        [Route(PublicExamRoute.FilterListExamLevel), HttpPost]
        
        [AllowAnonymous]
        public async Task<List<PublicExam_ExamLevelDTO>> FilterListExamLevel([FromBody] PublicExam_ExamLevelFilterDTO PublicExam_ExamLevelFilterDTO)
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
            List<PublicExam_ExamLevelDTO> PublicExam_ExamLevelDTOs = ExamLevels
                .Select(x => new PublicExam_ExamLevelDTO(x)).ToList();
            return PublicExam_ExamLevelDTOs;
        }
        [Route(PublicExamRoute.FilterListExamStatus), HttpPost]
        public async Task<List<PublicExam_ExamStatusDTO>> FilterListExamStatus([FromBody] PublicExam_ExamStatusFilterDTO PublicExam_ExamStatusFilterDTO)
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
            List<PublicExam_ExamStatusDTO> PublicExam_ExamStatusDTOs = ExamStatuses
                .Select(x => new PublicExam_ExamStatusDTO(x)).ToList();
            return PublicExam_ExamStatusDTOs;
        }

        [AllowAnonymous]
        [Route(PublicExamRoute.FilterListGrade), HttpPost]
        public async Task<List<PublicExam_GradeDTO>> FilterListGrade([FromBody] PublicExam_GradeFilterDTO PublicExam_GradeFilterDTO)
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
            List<PublicExam_GradeDTO> PublicExam_GradeDTOs = Grades
                .Select(x => new PublicExam_GradeDTO(x)).ToList();
            return PublicExam_GradeDTOs;
        }
        [Route(PublicExamRoute.FilterListStatus), HttpPost]
        public async Task<List<PublicExam_StatusDTO>> FilterListStatus([FromBody] PublicExam_StatusFilterDTO PublicExam_StatusFilterDTO)
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
            List<PublicExam_StatusDTO> PublicExam_StatusDTOs = Statuses
                .Select(x => new PublicExam_StatusDTO(x)).ToList();
            return PublicExam_StatusDTOs;
        }

        [AllowAnonymous]
        [Route(PublicExamRoute.FilterListSubject), HttpPost]
        public async Task<List<PublicExam_SubjectDTO>> FilterListSubject([FromBody] PublicExam_SubjectFilterDTO PublicExam_SubjectFilterDTO)
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
            List<PublicExam_SubjectDTO> PublicExam_SubjectDTOs = Subjects
                .Select(x => new PublicExam_SubjectDTO(x)).ToList();
            return PublicExam_SubjectDTOs;
        }
        [Route(PublicExamRoute.FilterListQuestion), HttpPost]
        public async Task<List<PublicExam_QuestionDTO>> FilterListQuestion([FromBody] PublicExam_QuestionFilterDTO PublicExam_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionFilter QuestionFilter = new QuestionFilter();
            QuestionFilter.Skip = 0;
            QuestionFilter.Take = 20;
            QuestionFilter.OrderBy = QuestionOrder.Id;
            QuestionFilter.OrderType = OrderType.ASC;
            QuestionFilter.Selects = QuestionSelect.ALL;
            QuestionFilter.Id = PublicExam_QuestionFilterDTO.Id;
            QuestionFilter.Code = PublicExam_QuestionFilterDTO.Code;
            QuestionFilter.Name = PublicExam_QuestionFilterDTO.Name;
            QuestionFilter.SubjectId = PublicExam_QuestionFilterDTO.SubjectId;
            QuestionFilter.QuestionGroupId = PublicExam_QuestionFilterDTO.QuestionGroupId;
            QuestionFilter.QuestionTypeId = PublicExam_QuestionFilterDTO.QuestionTypeId;
            QuestionFilter.Content = PublicExam_QuestionFilterDTO.Content;
            QuestionFilter.StatusId = PublicExam_QuestionFilterDTO.StatusId;
            QuestionFilter.CreatorId = PublicExam_QuestionFilterDTO.CreatorId;
            QuestionFilter.GradeId = PublicExam_QuestionFilterDTO.GradeId;
            QuestionFilter.SearchBy = QuestionSearch.Code | QuestionSearch.Name;
            QuestionFilter.Search = PublicExam_QuestionFilterDTO.Search;

            List<Question> Questions = await QuestionService.List(QuestionFilter);
            List<PublicExam_QuestionDTO> PublicExam_QuestionDTOs = Questions
                .Select(x => new PublicExam_QuestionDTO(x)).ToList();
            return PublicExam_QuestionDTOs;
        }
    }
}

