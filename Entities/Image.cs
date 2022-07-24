using TrueSight.Common;
using System;
using System.Collections.Generic;
using Tracnghiem.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tracnghiem.Entities
{
    public class Image : DataEntity,  IEquatable<Image>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Guid RowId { get; set; }
        
        public bool Equals(Image other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Name != other.Name) return false;
            if (this.Url != other.Url) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ImageFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Url { get; set; }
        public string Search { get; set; }
        public List<ImageFilter> OrFilter { get; set; }
        public ImageOrder OrderBy {get; set;}
        public ImageSelect Selects {get; set;}
        public ImageSearch SearchBy {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImageOrder
    {
        Id = 0,
        Name = 1,
        Url = 2,
    }

    [Flags]
    public enum ImageSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Url = E._2,
    }

    [Flags]
    public enum ImageSearch:long
    {
        ALL = E.ALL,
        Name = E._1,
        Url = E._2,
    }
}
