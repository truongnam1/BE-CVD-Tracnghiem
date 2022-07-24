using TrueSight.Common;
using System;
using System.Collections.Generic;
using Tracnghiem.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tracnghiem.Entities
{
    public class Subject : DataEntity,  IEquatable<Subject>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public bool Equals(Subject other)
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

    public class SubjectFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<SubjectFilter> OrFilter { get; set; }
        public SubjectOrder OrderBy {get; set;}
        public SubjectSelect Selects {get; set;}
        public SubjectSearch SearchBy {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubjectOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum SubjectSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }

    [Flags]
    public enum SubjectSearch:long
    {
        ALL = E.ALL,
        Code = E._1,
        Name = E._2,
    }
}
