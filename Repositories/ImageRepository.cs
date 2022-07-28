using TrueSight.Common;
using Tracnghiem.Common;
using Tracnghiem.Helpers;
using Tracnghiem.Entities;
using Tracnghiem.Models;
using Tracnghiem.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

namespace Tracnghiem.Repositories
{
    public interface IImageRepository
    {
        Task<int> CountAll(ImageFilter ImageFilter);
        Task<int> Count(ImageFilter ImageFilter);
        Task<List<Image>> List(ImageFilter ImageFilter);
        Task<List<Image>> List(List<long> Ids);
        Task<Image> Get(long Id);
        Task<Image> Create(Image Image);
    }
    public class ImageRepository : IImageRepository
    {
        private DataContext DataContext;
        public ImageRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ImageDAO>> DynamicFilter(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Url, filter.Url);
            if (filter.Search != null)
            {
                 query = query.Where(q => 
                    //(filter.SearchBy.Contains(ImageSearch.Code) && q.Code.ToLower().Contains(filter.Search.ToLower())) ||
                    (filter.SearchBy.Contains(ImageSearch.Name) && q.Name.ToLower().Contains(filter.Search.ToLower())));
            }

            return query;
        }

        private async Task<IQueryable<ImageDAO>> OrFilter(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ImageDAO> initQuery = query.Where(q => false);
            foreach (ImageFilter ImageFilter in filter.OrFilter)
            {
                IQueryable<ImageDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ImageFilter.Id);
                queryable = queryable.Where(q => q.Name, ImageFilter.Name);
                queryable = queryable.Where(q => q.Url, ImageFilter.Url);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ImageDAO> DynamicOrder(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ImageOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ImageOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ImageOrder.Url:
                            query = query.OrderBy(q => q.Url);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ImageOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ImageOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ImageOrder.Url:
                            query = query.OrderByDescending(q => q.Url);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Image>> DynamicSelect(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            List<Image> Images = await query.Select(q => new Image()
            {
                Id = filter.Selects.Contains(ImageSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(ImageSelect.Name) ? q.Name : default(string),
                Url = filter.Selects.Contains(ImageSelect.Url) ? q.Url : default(string),
                RowId = q.RowId,
            }).ToListAsync();
            return Images;
        }

        public async Task<int> CountAll(ImageFilter filter)
        {
            IQueryable<ImageDAO> ImageDAOs = DataContext.Image.AsNoTracking();
            ImageDAOs = await DynamicFilter(ImageDAOs, filter);
            return await ImageDAOs.CountAsync();
        }

        public async Task<int> Count(ImageFilter filter)
        {
            IQueryable<ImageDAO> ImageDAOs = DataContext.Image.AsNoTracking();
            ImageDAOs = await DynamicFilter(ImageDAOs, filter);
            ImageDAOs = await OrFilter(ImageDAOs, filter);
            return await ImageDAOs.CountAsync();
        }

        public async Task<List<Image>> List(ImageFilter filter)
        {
            if (filter == null) return new List<Image>();
            IQueryable<ImageDAO> ImageDAOs = DataContext.Image.AsNoTracking();
            ImageDAOs = await DynamicFilter(ImageDAOs, filter);
            ImageDAOs = await OrFilter(ImageDAOs, filter);
            ImageDAOs = DynamicOrder(ImageDAOs, filter);
            List<Image> Images = await DynamicSelect(ImageDAOs, filter);
            return Images;
        }

        public async Task<List<Image>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ImageDAO> query = DataContext.Image.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Image> Images = await query.AsNoTracking()
            .Select(x => new Image()
            {
                RowId = x.RowId,
                Id = x.Id,
                Name = x.Name,
                Url = x.Url,
            }).ToListAsync();
            

            return Images;
        }

        public async Task<Image> Get(long Id)
        {
            Image Image = await DataContext.Image.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new Image()
            {
                Id = x.Id,
                Name = x.Name,
                Url = x.Url,
            }).FirstOrDefaultAsync();

            if (Image == null)
                return null;

            return Image;
        }
        public async Task<Image> Create(Image Image)
        {
            ImageDAO ImageDAO = new ImageDAO();
            ImageDAO.Id = Image.Id;
            ImageDAO.Name = Image.Name;
            ImageDAO.Url = Image.Url;
            ImageDAO.RowId = Guid.NewGuid();
            DataContext.Image.Add(ImageDAO);
            await DataContext.SaveChangesAsync();
            Image.Id = ImageDAO.Id;
            await SaveReference(Image);
            return Image;
        }

        private async Task SaveReference(Image Image)
        {
        }

    }
}
