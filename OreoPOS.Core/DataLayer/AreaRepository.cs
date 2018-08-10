using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OreoPOS.Core.EntityLayer;

namespace OreoPOS.Core.DataLayer
{
    public class AreaRepository : IAreaRepository
    {
        private readonly PosWorksDbContext DbContext;
        private Boolean Disposed;
        public AreaRepository(PosWorksDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<Area> AddAreaAsync(Area entity)
        {
            entity.CreatedOn = DateTime.Now;
            DbContext.Areas.Add(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Area> DeleteAreaAsync(Area changes)
        {
            var entity = await GetAreaAsync(changes);

            if (entity != null)
            {
                DbContext.Set<Area>().Remove(entity);

                await DbContext.SaveChangesAsync();
            }

            return entity;
        }

        public void Dispose()

        {
            if (!Disposed)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();

                    Disposed = true;
                }
            }
        }

        public Task<Area> GetAreaAsync(Area entity)
        {
            return DbContext.Areas.FirstOrDefaultAsync(item => item.Id == entity.Id);
        }

        public IQueryable<Area> GetAreas(int pageSize, int pageNumber, string name)
        {
            var query = DbContext.Areas.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            if (!String.IsNullOrEmpty(name))
            {
                query = query.Where(item => item.Name.ToLower().Contains(name.ToLower()));
            }

            return query;
        }

        public async Task<Area> UpdateAreaAsync(Area changes)
        {
            var entity = await GetAreaAsync(changes);

            if (entity != null)
            {
                entity.Name = changes.Name;
                entity.Code = changes.Code;
                entity.IsActive = changes.IsActive;
                entity.IsDelete = changes.IsDelete;
                entity.ModifiedOn = DateTime.Now;
                entity.DisplayOrder = changes.DisplayOrder;

                await DbContext.SaveChangesAsync();
            }

            return entity;
        }
    }
}
