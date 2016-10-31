using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Data.Configuration;
using Shared.Entity;

namespace Data.Context
{
    public partial class kdscwebContext : DbContext
    {
        static kdscwebContext()
        {
            Database.SetInitializer<kdscwebContext>(null);

        }

        public kdscwebContext()
            : base("Name=kdscwebContext")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180;
        }

        public DbSet<Engagement> Engagement { get; set; }
        public DbSet<TAB_KDSC_LOG> TAB_KDSC_LOG { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Properties<string>()
             .Configure(p => p.HasMaxLength(100));


            modelBuilder.Properties<string>()
                .Configure(p => p.HasColumnType("varchar"));

            modelBuilder.Configurations.Add(new EngagementMap());




        }
    }
}
