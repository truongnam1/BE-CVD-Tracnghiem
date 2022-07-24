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

namespace Tracnghiem.Services.MQuestionGroup
{
    public interface IQuestionGroupService :  IServiceScoped
    {
        Task<int> Count(QuestionGroupFilter QuestionGroupFilter);
        Task<List<QuestionGroup>> List(QuestionGroupFilter QuestionGroupFilter);
    }

    public class QuestionGroupService : BaseService, IQuestionGroupService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public QuestionGroupService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
        }

        public async Task<int> Count(QuestionGroupFilter QuestionGroupFilter)
        {
            try
            {
                int result = await UOW.QuestionGroupRepository.Count(QuestionGroupFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionGroupService));
            }
            return 0;
        }

        public async Task<List<QuestionGroup>> List(QuestionGroupFilter QuestionGroupFilter)
        {
            try
            {
                List<QuestionGroup> QuestionGroups = await UOW.QuestionGroupRepository.List(QuestionGroupFilter);
                return QuestionGroups;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionGroupService));
            }
            return null;
        }

    }
}
