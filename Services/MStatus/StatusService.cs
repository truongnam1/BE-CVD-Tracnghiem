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

namespace Tracnghiem.Services.MStatus
{
    public interface IStatusService :  IServiceScoped
    {
        Task<int> Count(StatusFilter StatusFilter);
        Task<List<Status>> List(StatusFilter StatusFilter);
    }

    public class StatusService : BaseService, IStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public StatusService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
        }

        public async Task<int> Count(StatusFilter StatusFilter)
        {
            try
            {
                int result = await UOW.StatusRepository.Count(StatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(StatusService));
            }
            return 0;
        }

        public async Task<List<Status>> List(StatusFilter StatusFilter)
        {
            try
            {
                List<Status> Statuses = await UOW.StatusRepository.List(StatusFilter);
                return Statuses;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(StatusService));
            }
            return null;
        }

    }
}
