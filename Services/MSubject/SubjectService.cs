using TrueSight.Common;
using Tracnghiem.Common;
using Tracnghiem.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Tracnghiem.Repositories;
using Tracnghiem.Entities;
using Tracnghiem.Enums;

namespace Tracnghiem.Services.MSubject
{
    public interface ISubjectService :  IServiceScoped
    {
        Task<int> Count(SubjectFilter SubjectFilter);
        Task<List<Subject>> List(SubjectFilter SubjectFilter);
    }

    public class SubjectService : BaseService, ISubjectService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public SubjectService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
        }

        public async Task<int> Count(SubjectFilter SubjectFilter)
        {
            try
            {
                int result = await UOW.SubjectRepository.Count(SubjectFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SubjectService));
            }
            return 0;
        }

        public async Task<List<Subject>> List(SubjectFilter SubjectFilter)
        {
            try
            {
                List<Subject> Subjects = await UOW.SubjectRepository.List(SubjectFilter);

                if (Subjects != null)
                {
                    List<long> Ids = Subjects.Select(x => x.Id).ToList();
                    ExamFilter ExamFilter = new ExamFilter();
                    ExamFilter.Take = int.MaxValue;
                    ExamFilter.Skip = 0;
                    ExamFilter.SubjectId = new IdFilter { In = Ids };
                    ExamFilter.Selects = ExamSelect.Id | ExamSelect.Subject;
                    List<Exam> Exams = await UOW.ExamRepository.List(ExamFilter);
                    foreach (var Subject in Subjects)
                    {
                        Subject.TotalExam = Exams.Where(x => x.SubjectId == Subject.Id).Count();
                    }

                }
                return Subjects;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SubjectService));
            }
            return null;
        }

    }
}
