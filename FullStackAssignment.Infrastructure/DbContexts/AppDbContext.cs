using FullStackAssignment.Application.Mappers;
using FullStackAssignment.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FullStackAssignment.Infrastructure.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasSequence<int>("ProductCodeSeq")
                        .StartsAt(4)
                        .IncrementsBy(1);

            #region Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductCode);

                entity.Property(p => p.ProductCode)
                      .IsRequired()
                      .HasMaxLength(20)
                      .ValueGeneratedNever();

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Image)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(p => p.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(p => p.DiscountRate)
                      .HasColumnType("decimal(5,4)")
                      .HasDefaultValue(0m);

                entity.Property(p => p.MinimumQuantity)
                      .HasDefaultValue(1);

                // Foreign key relationship
                entity.HasOne(p => p.Category)
                      .WithMany()
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.CategoryName)
                      .IsRequired()
                      .HasMaxLength(100);
            });
            #endregion

            #region User
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.UserName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(u => u.HashedPassword)
                    .IsRequired()
                    .HasMaxLength(300);
            });
            #endregion

            // ======================
            // Seed Data
            // ======================
            #region SeedData
            //categories
            var electronicsId = Guid.Parse("379BA4ED-A814-4058-A0A0-A7436CEEB3B3");
            var fashionId = Guid.Parse("D6701001-5662-41B1-8D13-13CC55F78A30");
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = electronicsId, CategoryName = "Electronics" },
                new Category { CategoryId = fashionId, CategoryName = "Fashion" }
            );

            //products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductCode = "P00001",
                    Name = "Smartphone",
                    CategoryId = electronicsId,
                    Image = "/images/products/iphone.jpeg",
                    Price = 599.99m,
                    DiscountRate = 0.05m,
                    MinimumQuantity = 1
                },
                new Product
                {
                    ProductCode = "P00002",
                    Name = "Laptop",
                    CategoryId = electronicsId,
                    Image = "/images/products/laptop.jpg",
                    Price = 1999.99m,
                    DiscountRate = 0.10m,
                    MinimumQuantity = 1
                }, new Product
                {
                    ProductCode = "P00003",
                    Name = "T-Shirt",
                    CategoryId = fashionId,
                    Image = "/images/products/tshirt.jpg",
                    Price = 19.99m,
                    DiscountRate = 0.10m,
                    MinimumQuantity = 2
                }
            );

            //userAdmin
            modelBuilder.Entity<User>().HasData(
               new User
               {
                   UserName = "admin",
                   EmailAddress = "admin@example.com",
                   HashedPassword = "tAYkefNRbgerfqenlCh73g==.7f8xyKILm9tCL0faOUwbLF0KR9y/JdnkPVbAdbE56tY=", 
                   //pass is @Aa12345678;
                   LastLoginDate = new DateTime(2025, 01, 01)
               }
           );
            #endregion

        }
        // Method to execute stored procedure
        public async Task<int> GetNextProductCodeAsync()
        {
            var connection = Database.GetDbConnection();
            var wasConnectionOpen = connection.State == ConnectionState.Open;

            if (!wasConnectionOpen)
                await connection.OpenAsync();

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = "EXEC sp_GetNextProductCode";
                command.CommandType = CommandType.Text;

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            finally
            {
                // Only close if we opened it
                if (!wasConnectionOpen && connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }
    }
}
