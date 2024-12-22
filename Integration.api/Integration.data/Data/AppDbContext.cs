using Integration.data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
namespace Integration.data.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<DataBase>   dataBases { get; set; }
        public DbSet<Models.Module> modules { get; set; }
        public DbSet<ColumnFrom> columnFroms { get; set; }
        public DbSet<TableReference> References { get; set; }
        public DbSet<Operation>  Operations { get; set; }
        public DbSet<StaticStandard>  staticStandards{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Models.Module>()
                .HasOne(m => m.ToDb)
                .WithMany()
                .HasForeignKey(m => m.ToDbId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Module>()
                .HasOne(m => m.FromDb)
                .WithMany()
                .HasForeignKey(m => m.FromDbId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<TableReference>()
                .HasOne(tr => tr.Module)
                .WithMany()
                .HasForeignKey(tr => tr.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);

        }


    }
}

