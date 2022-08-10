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

namespace Tracnghiem.Services.MDoingExam
{
    public interface IDoingExamService : IServiceScoped
    {
        Task<Exam> GetExam(long Id);
        Task<ExamHistory> SubmitExam(Exam Exam);

    }

    public class DoingExamService : BaseService, IDoingExamService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        //private IExamValidator ExamValidator;

        public DoingExamService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            //IExamValidator ExamValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;

            //this.ExamValidator = ExamValidator;
        }

        public async Task<Exam> GetExam(long ExamId)
        {
            Exam Exam = await UOW.ExamRepository.Get(ExamId);
            if (Exam == null)
                return null;
            var QuestionContents = Exam.ExamQuestionMappings.SelectMany(x => x.Question.QuestionContents).ToList();

            QuestionContents.ForEach(x => x.IsRight = false);

            return Exam;
        }

        public async Task<ExamHistory> SubmitExam(Exam Exam)
        {
            try
            {
                List<ExamHistory> ExamHistories = await UOW.ExamHistoryRepository.List(new ExamHistoryFilter
                {
                    AppUserId = new IdFilter { Equal = CurrentContext.UserId },
                    ExamId = new IdFilter { Equal = Exam.Id },
                    Take = int.MaxValue,
                    Selects = ExamHistorySelect.ALL
                });
                long ExamingLatestTime = ExamHistories.Count != 0? ExamHistories.Max(x => x.Times) : 0;

                List<Question> Answers = Exam.ExamQuestionMappings.Select(x => x.Question).ToList();
                //check dap an
                var ExamDB = await UOW.ExamRepository.Get(Exam.Id);

                long totalQuestions = ExamDB.ExamQuestionMappings.Count();
                long totalRightAnswers = 0;
                foreach (Question Answer in Answers)
                {
                    var ExamQuestionMapping = ExamDB.ExamQuestionMappings.FirstOrDefault(x => x.QuestionId == Answer.Id);
                    Question Question = ExamQuestionMapping.Question;
                    var StudentAnswers = Answer.QuestionContents.Where(x => x.IsRight).Select(x => x.Id).ToList();
                    var RightAnswers = Question.QuestionContents.Where(x => x.IsRight).Select(x => x.Id).ToList();
                    if (StudentAnswers.Count == RightAnswers.Count)
                    {
                        var Differences = RightAnswers.Except(StudentAnswers).ToList();
                        if (Differences.Count == 0)
                        {
                            totalRightAnswers++;
                        }
                    }
                }
                ExamHistory ExamHistory = new ExamHistory()
                {
                    AppUserId = CurrentContext.UserId,
                    ExamId = Exam.Id,
                    Times = ExamingLatestTime + 1,
                    CorrectAnswerQuantity = totalRightAnswers,
                    TotalQuestionQuantity = totalQuestions,
                    Mark = Math.Round((decimal)totalRightAnswers / totalQuestions * 10, 2),
                    ExamedAt = StaticParams.DateTimeNow
                };
                await UOW.ExamHistoryRepository.Create(ExamHistory);
                await BuildNumberTest(Exam.Id);
                return ExamHistory;
            } 
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(DoingExamService));
            }
            return null;
        }
        private async Task BuildNumberTest(long ExamId)
        {
            Exam Exam = await UOW.ExamRepository.Get(ExamId);
            DateTime Now = StaticParams.DateTimeNow;
            ExamHistoryFilter ExamHistoryFilter = new ExamHistoryFilter();
            ExamHistoryFilter.Skip = 0;
            ExamHistoryFilter.Take = int.MaxValue;
            ExamHistoryFilter.ExamId = new IdFilter { Equal = ExamId };
            ExamHistoryFilter.Selects = ExamHistorySelect.ExamedAt;
            List<ExamHistory> ExamHistories = await UOW.ExamHistoryRepository.List(ExamHistoryFilter);

            Exam.CurrentMonthNumberTest = ExamHistories.Where(x => x.ExamedAt >= Now.AddMonths(-1)).Count();
            Exam.TotalNumberTest = ExamHistories.Count();

            await UOW.ExamRepository.Update(Exam);
        }
    }
}
