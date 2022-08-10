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

namespace Tracnghiem.Rpc.subject
{
    public class SubjectController : RpcController
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
        public SubjectController(
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
        [Route(SubjectRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Subject_SubjectFilterDTO Subject_SubjectFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SubjectFilter SubjectFilter = ConvertFilterDTOToFilterEntity(Subject_SubjectFilterDTO);
            int count = await SubjectService.Count(SubjectFilter);
            return count;
        }

        [AllowAnonymous]
        [Route(SubjectRoute.List), HttpPost]
        public async Task<ActionResult<List<Subject_SubjectDTO>>> List([FromBody] Subject_SubjectFilterDTO Subject_SubjectFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SubjectFilter SubjectFilter = ConvertFilterDTOToFilterEntity(Subject_SubjectFilterDTO);
            List<Subject> Subjects = await SubjectService.List(SubjectFilter);
            List<Subject_SubjectDTO> Subject_SubjectDTOs = Subjects
                .Select(c => new Subject_SubjectDTO(c)).ToList();
            return Subject_SubjectDTOs;
        }

        private SubjectFilter ConvertFilterDTOToFilterEntity(Subject_SubjectFilterDTO Subject_SubjectFilterDTO)
        {
            SubjectFilter SubjectFilter = new SubjectFilter();
            SubjectFilter.Selects = SubjectSelect.ALL;
            SubjectFilter.SearchBy = SubjectSearch.ALL;
            SubjectFilter.Skip = Subject_SubjectFilterDTO.Skip;
            SubjectFilter.Take = Subject_SubjectFilterDTO.Take;
            SubjectFilter.OrderBy = Subject_SubjectFilterDTO.OrderBy;
            SubjectFilter.OrderType = Subject_SubjectFilterDTO.OrderType;

            SubjectFilter.Id = Subject_SubjectFilterDTO.Id;
            SubjectFilter.Code = Subject_SubjectFilterDTO.Code;
            SubjectFilter.Name = Subject_SubjectFilterDTO.Name;
            
            SubjectFilter.Search = Subject_SubjectFilterDTO.Search;
            return SubjectFilter;
        }
    }
}

