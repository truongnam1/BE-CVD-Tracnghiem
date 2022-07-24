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

namespace Tracnghiem.Services.MGrade
{
    public interface IGradeService :  IServiceScoped
    {
        Task<int> Count(GradeFilter GradeFilter);
        Task<List<Grade>> List(GradeFilter GradeFilter);
    }

    public class GradeService : BaseService, IGradeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        

        public GradeService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
        }

        public async Task<int> Count(GradeFilter GradeFilter)
        {
            try
            {
                int result = await UOW.GradeRepository.Count(GradeFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(GradeService));
            }
            return 0;
        }

        public async Task<List<Grade>> List(GradeFilter GradeFilter)
        {
            try
            {
                List<Grade> Grades = await UOW.GradeRepository.List(GradeFilter);
                return Grades;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(GradeService));
            }
            return null;
        }

    }
}
