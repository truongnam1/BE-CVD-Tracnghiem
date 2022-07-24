using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tracnghiem.Entities;
using Tracnghiem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER.Entities;
using Action = TrueSight.PER.Entities.Action;

namespace Tracnghiem.Repositories
{
    public interface ISiteRepository
    {
        Task<int> Count(SiteFilter SiteFilter);
        Task<List<Site>> List(SiteFilter SiteFilter);
        Task<Site> Get(long Id);
        Task<bool> BulkMerge(List<Site> Sites);
        Task<bool> Update(Site Site);
        Task<bool> Create(Site Site);
    }
    public class SiteRepository : CacheRepository, ISiteRepository
    {
        private DataContext DataContext;
        public SiteRepository(DataContext DataContext, IRedisStore RedisStore, IConfiguration Configuration)
            : base(DataContext, RedisStore, Configuration)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<SiteDAO> DynamicFilter(IQueryable<SiteDAO> query, SiteFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Description, filter.Description);
            query = query.Where(q => q.IsDisplay, filter.IsDisplay);
            return query;
        }

        private IQueryable<SiteDAO> DynamicOrder(IQueryable<SiteDAO> query, SiteFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case SiteOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case SiteOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case SiteOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case SiteOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case SiteOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case SiteOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Site>> DynamicSelect(IQueryable<SiteDAO> query, SiteFilter filter)
        {
            List<Site> Sites = await query.Select(q => new Site()
            {
                Id = q.Id,
                Code = q.Code,
                Name = q.Name,
                Description = q.Description
            }).ToListAsync();

            return Sites;
        }

        public async Task<int> Count(SiteFilter filter)
        {
            IQueryable<SiteDAO> Sites = DataContext.Site.AsNoTracking();
            Sites = DynamicFilter(Sites, filter);
            return await Sites.CountAsync();
        }

        public async Task<List<Site>> List(SiteFilter filter)
        {
            string key = $"{nameof(SiteRepository)}.{nameof(SiteRepository.List)}.{filter.ToHash()}";
            List<Site> Sites = await GetFromCache<List<Site>>(key);
            if (Sites == null)
            {
                if (filter == null) return new List<Site>();
                IQueryable<SiteDAO> SiteDAOs = DataContext.Site.AsNoTracking();
                SiteDAOs = DynamicFilter(SiteDAOs, filter);
                SiteDAOs = DynamicOrder(SiteDAOs, filter);
                Sites = await DynamicSelect(SiteDAOs, filter);
                await SetToCache(key, Sites);
            }
            return Sites;
        }

        public async Task<Site> Get(long Id)
        {
            Site Site = await DataContext.Site.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Site()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    IsDisplay = x.IsDisplay,
                }).FirstOrDefaultAsync();

            if (Site == null)
                return null;

            Site.Menus = await DataContext.Menu
                .Where(x => x.SiteId == Site.Id)
                .Select(x => new Menu
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    SiteId = x.SiteId,
                    IsDeleted = x.IsDeleted,
                }).ToListAsync();

            List<long> MenuIds = Site.Menus.Select(x => x.Id).ToList();

            List<Field> Fields = await DataContext.Field.AsNoTracking()
                .Where(x => x.MenuId, new IdFilter { In = MenuIds })
                .Select(x => new Field
                {
                    Id = x.Id,
                    Name = x.Name,
                    FieldTypeId = x.FieldTypeId,
                    MenuId = x.MenuId
                }).ToListAsync();

            List<Page> Pages = await DataContext.Page.AsNoTracking()
                .Where(x => x.MenuId, new IdFilter { In = MenuIds })
                .Select(x => new Page
                {
                    Id = x.Id,
                    Name = x.Name,
                    Path = x.Path,
                    MenuId = x.MenuId
                }).ToListAsync();

            List<Action> Actions = await DataContext.Action.AsNoTracking()
                .Where(x => x.MenuId, new IdFilter { In = MenuIds })
                .Select(x => new Action
                {
                    Id = x.Id,
                    MenuId = x.MenuId,
                    Name = x.Name,
                }).ToListAsync();

            List<ActionPageMapping> ActionPageMappings = await DataContext.ActionPageMapping.AsNoTracking()
                .Where(x => x.ActionId, new IdFilter { In = Actions.Select(x => x.Id).Distinct().ToList() })
                .Select(x => new ActionPageMapping
                {
                    ActionId = x.ActionId,
                    PageId = x.PageId,
                }).ToListAsync();

            foreach (var action in Actions)
            {
                action.Pages = ActionPageMappings.Where(x => x.ActionId == action.Id).Select(x => new Page
                {
                    Id = x.PageId,
                }).ToList();
            }

            foreach (var Menu in Site.Menus)
            {
                Menu.Fields = Fields.Where(x => x.MenuId == Menu.Id).ToList();
                Menu.Pages = Pages.Where(x => x.MenuId == Menu.Id).ToList();
                Menu.Actions = Actions.Where(x => x.MenuId == Menu.Id).ToList();
            }

            return Site;
        }

        public async Task<bool> Create(Site Site)
        {
            List<long> Ids = await DataContext.Site.Where(x => x.Id < 100).Select(x => x.Id).ToListAsync();
            SiteDAO SiteDAO = new SiteDAO();
            SiteDAO.Id = Ids.Max() + 1;
            SiteDAO.Code = Site.Code;
            SiteDAO.Name = Site.Name;
            SiteDAO.Description = Site.Description;
            DataContext.Site.Add(SiteDAO);
            await DataContext.SaveChangesAsync();
            await RemoveFromCache(nameof(Site));
            Site.Id = SiteDAO.Id;
            return true;
        }

        public async Task<bool> Update(Site Site)
        {
            SiteDAO SiteDAO = DataContext.Site.Where(x => x.Id == Site.Id).FirstOrDefault();
            if (SiteDAO == null)
                return false;
            SiteDAO.Name = Site.Name;
            SiteDAO.Description = Site.Description;
            await DataContext.SaveChangesAsync();
            await RemoveFromCache(nameof(Site));
            return true;
        }
        public async Task<bool> BulkMerge(List<Site> Sites)
        {
            IdFilter IdFilter = new IdFilter { In = Sites.Select(x => x.Id).ToList() };
            List<SiteDAO> SiteDAOs = new List<SiteDAO>();
            List<SiteDAO> DbSiteDAOs = await DataContext.Site
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Site Site in Sites)
            {
                SiteDAO SiteDAO = DbSiteDAOs
                        .Where(x => x.Id == Site.Id)
                        .FirstOrDefault();
                if (SiteDAO == null)
                {
                    SiteDAO = new SiteDAO();
                }
                SiteDAO.Code = Site.Code;
                SiteDAO.Name = Site.Name;
                SiteDAO.Description = Site.Description;
                SiteDAOs.Add(SiteDAO);
            }
            await DataContext.BulkMergeAsync(SiteDAOs);
            return true;
        }
    }
}
