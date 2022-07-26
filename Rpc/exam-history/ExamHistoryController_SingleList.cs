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
        [Route(ExamHistoryRoute.SingleListExam), HttpPost]
        public async Task<List<ExamHistory_ExamDTO>> SingleListExam([FromBody] ExamHistory_ExamFilterDTO ExamHistory_ExamFilterDTO)
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
            ExamFilter.StatusId = new IdFilter{ Equal = 1 };

            List<Exam> Exams = await ExamService.List(ExamFilter);
            List<ExamHistory_ExamDTO> ExamHistory_ExamDTOs = Exams
                .Select(x => new ExamHistory_ExamDTO(x)).ToList();
            return ExamHistory_ExamDTOs;
        }
    }
}

