using Tracnghiem.Entities;
using Tracnghiem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER.Entities;

namespace Tracnghiem.Repositories
{
    public interface IPermissionOperatorRepository
    {
        Task<int> Count(PermissionOperatorFilter PermissionOperatorFilter);
        Task<List<PermissionOperator>> List(PermissionOperatorFilter PermissionOperatorFilter);
        Task<PermissionOperator> Get(long Id);
    }
    public class PermissionOperatorRepository : IPermissionOperatorRepository
    {
        private DataContext DataContext;
        public PermissionOperatorRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PermissionOperatorDAO> DynamicFilter(IQueryable<PermissionOperatorDAO> query, PermissionOperatorFilter filter)
        {
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.FieldTypeId, filter.FieldTypeId);
            return query;
        }


        private async Task<List<PermissionOperator>> DynamicSelect(IQueryable<PermissionOperatorDAO> query, PermissionOperatorFilter filter)
        {
            query = query.OrderBy(q => q.Id).Skip(0).Take(int.MaxValue);
            List<PermissionOperator> PermissionOperators = await query.Select(q => new PermissionOperator()
            {
                Id = q.Id,
                Code = q.Code,
                Name = q.Name,
                FieldTypeId = q.FieldTypeId,
            }).ToListAsync();
            return PermissionOperators;
        }

        public async Task<int> Count(PermissionOperatorFilter filter)
        {
            IQueryable<PermissionOperatorDAO> PermissionOperators = DataContext.PermissionOperator.AsNoTracking();
            PermissionOperators = DynamicFilter(PermissionOperators, filter);
            return await PermissionOperators.CountAsync();
        }

        public async Task<List<PermissionOperator>> List(PermissionOperatorFilter filter)
        {
            if (filter == null) return new List<PermissionOperator>();
            IQueryable<PermissionOperatorDAO> PermissionOperatorDAOs = DataContext.PermissionOperator.AsNoTracking();
            PermissionOperatorDAOs = DynamicFilter(PermissionOperatorDAOs, filter);
            List<PermissionOperator> PermissionOperators = await DynamicSelect(PermissionOperatorDAOs, filter);
            return PermissionOperators;
        }

        public async Task<PermissionOperator> Get(long Id)
        {
            PermissionOperator PermissionOperator = await DataContext.PermissionOperator.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new PermissionOperator()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                FieldTypeId = x.FieldTypeId,
            }).FirstOrDefaultAsync();

            return PermissionOperator;
        }
    }
}
