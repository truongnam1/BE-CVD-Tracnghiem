using TrueSight.Common;
using System;
using System.Collections.Generic;
using Tracnghiem.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tracnghiem.Entities
{
    public class Exam : DataEntity,  IEquatable<Exam>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long SubjectId { get; set; }
        public long ExamLevelId { get; set; }
        public long StatusId { get; set; }
        public long CreatorId { get; set; }
        public long GradeId { get; set; }
        public long ExamStatusId { get; set; }
        public decimal? TotalMark { get; set; }
        public long TotalQuestion { get; set; }
        public long? ImageId { get; set; }
        public long? Time { get; set; }
        public long CurrentMonthNumberTest { get; set; }
        public long TotalNumberTest { get; set; }
        public AppUser Creator { get; set; }
        public ExamLevel ExamLevel { get; set; }
        public ExamStatus ExamStatus { get; set; }
        public Grade Grade { get; set; }
        public Image Image { get; set; }
        public Status Status { get; set; }
        public Subject Subject { get; set; }
        public List<ExamQuestionMapping> ExamQuestionMappings { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Exam other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            if (this.SubjectId != other.SubjectId) return false;
            if (this.ExamLevelId != other.ExamLevelId) return false;
            if (this.StatusId != other.StatusId) return false;
            if (this.CreatorId != other.CreatorId) return false;
            if (this.GradeId != other.GradeId) return false;
            if (this.ExamStatusId != other.ExamStatusId) return false;
            if (this.TotalMark != other.TotalMark) return false;
            if (this.TotalQuestion != other.TotalQuestion) return false;
            if (this.ImageId != other.ImageId) return false;
            if (this.Time != other.Time) return false;
            if (this.ExamQuestionMappings?.Count != other.ExamQuestionMappings?.Count) return false;
            else if (this.ExamQuestionMappings != null && other.ExamQuestionMappings != null)
            {
                for (int i = 0; i < ExamQuestionMappings.Count; i++)
                {
                    ExamQuestionMapping ExamQuestionMapping = ExamQuestionMappings[i];
                    ExamQuestionMapping otherExamQuestionMapping = other.ExamQuestionMappings[i];
                    if (ExamQuestionMapping == null && otherExamQuestionMapping != null)
                        return false;
                    if (ExamQuestionMapping != null && otherExamQuestionMapping == null)
                        return false;
                    if (ExamQuestionMapping.Equals(otherExamQuestionMapping) == false)
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

    public class ExamFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter SubjectId { get; set; }
        public IdFilter ExamLevelId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter CreatorId { get; set; }
        public IdFilter GradeId { get; set; }
        public IdFilter ExamStatusId { get; set; }
        public DecimalFilter TotalMark { get; set; }
        public LongFilter TotalQuestion { get; set; }
        public LongFilter CurrentMonthNumberTest { get; set; }
        public LongFilter TotalNumberTest { get; set; }
        public IdFilter ImageId { get; set; }
        public LongFilter Time { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public string Search { get; set; }
        public List<ExamFilter> OrFilter { get; set; }
        public ExamOrder OrderBy {get; set;}
        public ExamSelect Selects {get; set;}
        public ExamSearch SearchBy {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExamOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Subject = 3,
        ExamLevel = 4,
        Status = 5,
        Creator = 6,
        Grade = 11,
        ExamStatus = 12,
        TotalMark = 13,
        TotalQuestion = 14,
        Image = 15,
        Time = 16,
        CurrentMonthNumberTest = 17,
        TotalNumberTest = 18,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ExamSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Subject = E._3,
        ExamLevel = E._4,
        Status = E._5,
        Creator = E._6,
        Grade = E._11,
        ExamStatus = E._12,
        TotalMark = E._13,
        TotalQuestion = E._14,
        Image = E._15,
        Time = E._16,
        CurrentMonthNumberTest = E._17,
        TotalNumberTest = E._18,
    }

    [Flags]
    public enum ExamSearch:long
    {
        ALL = E.ALL,
        Code = E._1,
        Name = E._2,
    }
}
