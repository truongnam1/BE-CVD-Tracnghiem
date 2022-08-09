using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.public_exam
{
    public class PublicExam_ExamQuestionMappingDTO : DataDTO
    {
        public long ExamId { get; set; }
        public long QuestionId { get; set; }
        public decimal? Mark { get; set; }
        public PublicExam_QuestionDTO Question { get; set; }
        public PublicExam_ExamQuestionMappingDTO() {}
        public PublicExam_ExamQuestionMappingDTO(ExamQuestionMapping ExamQuestionMapping)
        {
            this.ExamId = ExamQuestionMapping.ExamId;
            this.QuestionId = ExamQuestionMapping.QuestionId;
            this.Mark = ExamQuestionMapping.Mark;
            this.Question = ExamQuestionMapping.Question == null ? null : new PublicExam_QuestionDTO(ExamQuestionMapping.Question);
            this.Errors = ExamQuestionMapping.Errors;
        }
    }

    public class PublicExam_ExamQuestionMappingFilterDTO : FilterDTO
    {
        public IdFilter ExamId { get; set; }
        public IdFilter QuestionId { get; set; }
        public DecimalFilter Mark { get; set; }
        public ExamQuestionMappingOrder OrderBy { get; set; }
    }
}