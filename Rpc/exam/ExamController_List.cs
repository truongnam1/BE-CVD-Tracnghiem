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
        [Route(ExamRoute.CountQuestion), HttpPost]
        public async Task<long> CountQuestion([FromBody] Exam_QuestionFilterDTO Exam_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionFilter QuestionFilter = new QuestionFilter();
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

            return await QuestionService.Count(QuestionFilter);
        }

        [Route(ExamRoute.ListQuestion), HttpPost]
        public async Task<List<Exam_QuestionDTO>> ListQuestion([FromBody] Exam_QuestionFilterDTO Exam_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionFilter QuestionFilter = new QuestionFilter();
            QuestionFilter.Skip = Exam_QuestionFilterDTO.Skip;
            QuestionFilter.Take = Exam_QuestionFilterDTO.Take;
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

