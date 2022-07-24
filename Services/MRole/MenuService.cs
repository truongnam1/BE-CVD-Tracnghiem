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
    public interface IMenuService : IServiceScoped
    {
        Task<int> Count(MenuFilter MenuFilter);
        Task<List<Menu>> List(MenuFilter MenuFilter);
        Task<Menu> Get(long Id);
        MenuFilter ToFilter(MenuFilter MenuFilter);
    }

    public class MenuService : BaseService, IMenuService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public MenuService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(MenuFilter MenuFilter)
        {
            try
            {
                int result = await UOW.MenuRepository.Count(MenuFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MenuService));
            }
            return 0;
        }

        public async Task<List<Menu>> List(MenuFilter MenuFilter)
        {
            try
            {
                List<Menu> Menus = await UOW.MenuRepository.List(MenuFilter);
                return Menus;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(MenuService));
            }
            return null;
        }
        public async Task<Menu> Get(long Id)
        {
            Menu Menu = await UOW.MenuRepository.Get(Id);
            if (Menu == null)
                return null;
            return Menu;
        }

        public MenuFilter ToFilter(MenuFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<MenuFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                MenuFilter subFilter = new MenuFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                }
            }
            return filter;
        }
    }
}
