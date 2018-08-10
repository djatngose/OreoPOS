using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OreoPOS.Core.EntityLayer;

namespace OreoPOS.Core.DataLayer
{
    public interface IAreaRepository : IDisposable
    {
        IQueryable<Area> GetAreas(Int32 pageSize, Int32 pageNumber, String name);

        Task<Area> GetAreaAsync(Area entity);

        Task<Area> AddAreaAsync(Area entity);

        Task<Area> UpdateAreaAsync(Area changes);

        Task<Area> DeleteAreaAsync(Area changes);
    }
}
