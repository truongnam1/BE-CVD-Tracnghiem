using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.exam
{
    public class Exam_ExamLevelDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Exam_ExamLevelDTO() {}
        public Exam_ExamLevelDTO(ExamLevel ExamLevel)
        {
            this.Id = ExamLevel.Id;
            this.Code = ExamLevel.Code;
            this.Name = ExamLevel.Name;
            this.Informations = ExamLevel.Informations;
            this.Warnings = ExamLevel.Warnings;
            this.Errors = ExamLevel.Errors;
        }
    }

    public class Exam_ExamLevelFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public ExamLevelOrder OrderBy { get; set; }
    }
}