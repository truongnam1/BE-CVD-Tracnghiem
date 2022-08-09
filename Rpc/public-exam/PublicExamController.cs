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
using Tracnghiem.Services.MExam;
using Tracnghiem.Services.MAppUser;
using Tracnghiem.Services.MExamLevel;
using Tracnghiem.Services.MExamStatus;
using Tracnghiem.Services.MGrade;
using Tracnghiem.Services.MImage;
using Tracnghiem.Services.MStatus;
using Tracnghiem.Services.MSubject;
using Tracnghiem.Services.MQuestion;
using Tracnghiem.Enums;
using Tracnghiem.Services.MDoingExam;

namespace Tracnghiem.Rpc.public_exam
{
    public partial class PublicExamController : RpcController
    {
        private IAppUserService AppUserService;
        private IExamLevelService ExamLevelService;
        private IExamStatusService ExamStatusService;
        private IGradeService GradeService;
        private IImageService ImageService;
        private IStatusService StatusService;
        private ISubjectService SubjectService;
        private IQuestionService QuestionService;
        private IExamService ExamService;
        private IDoingExamService DoingExamService;
        private ICurrentContext CurrentContext;
        public PublicExamController(
            IAppUserService AppUserService,
            IExamLevelService ExamLevelService,
            IExamStatusService ExamStatusService,
            IGradeService GradeService,
            IImageService ImageService,
            IStatusService StatusService,
            ISubjectService SubjectService,
            IQuestionService QuestionService,
            IExamService ExamService,
            IDoingExamService DoingExamService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.ExamLevelService = ExamLevelService;
            this.ExamStatusService = ExamStatusService;
            this.GradeService = GradeService;
            this.ImageService = ImageService;
            this.StatusService = StatusService;
            this.SubjectService = SubjectService;
            this.QuestionService = QuestionService;
            this.ExamService = ExamService;
            this.DoingExamService = DoingExamService;
            this.CurrentContext = CurrentContext;
        }

        [AllowAnonymous]
        [Route(PublicExamRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] PublicExam_PublicExamFilterDTO Exam_ExamFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamFilter ExamFilter = ConvertFilterDTOToFilterEntity(Exam_ExamFilterDTO);
            ExamFilter.ExamStatusId = new IdFilter { Equal = ExamStatusEnum.Public.Id };
            ExamFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ExamFilter = await ExamService.ToFilter(ExamFilter);
            int count = await ExamService.Count(ExamFilter);
            return count;
        }

        [AllowAnonymous]
        [Route(PublicExamRoute.List), HttpPost]
        public async Task<ActionResult<List<PublicExam_PublicExamDTO>>> List([FromBody] PublicExam_PublicExamFilterDTO Exam_ExamFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ExamFilter ExamFilter = ConvertFilterDTOToFilterEntity(Exam_ExamFilterDTO);
            ExamFilter.ExamStatusId = new IdFilter { Equal = ExamStatusEnum.Public.Id };
            ExamFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            ExamFilter = await ExamService.ToFilter(ExamFilter);
            List<Exam> Exams = await ExamService.List(ExamFilter);
            List<PublicExam_PublicExamDTO> Exam_ExamDTOs = Exams
                .Select(c => new PublicExam_PublicExamDTO(c)).ToList();
            return Exam_ExamDTOs;
        }

        [AllowAnonymous]
        [Route(PublicExamRoute.MonthMostTested), HttpPost]
        public async Task<ActionResult<List<PublicExam_PublicExamDTO>>> MonthMostTested()
        {
            List<Exam> Exams = await ExamService.MonthMostTested();
            List<PublicExam_PublicExamDTO> Exam_ExamDTOs = Exams
                .Select(c => new PublicExam_PublicExamDTO(c)).ToList();
            return Exam_ExamDTOs;
        }

        [AllowAnonymous]
        [Route(PublicExamRoute.Get), HttpPost]
        public async Task<ActionResult<PublicExam_PublicExamDTO>> Get([FromBody] PublicExam_PublicExamDTO Exam_ExamDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Exam_ExamDTO.Id))
                return Forbid();

            Exam Exam = await ExamService.PublicGet(Exam_ExamDTO.Id);
            return new PublicExam_PublicExamDTO(Exam);
        }

        private async Task<bool> HasPermission(long Id)
        {
            ExamFilter ExamFilter = new ExamFilter();
            ExamFilter = await ExamService.ToFilter(ExamFilter);
            if (Id == 0)
            {

            }
            else
            {
                ExamFilter.Id = new IdFilter { Equal = Id };
                int count = await ExamService.Count(ExamFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Exam ConvertDTOToEntity(PublicExam_PublicExamDTO Exam_ExamDTO)
        {
            Exam_ExamDTO.TrimString();
            Exam Exam = new Exam();
            Exam.Id = Exam_ExamDTO.Id;
            Exam.Code = Exam_ExamDTO.Code;
            Exam.Name = Exam_ExamDTO.Name;
            Exam.SubjectId = Exam_ExamDTO.SubjectId;
            Exam.ExamLevelId = Exam_ExamDTO.ExamLevelId;
            Exam.StatusId = Exam_ExamDTO.StatusId;
            Exam.CreatorId = Exam_ExamDTO.CreatorId;
            Exam.GradeId = Exam_ExamDTO.GradeId;
            Exam.ExamStatusId = Exam_ExamDTO.ExamStatusId;
            Exam.TotalMark = Exam_ExamDTO.TotalMark;
            Exam.TotalQuestion = Exam_ExamDTO.TotalQuestion;
            Exam.ImageId = Exam_ExamDTO.ImageId;
            Exam.Time = Exam_ExamDTO.Time;
            Exam.Creator = Exam_ExamDTO.Creator == null ? null : new AppUser
            {
                Id = Exam_ExamDTO.Creator.Id,
                Username = Exam_ExamDTO.Creator.Username,
                DisplayName = Exam_ExamDTO.Creator.DisplayName,
                Password = Exam_ExamDTO.Creator.Password,
                RefreshToken = Exam_ExamDTO.Creator.RefreshToken,
                RoleId = Exam_ExamDTO.Creator.RoleId,
                ImageId = Exam_ExamDTO.Creator.ImageId,
            };
            Exam.ExamLevel = Exam_ExamDTO.ExamLevel == null ? null : new ExamLevel
            {
                Id = Exam_ExamDTO.ExamLevel.Id,
                Code = Exam_ExamDTO.ExamLevel.Code,
                Name = Exam_ExamDTO.ExamLevel.Name,
            };
            Exam.ExamStatus = Exam_ExamDTO.ExamStatus == null ? null : new ExamStatus
            {
                Id = Exam_ExamDTO.ExamStatus.Id,
                Code = Exam_ExamDTO.ExamStatus.Code,
                Name = Exam_ExamDTO.ExamStatus.Name,
            };
            Exam.Grade = Exam_ExamDTO.Grade == null ? null : new Grade
            {
                Id = Exam_ExamDTO.Grade.Id,
                Code = Exam_ExamDTO.Grade.Code,
                Name = Exam_ExamDTO.Grade.Name,
            };
            Exam.Image = Exam_ExamDTO.Image == null ? null : new Image
            {
                Id = Exam_ExamDTO.Image.Id,
                Name = Exam_ExamDTO.Image.Name,
                Url = Exam_ExamDTO.Image.Url,
            };
            Exam.Status = Exam_ExamDTO.Status == null ? null : new Status
            {
                Id = Exam_ExamDTO.Status.Id,
                Code = Exam_ExamDTO.Status.Code,
                Name = Exam_ExamDTO.Status.Name,
            };
            Exam.Subject = Exam_ExamDTO.Subject == null ? null : new Subject
            {
                Id = Exam_ExamDTO.Subject.Id,
                Code = Exam_ExamDTO.Subject.Code,
                Name = Exam_ExamDTO.Subject.Name,
            };
            Exam.ExamQuestionMappings = Exam_ExamDTO.ExamQuestionMappings?
                .Select(x => new ExamQuestionMapping
                {
                    QuestionId = x.QuestionId,
                    Mark = x.Mark,
                    Question = x.Question == null ? null : new Question
                    {
                        Id = x.Question.Id,
                        Code = x.Question.Code,
                        Name = x.Question.Name,
                        SubjectId = x.Question.SubjectId,
                        QuestionGroupId = x.Question.QuestionGroupId,
                        QuestionTypeId = x.Question.QuestionTypeId,
                        Content = x.Question.Content,
                        StatusId = x.Question.StatusId,
                        CreatorId = x.Question.CreatorId,
                        GradeId = x.Question.GradeId,
                    },
                }).ToList();
            var QuestionContentDTOs = Exam_ExamDTO.ExamQuestionMappings?.SelectMany(x => x.Question.QuestionContents).ToList();
            if (QuestionContentDTOs != null)
            {
                var QuestionContents = QuestionContentDTOs.Select(x => new QuestionContent
                {
                    Id = x.Id,
                    IsRight = x.IsRight,
                    QuestionId = x.QuestionId,
                    AnswerContent = x.AnswerContent
                }).ToList(); 

                foreach (ExamQuestionMapping ExamQuestionMapping in Exam.ExamQuestionMappings)
                {
                    ExamQuestionMapping.Question.QuestionContents = QuestionContents.Where(x => x.QuestionId == ExamQuestionMapping.QuestionId).ToList();
                }

            }
            Exam.BaseLanguage = CurrentContext.Language;
            return Exam;
        }

        private ExamFilter ConvertFilterDTOToFilterEntity(PublicExam_PublicExamFilterDTO Exam_ExamFilterDTO)
        {
            ExamFilter ExamFilter = new ExamFilter();
            ExamFilter.Selects = ExamSelect.ALL;
            ExamFilter.SearchBy = ExamSearch.ALL;
            ExamFilter.Skip = Exam_ExamFilterDTO.Skip;
            ExamFilter.Take = Exam_ExamFilterDTO.Take;
            ExamFilter.OrderBy = Exam_ExamFilterDTO.OrderBy;
            ExamFilter.OrderType = Exam_ExamFilterDTO.OrderType;

            ExamFilter.Id = Exam_ExamFilterDTO.Id;
            ExamFilter.Code = Exam_ExamFilterDTO.Code;
            ExamFilter.Name = Exam_ExamFilterDTO.Name;
            ExamFilter.SubjectId = Exam_ExamFilterDTO.SubjectId;
            ExamFilter.ExamLevelId = Exam_ExamFilterDTO.ExamLevelId;
            ExamFilter.StatusId = Exam_ExamFilterDTO.StatusId;
            ExamFilter.CreatorId = Exam_ExamFilterDTO.CreatorId;
            ExamFilter.GradeId = Exam_ExamFilterDTO.GradeId;
            ExamFilter.ExamStatusId = Exam_ExamFilterDTO.ExamStatusId;
            ExamFilter.TotalMark = Exam_ExamFilterDTO.TotalMark;
            ExamFilter.TotalQuestion = Exam_ExamFilterDTO.TotalQuestion;
            ExamFilter.ImageId = Exam_ExamFilterDTO.ImageId;
            ExamFilter.Time = Exam_ExamFilterDTO.Time;
            ExamFilter.CreatedAt = Exam_ExamFilterDTO.CreatedAt;
            ExamFilter.UpdatedAt = Exam_ExamFilterDTO.UpdatedAt;
            ExamFilter.Search = Exam_ExamFilterDTO.Search;
            return ExamFilter;
        }
    }
}

