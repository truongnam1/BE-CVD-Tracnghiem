using TrueSight.Common;
using System.Collections.Generic;

namespace Tracnghiem.Enums
{
    public class RoutingKeyEnum
    {
        public static GenericEnum MenuSend = new GenericEnum(Id: 1, Code: "Menu.Send", Name: "Menu Send");
        public static GenericEnum RoleSend = new GenericEnum(Id: 2, Code: "Role.Send", Name: "Role Send");
        public static GenericEnum MailSend = new GenericEnum(Id: 3, Code: "Mail.Send", Name: "Mail Send");

        public static GenericEnum MasterEntityRegister = new GenericEnum(Id: 3, Code: "MasterEntity.Register", Name: "MasterEntiry Register");
        public static GenericEnum AuditLogSend = new GenericEnum (Id : 5, Code : "AuditLog.Send", Name : "Audit Log");
        public static GenericEnum SystemLogSend = new GenericEnum (Id : 6, Code : "SystemLog.Send", Name : "System Log");

        public static List<GenericEnum> RoutingKeyEnumList = new List<GenericEnum>()
        {
            AuditLogSend, SystemLogSend
        };

        public static GenericEnum AppUserSync = new GenericEnum (Id : 1000, Code : "AppUser.Sync", Name : "AppUser");
        public static GenericEnum ExamSync = new GenericEnum (Id : 1001, Code : "Exam.Sync", Name : "Exam");
        public static GenericEnum ExamHistorySync = new GenericEnum (Id : 1002, Code : "ExamHistory.Sync", Name : "ExamHistory");
        public static GenericEnum ExamLevelSync = new GenericEnum (Id : 1003, Code : "ExamLevel.Sync", Name : "ExamLevel");
        public static GenericEnum ExamStatusSync = new GenericEnum (Id : 1005, Code : "ExamStatus.Sync", Name : "ExamStatus");
        public static GenericEnum GradeSync = new GenericEnum (Id : 1006, Code : "Grade.Sync", Name : "Grade");
        public static GenericEnum ImageSync = new GenericEnum (Id : 1007, Code : "Image.Sync", Name : "Image");
        public static GenericEnum QuestionContentSync = new GenericEnum (Id : 1008, Code : "QuestionContent.Sync", Name : "QuestionContent");
        public static GenericEnum QuestionSync = new GenericEnum (Id : 1009, Code : "Question.Sync", Name : "Question");
        public static GenericEnum QuestionGroupSync = new GenericEnum (Id : 1010, Code : "QuestionGroup.Sync", Name : "QuestionGroup");
        public static GenericEnum QuestionTypeSync = new GenericEnum (Id : 1011, Code : "QuestionType.Sync", Name : "QuestionType");
        public static GenericEnum RoleSync = new GenericEnum (Id : 1012, Code : "Role.Sync", Name : "Role");
        public static GenericEnum StatusSync = new GenericEnum (Id : 1013, Code : "Status.Sync", Name : "Status");
        public static GenericEnum SubjectSync = new GenericEnum (Id : 1014, Code : "Subject.Sync", Name : "Subject");

        public static GenericEnum AppUserUsed = new GenericEnum (Id : 2000, Code : "AppUser.Used", Name : "AppUser Used");
        public static GenericEnum ExamUsed = new GenericEnum (Id : 2001, Code : "Exam.Used", Name : "Exam Used");
        public static GenericEnum ExamHistoryUsed = new GenericEnum (Id : 2002, Code : "ExamHistory.Used", Name : "ExamHistory Used");
        public static GenericEnum ExamLevelUsed = new GenericEnum (Id : 2003, Code : "ExamLevel.Used", Name : "ExamLevel Used");
        public static GenericEnum ExamStatusUsed = new GenericEnum (Id : 2005, Code : "ExamStatus.Used", Name : "ExamStatus Used");
        public static GenericEnum GradeUsed = new GenericEnum (Id : 2006, Code : "Grade.Used", Name : "Grade Used");
        public static GenericEnum ImageUsed = new GenericEnum (Id : 2007, Code : "Image.Used", Name : "Image Used");
        public static GenericEnum QuestionContentUsed = new GenericEnum (Id : 2008, Code : "QuestionContent.Used", Name : "QuestionContent Used");
        public static GenericEnum QuestionUsed = new GenericEnum (Id : 2009, Code : "Question.Used", Name : "Question Used");
        public static GenericEnum QuestionGroupUsed = new GenericEnum (Id : 2010, Code : "QuestionGroup.Used", Name : "QuestionGroup Used");
        public static GenericEnum QuestionTypeUsed = new GenericEnum (Id : 2011, Code : "QuestionType.Used", Name : "QuestionType Used");
        public static GenericEnum RoleUsed = new GenericEnum (Id : 2012, Code : "Role.Used", Name : "Role Used");
        public static GenericEnum StatusUsed = new GenericEnum (Id : 2013, Code : "Status.Used", Name : "Status Used");
        public static GenericEnum SubjectUsed = new GenericEnum (Id : 2014, Code : "Subject.Used", Name : "Subject Used");
    }
}
