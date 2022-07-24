using Tracnghiem.Entities;
using Tracnghiem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER.Entities;

namespace Tracnghiem.Repositories
{
    public interface IFieldRepository
    {
        Task<int> Count(FieldFilter FieldFilter);
        Task<List<Field>> List(FieldFilter FieldFilter);
        Task<List<Field>> List(List<long> Ids);
        Task<bool> BulkMerge(List<Field> Fields);
        Task<bool> Update(Field Field);
        Task<Field> Get(long Id);
    }
    public class FieldRepository : IFieldRepository
    {
        private DataContext DataContext;
        public FieldRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<FieldDAO> DynamicFilter(IQueryable<FieldDAO> query, FieldFilter filter)
        {
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.MenuId, filter.MenuId);
            query = query.Where(q => q.FieldTypeId, filter.FieldTypeId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<FieldDAO> OrFilter(IQueryable<FieldDAO> query, FieldFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<FieldDAO> initQuery = query.Where(q => false);
            foreach (FieldFilter FieldFilter in filter.OrFilter)
            {
                IQueryable<FieldDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, FieldFilter.Id);
                query = query.Where(q => q.FieldTypeId, filter.FieldTypeId);
                query = query.Where(q => q.MenuId, filter.MenuId);
                queryable = queryable.Where(q => q.Name, FieldFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<FieldDAO> DynamicOrder(IQueryable<FieldDAO> query, FieldFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case FieldOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case FieldOrder.Menu:
                            query = query.OrderBy(q => q.Menu.Name);
                            break;
                        case FieldOrder.FieldType:
                            query = query.OrderBy(q => q.FieldType.Name);
                            break;
                        case FieldOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case FieldOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case FieldOrder.Menu:
                            query = query.OrderByDescending(q => q.Menu.Name);
                            break;
                        case FieldOrder.FieldType:
                            query = query.OrderByDescending(q => q.FieldType.Name);
                            break;
                        case FieldOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Field>> DynamicSelect(IQueryable<FieldDAO> query, FieldFilter filter)
        {
            List<Field> Fields = await query.Select(q => new Field()
            {
                Id = filter.Selects.Contains(FieldSelect.Id) ? q.Id : default(long),
                FieldTypeId = filter.Selects.Contains(FieldSelect.FieldType) ? q.FieldTypeId : default(long),
                MenuId = filter.Selects.Contains(FieldSelect.Menu) ? q.MenuId : default(long),
                Name = filter.Selects.Contains(FieldSelect.Name) ? q.Name : default(string),
                Menu = filter.Selects.Contains(FieldSelect.Menu) && q.Menu != null ? new Menu
                {
                    Id = q.Menu.Id,
                    Code = q.Menu.Code,
                    Name = q.Menu.Name,
                    SiteId = q.Menu.SiteId,
                    IsDeleted = q.Menu.IsDeleted,
                    Site = q.Menu.Site == null ? null : new Site
                    {
                        Id = q.Menu.Site.Id,
                        Code = q.Menu.Site.Code,
                        Name = q.Menu.Site.Name,
                        Description = q.Menu.Site.Description,
                        IsDisplay = q.Menu.Site.IsDisplay
                    }
                } : null,
                FieldType = filter.Selects.Contains(FieldSelect.FieldType) && q.FieldType != null ? new FieldType
                {
                    Id = q.Menu.Id,
                    Code = q.Menu.Code,
                    Name = q.Menu.Name,
                } : null,
            }).ToListAsync();

            if (filter.Selects.Contains(FieldSelect.FieldFieldEntityMapping))
            {
                var FieldFilter = new IdFilter { In = Fields.Select(x => x.Id).ToList() };
                //var FieldFieldEntityMappings = await DataContext.FieldFieldEntityMapping.AsNoTracking()
                //    .Where(x => x.FieldId, FieldFilter).Select(x => new FieldFieldEntityMapping
                //    {
                //        FieldId = x.FieldId,
                //        FieldEntityId = x.FieldEntityId,
                //        FieldEntity = new FieldEntity
                //        {
                //            Id = x.FieldEntity.Id,
                //            EntityName = x.FieldEntity.EntityName,
                //            EntityKey = x.FieldEntity.EntityKey,
                //            EntityDisplay = x.FieldEntity.EntityDisplay,
                //        }
                //    }).ToListAsync();
                //foreach (var field in Fields)
                //{
                //    field.FieldFieldEntityMappings = FieldFieldEntityMappings.Where(x => x.FieldId == field.Id).ToList();
                //}
            }

            return Fields;
        }

        public async Task<int> Count(FieldFilter filter)
        {
            IQueryable<FieldDAO> Fields = DataContext.Field.AsNoTracking();
            Fields = DynamicFilter(Fields, filter);
            return await Fields.CountAsync();
        }

        public async Task<List<Field>> List(List<long> Ids)
        {
            IdFilter filter = new IdFilter { In = Ids };
            List<Field> Fields = await DataContext.Field.AsNoTracking()
                .Where(x => x.Id, filter).Select(x => new Field()
                {
                    Id = x.Id,
                    Name = x.Name,
                    MenuId = x.MenuId,
                    FieldTypeId = x.FieldTypeId,
                    Menu = x.Menu == null ? null : new Menu
                    {
                        Id = x.Menu.Id,
                        Code = x.Menu.Code,
                        Name = x.Menu.Name,
                        SiteId = x.Menu.SiteId,
                        IsDeleted = x.Menu.IsDeleted,
                        Site = x.Menu.Site == null ? null : new Site
                        {
                            Id = x.Menu.Site.Id,
                            Code = x.Menu.Site.Code,
                            Name = x.Menu.Site.Name,
                            Description = x.Menu.Site.Description,
                            IsDisplay = x.Menu.Site.IsDisplay,
                        }
                    },
                    FieldType = x.FieldType == null ? null : new FieldType
                    {
                        Id = x.Menu.Id,
                        Code = x.Menu.Code,
                        Name = x.Menu.Name,
                    },
                }).ToListAsync();

            //List<FieldFieldEntityMapping> FieldFieldEntityMappings = await DataContext.FieldFieldEntityMapping.AsNoTracking()
            //    .Where(x => x.FieldId, filter).Select(x => new FieldFieldEntityMapping
            //    {
            //        FieldId = x.FieldId,
            //        FieldEntityId = x.FieldEntityId,
            //        FieldEntity = x.FieldEntity == null ? null : new FieldEntity
            //        {
            //            Id = x.FieldEntity.Id,
            //            FieldName = x.FieldEntity.FieldName,
            //            EntityName = x.FieldEntity.EntityName,
            //        }
            //    }).ToListAsync();

            //foreach (var Field in Fields)
            //{
            //    Field.FieldFieldEntityMappings = FieldFieldEntityMappings.Where(x => x.FieldId == Field.Id).ToList();
            //}

            return Fields;
        }

        public async Task<List<Field>> List(FieldFilter filter)
        {
            if (filter == null) return new List<Field>();
            IQueryable<FieldDAO> FieldDAOs = DataContext.Field.AsNoTracking();
            FieldDAOs = DynamicFilter(FieldDAOs, filter);
            FieldDAOs = DynamicOrder(FieldDAOs, filter);
            List<Field> Fields = await DynamicSelect(FieldDAOs, filter);
            return Fields;
        }
        public async Task<bool> Update(Field Field)
        {
            FieldDAO FieldDAO = DataContext.Field.Where(x => x.Id == Field.Id).FirstOrDefault();
            if (FieldDAO == null)
                return false;
            FieldDAO.Id = Field.Id;
            FieldDAO.Name = Field.Name;
            FieldDAO.FieldTypeId = Field.FieldTypeId;
            FieldDAO.MenuId = Field.MenuId;
            FieldDAO.IsDeleted = Field.IsDeleted;
            await DataContext.SaveChangesAsync();
            await SaveReference(Field);
            return true;
        }
        public async Task<bool> BulkMerge(List<Field> Fields)
        {
            List<FieldDAO> FieldDAOs = Fields.Select(x => new FieldDAO
            {
                Id = x.Id,
                Name = x.Name,
                FieldTypeId = x.FieldTypeId,
                MenuId = x.MenuId
            }).ToList();
            await DataContext.BulkMergeAsync(FieldDAOs);
            return true;
        }
        public async Task<Field> Get(long Id)
        {
            Field Field = await DataContext.Field.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Field()
                {
                    Id = x.Id,
                    Name = x.Name,
                    MenuId = x.MenuId,
                    FieldTypeId = x.FieldTypeId,
                    Menu = x.Menu == null ? null : new Menu
                    {
                        Id = x.Menu.Id,
                        Code = x.Menu.Code,
                        Name = x.Menu.Name,
                        SiteId = x.Menu.SiteId,
                        IsDeleted = x.Menu.IsDeleted,
                        Site = x.Menu.Site == null ? null : new Site
                        {
                            Id = x.Menu.Site.Id,
                            Code = x.Menu.Site.Code,
                            Name = x.Menu.Site.Name,
                            Description = x.Menu.Site.Description,
                            IsDisplay = x.Menu.Site.IsDisplay,
                        } 
                    },
                    FieldType = x.FieldType == null ? null : new FieldType
                    {
                        Id = x.Menu.Id,
                        Code = x.Menu.Code,
                        Name = x.Menu.Name,
                    },
                }).FirstOrDefaultAsync();

            //var FieldFieldEntityMappings = await DataContext.FieldFieldEntityMapping.AsNoTracking()
            //    .Where(x => x.FieldId == Id).Select(x => new FieldFieldEntityMapping
            //    {
            //        FieldId = x.FieldId,
            //        FieldEntityId = x.FieldEntityId,
            //        FieldEntity = new FieldEntity
            //        {
            //            Id = x.FieldEntity.Id,
            //            EntityKey = x.FieldEntity.EntityKey,
            //            EntityDisplay = x.FieldEntity.EntityDisplay,
            //        }
            //    }).ToListAsync();
            //Field.FieldFieldEntityMappings = FieldFieldEntityMappings;

            return Field;
        }
        private async Task SaveReference(Field Field)
        {
        }
    }
}
