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

namespace Tracnghiem.Services.MExamStatus
{
    public interface IExamStatusService :  IServiceScoped
    {
        Task<int> Count(ExamStatusFilter ExamStatusFilter);
        Task<List<ExamStatus>> List(ExamStatusFilter ExamStatusFilter);
    }

    public class ExamStatusService : BaseService, IExamStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public ExamStatusService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
        }

        public async Task<int> Count(ExamStatusFilter ExamStatusFilter)
        {
            try
            {
                int result = await UOW.ExamStatusRepository.Count(ExamStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamStatusService));
            }
            return 0;
        }

        public async Task<List<ExamStatus>> List(ExamStatusFilter ExamStatusFilter)
        {
            try
            {
                List<ExamStatus> ExamStatuses = await UOW.ExamStatusRepository.List(ExamStatusFilter);
                return ExamStatuses;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamStatusService));
            }
            return null;
        }

    }
}
