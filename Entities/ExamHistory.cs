using TrueSight.Common;
using System;
using System.Collections.Generic;
using Tracnghiem.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tracnghiem.Entities
{
    public class ExamHistory : DataEntity,  IEquatable<ExamHistory>
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long ExamId { get; set; }
        public long Times { get; set; }
        public long CorrectAnswerQuantity { get; set; }
        public long TotalQuestionQuantity { get; set; }
        public decimal Mark { get; set; }
        public DateTime ExamedAt { get; set; }
        public AppUser AppUser { get; set; }
        public Exam Exam { get; set; }
        
        public bool Equals(ExamHistory other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.AppUserId != other.AppUserId) return false;
            if (this.ExamId != other.ExamId) return false;
            if (this.Times != other.Times) return false;
            if (this.CorrectAnswerQuantity != other.CorrectAnswerQuantity) return false;
            if (this.TotalQuestionQuantity != other.TotalQuestionQuantity) return false;
            if (this.Mark != other.Mark) return false;
            if (this.ExamedAt != other.ExamedAt) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ExamHistoryFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter ExamId { get; set; }
        public LongFilter Times { get; set; }
        public LongFilter CorrectAnswerQuantity { get; set; }
        public LongFilter TotalQuestionQuantity { get; set; }
        public DecimalFilter Mark { get; set; }
        public DateFilter ExamedAt { get; set; }
        public List<ExamHistoryFilter> OrFilter { get; set; }
        public ExamHistoryOrder OrderBy {get; set;}
        public ExamHistorySelect Selects {get; set;}
        public ExamHistorySearch SearchBy {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExamHistoryOrder
    {
        Id = 0,
        AppUser = 1,
        Exam = 2,
        Times = 3,
        CorrectAnswerQuantity = 4,
        TotalQuestionQuantity = 5,
        Mark = 6,
        ExamedAt = 7,
    }

    [Flags]
    public enum ExamHistorySelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        AppUser = E._1,
        Exam = E._2,
        Times = E._3,
        CorrectAnswerQuantity = E._4,
        TotalQuestionQuantity = E._5,
        Mark = E._6,
        ExamedAt = E._7,
    }

    [Flags]
    public enum ExamHistorySearch:long
    {
        ALL = E.ALL,
    }
}
