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
    public interface IFieldService : IServiceScoped
    {
        Task<int> Count(FieldFilter FieldFilter);
        Task<List<Field>> List(FieldFilter FieldFilter);
        Task<List<Field>> Import(List<Field> Fields);
        Task<Field> Update(Field Field);
        Task<Field> Get(long Id);
    }

    public class FieldService : BaseService, IFieldService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IFieldValidator FieldValidator;

        public FieldService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IFieldValidator FieldValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.FieldValidator = FieldValidator;
        }
        public async Task<int> Count(FieldFilter FieldFilter)
        {
            try
            {
                int result = await UOW.FieldRepository.Count(FieldFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FieldService));
            }
            return 0;
        }

        public async Task<List<Field>> List(FieldFilter FieldFilter)
        {
            try
            {
                List<Field> Fields = await UOW.FieldRepository.List(FieldFilter);
                return Fields;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FieldService));
            }
            return null;
        }
        public async Task<Field> Update(Field Field)
        {
            if (!await FieldValidator.Update(Field))
                return Field;
            try
            {
                var oldData = await UOW.FieldRepository.Get(Field.Id);

                await UOW.Begin();
                await UOW.FieldRepository.Update(Field);
                await UOW.Commit();

                var newData = await UOW.FieldRepository.Get(Field.Id);
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    Logging.CreateSystemLog(ex, nameof(FieldService));
                    throw new MessageException(ex);
                }
                else
                {
                    Logging.CreateSystemLog(ex.InnerException, nameof(FieldService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Field> Get(long Id)
        {
            Field Field = await UOW.FieldRepository.Get(Id);
            if (Field == null)
                return null;
            return Field;
        }
        public async Task<List<Field>> Import(List<Field> Fields)
        {
            try
            {
                await UOW.Begin();
                await UOW.FieldRepository.BulkMerge(Fields);
                var listFieldsInDb = await UOW.FieldRepository.List(new FieldFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = FieldSelect.ALL
                });

                return Fields;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    Logging.CreateSystemLog(ex, nameof(FieldService));
                    throw new MessageException(ex);
                }
                else
                {
                    Logging.CreateSystemLog(ex.InnerException, nameof(FieldService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
    }
}
