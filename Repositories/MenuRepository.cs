using Tracnghiem.Entities;
using Tracnghiem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER.Entities;
using Action = TrueSight.PER.Entities.Action;
using Microsoft.Extensions.Configuration;

namespace Tracnghiem.Repositories
{
    public interface IMenuRepository
    {
        Task<int> Count(MenuFilter MenuFilter);
        Task<List<Menu>> List(MenuFilter MenuFilter);
        Task<Menu> Get(long Id);
        Task<bool> BulkMerge(Site Site);
    }
    public class MenuRepository : CacheRepository, IMenuRepository
    {
        private DataContext DataContext;
        public MenuRepository(DataContext DataContext, IRedisStore RedisStore, IConfiguration Configuration)
            : base(DataContext, RedisStore, Configuration)
        {
            this.DataContext = DataContext;
        }

    private IQueryable<MenuDAO> DynamicFilter(IQueryable<MenuDAO> query, MenuFilter filter)
        {
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.SiteId, filter.SiteId);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<MenuDAO> OrFilter(IQueryable<MenuDAO> query, MenuFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<MenuDAO> initQuery = query.Where(q => false);
            foreach (MenuFilter MenuFilter in filter.OrFilter)
            {
                IQueryable<MenuDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, MenuFilter.Id);
                queryable = queryable.Where(q => q.SiteId, MenuFilter.SiteId);
                queryable = queryable.Where(q => q.Code, MenuFilter.Code);
                queryable = queryable.Where(q => q.Name, MenuFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<MenuDAO> DynamicOrder(IQueryable<MenuDAO> query, MenuFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case MenuOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case MenuOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case MenuOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        //case MenuOrder.Path:
                        //    query = query.OrderBy(q => q.Path);
                        //    break;
                        case MenuOrder.IsDeleted:
                            query = query.OrderBy(q => q.IsDeleted);
                            break;
                        //case MenuOrder.Site:
                        //    query = query.OrderBy(q => q.Site.Name);
                        //    break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case MenuOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case MenuOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case MenuOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        //case MenuOrder.Path:
                        //    query = query.OrderByDescending(q => q.Path);
                        //    break;
                        case MenuOrder.IsDeleted:
                            query = query.OrderByDescending(q => q.IsDeleted);
                            break;
                        //case MenuOrder.Site:
                        //    query = query.OrderByDescending(q => q.Site.Name);
                        //    break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Menu>> DynamicSelect(IQueryable<MenuDAO> query, MenuFilter filter)
        {
            List<Menu> Menus = await query.Select(q => new Menu()
            {
                Id = filter.Selects.Contains(MenuSelect.Id) ? q.Id : default(long),
                SiteId = filter.Selects.Contains(MenuSelect.Site) ? q.SiteId : default(long),
                Code = filter.Selects.Contains(MenuSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(MenuSelect.Name) ? q.Name : default(string),
                IsDeleted = filter.Selects.Contains(MenuSelect.IsDeleted) ? q.IsDeleted : default(bool),
                //Site = filter.Selects.Contains(MenuSelect.Site) && q.Site != null ? new Site
                //{
                //    Id = q.Site.Id,
                //    Code = q.Site.Code,
                //    Name = q.Site.Name,
                //    Description = q.Site.Description,
                //    IsDisplay = q.Site.IsDisplay,
                //    RowId = q.Site.RowId,
                //} : null,
            }).ToListAsync();
            return Menus;
        }

        public async Task<int> Count(MenuFilter filter)
        {
            IQueryable<MenuDAO> Menus = DataContext.Menu;
            Menus = DynamicFilter(Menus, filter);
            return await Menus.CountAsync();
        }

        public async Task<List<Menu>> List(MenuFilter filter)
        {
            if (filter == null) return new List<Menu>();
            IQueryable<MenuDAO> MenuDAOs = DataContext.Menu.AsNoTracking();
            MenuDAOs = DynamicFilter(MenuDAOs, filter);
            MenuDAOs = DynamicOrder(MenuDAOs, filter);
            List<Menu> Menus = await DynamicSelect(MenuDAOs, filter);
            return Menus;
        }

        public async Task<Menu> Get(long Id)
        {
            Menu Menu = await DataContext.Menu.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Menu()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    SiteId = x.SiteId,
                    IsDeleted = x.IsDeleted,
                    //Site = x.Site == null ? null : new Site
                    //{
                    //    Id = x.Site.Id,
                    //    Code = x.Site.Code,
                    //    Name = x.Site.Name,
                    //    Description = x.Site.Description,
                    //    IsDisplay = x.Site.IsDisplay,
                    //    RowId = x.Site.RowId,
                    //}
                }).FirstOrDefaultAsync();

            if (Menu == null)
                return null;
            Menu.Fields = await DataContext.Field
                .Where(x => x.MenuId == Menu.Id)
                .Select(x => new Field
                {
                    Id = x.Id,
                    Name = x.Name,
                    MenuId = x.MenuId,
                    IsDeleted = x.IsDeleted,
                }).ToListAsync();
            Menu.Actions = await DataContext.Action
                .Where(x => x.MenuId == Menu.Id)
                .Select(x => new Action
                {
                    Id = x.Id,
                    Name = x.Name,
                    MenuId = x.MenuId,
                    IsDeleted = x.IsDeleted,
                }).ToListAsync();
            return Menu;
        }
        public async Task<bool> BulkMerge(Site Site)
        {
            #region Menu
            List<SiteDAO> SiteDAOs = await DataContext.Site.ToListAsync();
            long SiteId = SiteDAOs.Where(x => x.Code == Site.Code).Select(x => x.Id).FirstOrDefault();

            List<Menu> Menus = Site.Menus;
            List<MenuDAO> DbMenuDAOs = DataContext.Menu.Where(x => x.SiteId == SiteId).ToList();
            DbMenuDAOs.ForEach(x => x.IsDeleted = true);
            foreach (Menu Menu in Menus)
            {
                MenuDAO oldMenu = DbMenuDAOs
                    .Where(m => m.Code == Menu.Code && m.Name != "Root" && m.SiteId == SiteId)
                    .FirstOrDefault();
                if (oldMenu == null)
                {
                    oldMenu = new MenuDAO()
                    {
                        Code = Menu.Code,
                        Name = Menu.Name,
                        IsDeleted = false,
                        SiteId = SiteId,
                    };
                    DbMenuDAOs.Add(oldMenu);
                }
                else
                {
                    oldMenu.Name = Menu.Name;
                    oldMenu.IsDeleted = false;
                }
            }
            await DataContext.BulkMergeAsync(DbMenuDAOs);
            DbMenuDAOs = DbMenuDAOs.Where(x => x.IsDeleted == false).ToList();

            Menus.ForEach(x => x.Id = DbMenuDAOs.Where(m => m.Code == x.Code).Select(m => m.Id).FirstOrDefault());
            foreach (var menu in Menus)
            {
                if (menu.Fields != null)
                    menu.Fields.ForEach(x => x.MenuId = menu.Id);
                if (menu.Actions != null)
                {
                    menu.Actions.ForEach(x => x.MenuId = menu.Id);
                    menu.Actions.ForEach(x => x.Pages.ForEach(x => x.MenuId = menu.Id));
                }
                if (menu.Pages != null)
                    menu.Pages.ForEach(x => x.MenuId = menu.Id);

            };
            #endregion

            List<long> MenuIds = Menus.Select(x => x.Id).ToList();

            #region Field
            List<FieldDAO> DbFieldDAOs = DataContext.Field
                .Where(x => MenuIds.Contains(x.MenuId))
                .ToList();
            DbFieldDAOs.ForEach(x => x.IsDeleted = true);

            foreach (Menu Menu in Menus)
            {
                long MenuId = Menu.Id;
                List<Field> Fields = Menu.Fields;
                if (Fields != null)
                {
                    foreach (Field Field in Fields)
                    {
                        FieldDAO oldField = DbFieldDAOs
                            .Where(p => p.MenuId == MenuId && p.Name == Field.Name)
                            .FirstOrDefault();
                        if (oldField == null)
                        {
                            oldField = new FieldDAO
                            {
                                Name = Field.Name,
                                FieldTypeId = Field.FieldTypeId,
                                IsDeleted = false,
                                MenuId = MenuId,
                                //TranslatedName = Field.Name,
                            };
                            DbFieldDAOs.Add(oldField);
                        }
                        else
                        {
                            oldField.FieldTypeId = Field.FieldTypeId;
                            oldField.IsDeleted = false;
                        }
                    }
                }
            }
            await DataContext.BulkMergeAsync(DbFieldDAOs);
            DbFieldDAOs = DbFieldDAOs.Where(x => x.IsDeleted == false).ToList();
            //Update Field
            Menus.ForEach(x =>
            {
                //Field
                if (x.Fields != null)
                    x.Fields.ForEach(p => p.Id = DbFieldDAOs
                    .Where(a => a.Name == p.Name && a.MenuId == x.Id)
                    .Select(a => a.Id).FirstOrDefault());
            });
            #endregion

            #region Page
            List<PageDAO> DbPageDAOs = DataContext.Page
                .Where(x => MenuIds.Contains(x.MenuId))
                .ToList();
            DbPageDAOs.ForEach(x => x.IsDeleted = true);

            foreach (Menu Menu in Menus)
            {
                long MenuId = Menu.Id;
                List<Page> Pages = Menu.Pages;
                if (Pages != null)
                {
                    foreach (Page page in Pages)
                    {
                        PageDAO oldPage = DbPageDAOs
                            .Where(p => p.Path == page.Path && p.MenuId == MenuId)
                            .FirstOrDefault();
                        if (oldPage == null)
                        {
                            oldPage = new PageDAO
                            {
                                Name = page.Name,
                                Path = page.Path,
                                IsDeleted = false,
                                MenuId = MenuId,
                            };
                            DbPageDAOs.Add(oldPage);
                        }
                        else
                        {
                            oldPage.IsDeleted = false;
                        }
                    }
                }
            }
            await DataContext.BulkMergeAsync(DbPageDAOs);
            DbPageDAOs = DbPageDAOs.Where(x => x.IsDeleted == false).ToList();

            //Update Page
            Menus.ForEach(x =>
            {
                if (x.Pages != null)
                    x.Pages.ForEach(p => p.Id = DbPageDAOs
                    .Where(a => a.Path == p.Path && a.MenuId == x.Id)
                    .Select(a => a.Id).FirstOrDefault());
            });
            #endregion

            #region Action
            List<ActionDAO> DbActionDAOs = DataContext.Action
                .Where(x => MenuIds.Contains(x.MenuId))
                .ToList();
            DbActionDAOs.ForEach(x => x.IsDeleted = true);

            foreach (Menu Menu in Menus)
            {
                long MenuId = Menu.Id;
                List<Action> Actions = Menu.Actions;
                if (Actions != null)
                {
                    foreach (Action Action in Actions)
                    {
                        ActionDAO oldAction = DbActionDAOs
                            .Where(p => p.MenuId == MenuId && p.Name == Action.Name)
                            .FirstOrDefault();
                        if (oldAction == null)
                        {
                            oldAction = new ActionDAO
                            {
                                MenuId = MenuId,
                                Name = Action.Name,
                                IsDeleted = false,
                            };
                            DbActionDAOs.Add(oldAction);
                        }
                        else
                        {
                            oldAction.IsDeleted = false;
                        }

                    }
                }
            }
            await DataContext.BulkMergeAsync(DbActionDAOs);
            DbActionDAOs = DbActionDAOs.Where(x => x.IsDeleted == false).ToList();

            //Update Action
            Menus.ForEach(x =>
            {
                if (x.Actions != null)
                    x.Actions.ForEach(p =>
                    {
                        //ActionId
                        p.Id = DbActionDAOs
                        .Where(a => a.Name == p.Name && a.MenuId == x.Id)
                        .Select(a => a.Id).FirstOrDefault();

                        //ActionPageMappng
                        p.Pages.ForEach(a => a.Id = DbPageDAOs
                        .Where(b => b.Path == a.Path && b.MenuId == x.Id)
                        .Select(c => c.Id).FirstOrDefault());
                    });
            });
            #endregion

            #region ActionPageMapping
            List<long> ActionIds = DbActionDAOs.Select(x => x.Id).ToList();
            List<long> PageIds = DbPageDAOs.Select(x => x.Id).ToList();
            List<ActionPageMappingDAO> NewActionPageMappingDAOs = new List<ActionPageMappingDAO>();
            await DataContext.ActionPageMapping
                .Where(x => ActionIds.Contains(x.ActionId) || PageIds.Contains(x.PageId))
                .DeleteFromQueryAsync();
            foreach (Menu Menu in Menus)
            {
                if (Menu.Actions != null)
                {
                    List<Action> Actions = Menu.Actions;
                    foreach (var Action in Actions)
                    {
                        List<Page> Pages = Action.Pages;
                        foreach (var Page in Pages)
                        {
                            ActionPageMappingDAO ActionPageMappingDAO = new ActionPageMappingDAO
                            {
                                ActionId = Action.Id,
                                PageId = Page.Id,
                            };
                            if (NewActionPageMappingDAOs.Where(x => x.ActionId == Action.Id && x.PageId == Page.Id).Count() == 0)
                                NewActionPageMappingDAOs.Add(ActionPageMappingDAO);
                        }
                    }
                }
            }
            await DataContext.BulkMergeAsync(NewActionPageMappingDAOs);
            #endregion

            await DataContext.ActionPageMapping.Where(ap => ap.Action.IsDeleted || ap.Page.IsDeleted || ap.Action.Menu.IsDeleted).DeleteFromQueryAsync();
            await DataContext.PermissionActionMapping.Where(ap => ap.Action.IsDeleted || ap.Action.Menu.IsDeleted || ap.Permission.Menu.IsDeleted).DeleteFromQueryAsync();
            await DataContext.Action.Where(p => p.IsDeleted || p.Menu.IsDeleted).DeleteFromQueryAsync();
            //await DataContext.Page.Where(p => p.IsDeleted || p.Menu.IsDeleted).DeleteFromQueryAsync();
            await DataContext.PermissionContent.Where(f => f.Field.IsDeleted == true || f.Field.Menu.IsDeleted).DeleteFromQueryAsync();
            //await DataContext.FieldFieldEntityMapping.Where(x => x.Field.IsDeleted == true || x.Field.Menu.IsDeleted == true).DeleteFromQueryAsync();
            await DataContext.Field.Where(pf => pf.IsDeleted || pf.Menu.IsDeleted).DeleteFromQueryAsync();
            await DataContext.Permission.Where(p => p.Menu.IsDeleted).DeleteFromQueryAsync();
            await DataContext.Menu.Where(v => v.IsDeleted).DeleteFromQueryAsync();
            await RemoveFromCache(nameof(RoleRepository));
            await RemoveFromCache(nameof(PermissionRepository));

            return true;
        }

    }
}
