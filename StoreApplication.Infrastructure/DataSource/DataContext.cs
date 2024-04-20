using StoreApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace StoreApplication.Infrastructure.DataSource
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.OrderDate).IsRequired();
                entity.Property(o => o.Total).IsRequired();

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(o => o.UserId)
                      .IsRequired();
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => od.Id);

                entity.Property(od => od.Quantity).IsRequired();
                entity.Property(od => od.UnitPrice).IsRequired();

                entity.HasOne<Order>()
                      .WithMany()
                      .HasForeignKey(od => od.OrderId)
                      .IsRequired();

                entity.HasOne<Product>()
                      .WithMany()
                      .HasForeignKey(od => od.ProductId)
                      .IsRequired();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Description).IsRequired();
                entity.Property(p => p.Price).IsRequired();
                entity.Property(p => p.Stock).IsRequired();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Description).HasMaxLength(500);
            });

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(DomainEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).Property<DateTime>("CreatedOn").IsRequired();
                    modelBuilder.Entity(entityType.ClrType).Property<DateTime>("LastModifiedOn").IsRequired();
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
