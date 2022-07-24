using Tracnghiem.Common;
using Tracnghiem.Entities;
using Tracnghiem.Helpers;
using Tracnghiem.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER.Entities;

namespace Tracnghiem.Services.MRole
{
    public interface IPermissionOperatorService : IServiceScoped
    {
        Task<int> Count(PermissionOperatorFilter PermissionOperatorFilter);
        Task<List<PermissionOperator>> List(PermissionOperatorFilter PermissionOperatorFilter);
    }

    public class PermissionOperatorService : BaseService, IPermissionOperatorService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public PermissionOperatorService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(PermissionOperatorFilter PermissionOperatorFilter)
        {
            try
            {
                int result = await UOW.PermissionOperatorRepository.Count(PermissionOperatorFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(PermissionOperatorService));
            }
            return 0;
        }

        public async Task<List<PermissionOperator>> List(PermissionOperatorFilter PermissionOperatorFilter)
        {
            try
            {
                List<PermissionOperator> PermissionOperators = await UOW.PermissionOperatorRepository.List(PermissionOperatorFilter);
                return PermissionOperators;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(PermissionOperatorService));
            }
            return null;
        }
    }
}
