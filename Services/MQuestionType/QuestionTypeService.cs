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

namespace Tracnghiem.Services.MQuestionType
{
    public interface IQuestionTypeService :  IServiceScoped
    {
        Task<int> Count(QuestionTypeFilter QuestionTypeFilter);
        Task<List<QuestionType>> List(QuestionTypeFilter QuestionTypeFilter);
    }

    public class QuestionTypeService : BaseService, IQuestionTypeService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public QuestionTypeService(
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

        public async Task<int> Count(QuestionTypeFilter QuestionTypeFilter)
        {
            try
            {
                int result = await UOW.QuestionTypeRepository.Count(QuestionTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionTypeService));
            }
            return 0;
        }

        public async Task<List<QuestionType>> List(QuestionTypeFilter QuestionTypeFilter)
        {
            try
            {
                List<QuestionType> QuestionTypes = await UOW.QuestionTypeRepository.List(QuestionTypeFilter);
                return QuestionTypes;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionTypeService));
            }
            return null;
        }

    }
}
