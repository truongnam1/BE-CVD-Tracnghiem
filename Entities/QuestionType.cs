using TrueSight.Common;
using System;
using System.Collections.Generic;
using Tracnghiem.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tracnghiem.Entities
{
    public class QuestionType : DataEntity,  IEquatable<QuestionType>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public bool Equals(QuestionType other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class QuestionTypeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<QuestionTypeFilter> OrFilter { get; set; }
        public QuestionTypeOrder OrderBy {get; set;}
        public QuestionTypeSelect Selects {get; set;}
        public QuestionTypeSearch SearchBy {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuestionTypeOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum QuestionTypeSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }

    [Flags]
    public enum QuestionTypeSearch:long
    {
        ALL = E.ALL,
        Code = E._1,
        Name = E._2,
    }
}
