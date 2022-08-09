using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.subject
{
    public class Subject_SubjectDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public Subject_SubjectDTO() {}
        public Subject_SubjectDTO(Subject Subject)
        {
            this.Id = Subject.Id;
            this.Code = Subject.Code;
            this.Name = Subject.Name;
            this.Avatar = Subject.Avatar;
            this.Informations = Subject.Informations;
            this.Warnings = Subject.Warnings;
            this.Errors = Subject.Errors;
        }
    }

    public class Subject_SubjectFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
      
        public string Search { get; set; }
        public SubjectOrder OrderBy { get; set; }
    }
}
