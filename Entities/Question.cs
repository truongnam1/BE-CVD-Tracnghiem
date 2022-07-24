using TrueSight.Common;
using System;
using System.Collections.Generic;
using Tracnghiem.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tracnghiem.Entities
{
    public class Question : DataEntity,  IEquatable<Question>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long SubjectId { get; set; }
        public long QuestionGroupId { get; set; }
        public long QuestionTypeId { get; set; }
        public string Content { get; set; }
        public long StatusId { get; set; }
        public long CreatorId { get; set; }
        public long GradeId { get; set; }
        public Grade Grade { get; set; }
        public QuestionGroup QuestionGroup { get; set; }
        public QuestionType QuestionType { get; set; }
        public Status Status { get; set; }
        public Subject Subject { get; set; }
        public List<QuestionContent> QuestionContents { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Question other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            if (this.SubjectId != other.SubjectId) return false;
            if (this.QuestionGroupId != other.QuestionGroupId) return false;
            if (this.QuestionTypeId != other.QuestionTypeId) return false;
            if (this.Content != other.Content) return false;
            if (this.StatusId != other.StatusId) return false;
            if (this.CreatorId != other.CreatorId) return false;
            if (this.GradeId != other.GradeId) return false;
            if (this.QuestionContents?.Count != other.QuestionContents?.Count) return false;
            else if (this.QuestionContents != null && other.QuestionContents != null)
            {
                for (int i = 0; i < QuestionContents.Count; i++)
                {
                    QuestionContent QuestionContent = QuestionContents[i];
                    QuestionContent otherQuestionContent = other.QuestionContents[i];
                    if (QuestionContent == null && otherQuestionContent != null)
                        return false;
                    if (QuestionContent != null && otherQuestionContent == null)
                        return false;
                    if (QuestionContent.Equals(otherQuestionContent) == false)
                        return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class QuestionFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter SubjectId { get; set; }
        public IdFilter QuestionGroupId { get; set; }
        public IdFilter QuestionTypeId { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter CreatorId { get; set; }
        public IdFilter GradeId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public string Search { get; set; }
        public List<QuestionFilter> OrFilter { get; set; }
        public QuestionOrder OrderBy {get; set;}
        public QuestionSelect Selects {get; set;}
        public QuestionSearch SearchBy {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuestionOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Subject = 3,
        QuestionGroup = 4,
        QuestionType = 5,
        Content = 6,
        Status = 7,
        Creator = 8,
        Grade = 13,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum QuestionSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Subject = E._3,
        QuestionGroup = E._4,
        QuestionType = E._5,
        Content = E._6,
        Status = E._7,
        Creator = E._8,
        Grade = E._13,
    }

    [Flags]
    public enum QuestionSearch:long
    {
        ALL = E.ALL,
        Code = E._1,
        Name = E._2,
        Content = E._6,
    }
}
