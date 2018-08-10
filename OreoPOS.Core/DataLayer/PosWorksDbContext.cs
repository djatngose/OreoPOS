using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OreoPOS.Core.EntityLayer;

namespace OreoPOS.Core.DataLayer
{
    [DbConfigurationType(typeof(CodeConfig))]
    public class PosWorksDbContext : DbContext
    {
        public DbSet<Area> Areas { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public PosWorksDbContext(string connectionString) : base(connectionString)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<DbContext>(null);
            //base.OnModelCreating(modelBuilder);
        }

    }
    public class CodeConfig : DbConfiguration
    {
        public CodeConfig()
        {
            SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
        }
    }
}

