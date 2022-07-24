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
using Tracnghiem.Services.MGrade;
using Tracnghiem.Services.MQuestionGroup;
using Tracnghiem.Services.MQuestionType;
using Tracnghiem.Services.MStatus;
using Tracnghiem.Services.MSubject;
using Tracnghiem.Services.MQuestionContent;

namespace Tracnghiem.Rpc.question
{
    public partial class QuestionController 
    {
        [Route(QuestionRoute.SingleListGrade), HttpPost]
        public async Task<List<Question_GradeDTO>> SingleListGrade([FromBody] Question_GradeFilterDTO Question_GradeFilterDTO)
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
            List<Question_GradeDTO> Question_GradeDTOs = Grades
                .Select(x => new Question_GradeDTO(x)).ToList();
            return Question_GradeDTOs;
        }
        [Route(QuestionRoute.SingleListQuestionGroup), HttpPost]
        public async Task<List<Question_QuestionGroupDTO>> SingleListQuestionGroup([FromBody] Question_QuestionGroupFilterDTO Question_QuestionGroupFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionGroupFilter QuestionGroupFilter = new QuestionGroupFilter();
            QuestionGroupFilter.Skip = 0;
            QuestionGroupFilter.Take = int.MaxValue;
            QuestionGroupFilter.Take = 20;
            QuestionGroupFilter.OrderBy = QuestionGroupOrder.Id;
            QuestionGroupFilter.OrderType = OrderType.ASC;
            QuestionGroupFilter.Selects = QuestionGroupSelect.ALL;

            List<QuestionGroup> QuestionGroups = await QuestionGroupService.List(QuestionGroupFilter);
            List<Question_QuestionGroupDTO> Question_QuestionGroupDTOs = QuestionGroups
                .Select(x => new Question_QuestionGroupDTO(x)).ToList();
            return Question_QuestionGroupDTOs;
        }
        [Route(QuestionRoute.SingleListQuestionType), HttpPost]
        public async Task<List<Question_QuestionTypeDTO>> SingleListQuestionType([FromBody] Question_QuestionTypeFilterDTO Question_QuestionTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionTypeFilter QuestionTypeFilter = new QuestionTypeFilter();
            QuestionTypeFilter.Skip = 0;
            QuestionTypeFilter.Take = int.MaxValue;
            QuestionTypeFilter.Take = 20;
            QuestionTypeFilter.OrderBy = QuestionTypeOrder.Id;
            QuestionTypeFilter.OrderType = OrderType.ASC;
            QuestionTypeFilter.Selects = QuestionTypeSelect.ALL;

            List<QuestionType> QuestionTypes = await QuestionTypeService.List(QuestionTypeFilter);
            List<Question_QuestionTypeDTO> Question_QuestionTypeDTOs = QuestionTypes
                .Select(x => new Question_QuestionTypeDTO(x)).ToList();
            return Question_QuestionTypeDTOs;
        }
        [Route(QuestionRoute.SingleListStatus), HttpPost]
        public async Task<List<Question_StatusDTO>> SingleListStatus([FromBody] Question_StatusFilterDTO Question_StatusFilterDTO)
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
            List<Question_StatusDTO> Question_StatusDTOs = Statuses
                .Select(x => new Question_StatusDTO(x)).ToList();
            return Question_StatusDTOs;
        }
        [Route(QuestionRoute.SingleListSubject), HttpPost]
        public async Task<List<Question_SubjectDTO>> SingleListSubject([FromBody] Question_SubjectFilterDTO Question_SubjectFilterDTO)
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
            List<Question_SubjectDTO> Question_SubjectDTOs = Subjects
                .Select(x => new Question_SubjectDTO(x)).ToList();
            return Question_SubjectDTOs;
        }
        [Route(QuestionRoute.SingleListQuestionContent), HttpPost]
        public async Task<List<Question_QuestionContentDTO>> SingleListQuestionContent([FromBody] Question_QuestionContentFilterDTO Question_QuestionContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionContentFilter QuestionContentFilter = new QuestionContentFilter();
            QuestionContentFilter.Skip = 0;
            QuestionContentFilter.Take = 20;
            QuestionContentFilter.OrderBy = QuestionContentOrder.Id;
            QuestionContentFilter.OrderType = OrderType.ASC;
            QuestionContentFilter.Selects = QuestionContentSelect.ALL;
            QuestionContentFilter.Id = Question_QuestionContentFilterDTO.Id;
            QuestionContentFilter.QuestionId = Question_QuestionContentFilterDTO.QuestionId;
            QuestionContentFilter.AnswerContent = Question_QuestionContentFilterDTO.AnswerContent;

            List<QuestionContent> QuestionContents = await QuestionContentService.List(QuestionContentFilter);
            List<Question_QuestionContentDTO> Question_QuestionContentDTOs = QuestionContents
                .Select(x => new Question_QuestionContentDTO(x)).ToList();
            return Question_QuestionContentDTOs;
        }
    }
}

