using TrueSight.Common;
using Tracnghiem.Handlers.Configuration;
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

namespace Tracnghiem.Services.MExamLevel
{
    public interface IExamLevelService :  IServiceScoped
    {
        Task<int> Count(ExamLevelFilter ExamLevelFilter);
        Task<List<ExamLevel>> List(ExamLevelFilter ExamLevelFilter);
    }

    public class ExamLevelService : BaseService, IExamLevelService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public ExamLevelService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
        }

        public async Task<int> Count(ExamLevelFilter ExamLevelFilter)
        {
            try
            {
                int result = await UOW.ExamLevelRepository.Count(ExamLevelFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamLevelService));
            }
            return 0;
        }

        public async Task<List<ExamLevel>> List(ExamLevelFilter ExamLevelFilter)
        {
            try
            {
                List<ExamLevel> ExamLevels = await UOW.ExamLevelRepository.List(ExamLevelFilter);
                return ExamLevels;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamLevelService));
            }
            return null;
        }

    }
}
