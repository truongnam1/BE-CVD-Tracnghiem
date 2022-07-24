using System;
using System.Collections.Generic;

namespace Tracnghiem.Entities
{
    [Serializable]
    public class CurrentPermission
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public List<CurrentPermissionContent> CurrentPermissionContents { get; set; }
    }
    [Serializable]
    public class CurrentPermissionContent
    {
        public long Id { get; set; }
        public long FieldId { get; set; }
        public long FieldTypeId { get; set; }
        public string FieldName { get; set; }
        public long PermissionOperatorId { get; set; }
        public string Value { get; set; }
    }
}
