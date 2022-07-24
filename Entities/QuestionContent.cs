using TrueSight.Common;
using System;
using System.Collections.Generic;
using Tracnghiem.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tracnghiem.Entities
{
    public class QuestionContent : DataEntity,  IEquatable<QuestionContent>
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public string AnswerContent { get; set; }
        public bool IsRight { get; set; }
        public Question Question { get; set; }
        
        public bool Equals(QuestionContent other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.QuestionId != other.QuestionId) return false;
            if (this.AnswerContent != other.AnswerContent) return false;
            if (this.IsRight != other.IsRight) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class QuestionContentFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter QuestionId { get; set; }
        public StringFilter AnswerContent { get; set; }
        public bool? IsRight { get; set; }
        public List<QuestionContentFilter> OrFilter { get; set; }
        public QuestionContentOrder OrderBy {get; set;}
        public QuestionContentSelect Selects {get; set;}
        public QuestionContentSearch SearchBy {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuestionContentOrder
    {
        Id = 0,
        Question = 1,
        AnswerContent = 2,
        IsRight = 3,
    }

    [Flags]
    public enum QuestionContentSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Question = E._1,
        AnswerContent = E._2,
        IsRight = E._3,
    }

    [Flags]
    public enum QuestionContentSearch:long
    {
        ALL = E.ALL,
        AnswerContent = E._2,
    }
}
