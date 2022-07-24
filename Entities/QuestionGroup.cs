using TrueSight.Common;
using System;
using System.Collections.Generic;
using Tracnghiem.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tracnghiem.Entities
{
    public class QuestionGroup : DataEntity,  IEquatable<QuestionGroup>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public bool Equals(QuestionGroup other)
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

    public class QuestionGroupFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<QuestionGroupFilter> OrFilter { get; set; }
        public QuestionGroupOrder OrderBy {get; set;}
        public QuestionGroupSelect Selects {get; set;}
        public QuestionGroupSearch SearchBy {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuestionGroupOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum QuestionGroupSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }

    [Flags]
    public enum QuestionGroupSearch:long
    {
        ALL = E.ALL,
        Code = E._1,
        Name = E._2,
    }
}
