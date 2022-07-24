using TrueSight.Common;
using System;
using System.Collections.Generic;
using Tracnghiem.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tracnghiem.Entities
{
    public class ExamQuestionMapping : DataEntity,  IEquatable<ExamQuestionMapping>
    {
        public long ExamId { get; set; }
        public long QuestionId { get; set; }
        public decimal? Mark { get; set; }
        public Exam Exam { get; set; }
        public Question Question { get; set; }
        
        public bool Equals(ExamQuestionMapping other)
        {
            if (other == null) return false;
            if (this.ExamId != other.ExamId) return false;
            if (this.QuestionId != other.QuestionId) return false;
            if (this.Mark != other.Mark) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ExamQuestionMappingFilter : FilterEntity
    {
        public IdFilter ExamId { get; set; }
        public IdFilter QuestionId { get; set; }
        public DecimalFilter Mark { get; set; }
        public List<ExamQuestionMappingFilter> OrFilter { get; set; }
        public ExamQuestionMappingOrder OrderBy {get; set;}
        public ExamQuestionMappingSelect Selects {get; set;}
        public ExamQuestionMappingSearch SearchBy {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExamQuestionMappingOrder
    {
        Exam = 0,
        Question = 1,
        Mark = 2,
    }

    [Flags]
    public enum ExamQuestionMappingSelect:long
    {
        ALL = E.ALL,
        Exam = E._0,
        Question = E._1,
        Mark = E._2,
    }

    [Flags]
    public enum ExamQuestionMappingSearch:long
    {
        ALL = E.ALL,
    }
}
