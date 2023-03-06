using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StackManager.Context.Domain;

namespace StackManager.Context
{
    class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //DbSet<TEntity>属性让上下文知道要在模型中包含哪些类型
        public DbSet<AlarmCategory> AlarmCategories { get; set; }
        public DbSet<DeviceCategory> DeviceCategories { get; set; }
        public DbSet<SlaveDevice> SlaveDevices { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Box> Boxes { get; set; }
        public DbSet<Pallet> Pallets { get; set; }
        public DbSet<Flowline> Flowlines { get; set; }

        public DbSet<DeviceStats> DeviceStats { get; set; }
        public DbSet<DeviceAlarm> DeviceAlarms { get; set; }

        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var softDeleted = entityType.FindProperty("SoftDeleted");
                if (softDeleted != null && softDeleted.ClrType == typeof(bool))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "x");

                    var filter = Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, softDeleted.PropertyInfo),
                            Expression.Constant(false, typeof(bool))),
                        parameter);

                    entityType.SetQueryFilter(filter);
                }
            }

            modelBuilder.Entity<Flowline>()
                .HasMany(x => x.Boxes)
                .WithOne(x => x.Flowline);
        }
    }
}
