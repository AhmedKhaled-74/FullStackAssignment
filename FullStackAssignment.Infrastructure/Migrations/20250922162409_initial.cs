using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FullStackAssignment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "ProductCodeSeq",
                startValue: 4L);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false, defaultValue: 0m),
                    MinimumQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductCode);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { new Guid("379ba4ed-a814-4058-a0a0-a7436ceeb3b3"), "Electronics" },
                    { new Guid("d6701001-5662-41b1-8d13-13cc55f78a30"), "Fashion" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserName", "EmailAddress", "HashedPassword", "LastLoginDate", "RefreshToken", "RefreshTokenExpiration" },
                values: new object[] { "admin", "admin@example.com", "tAYkefNRbgerfqenlCh73g==.7f8xyKILm9tCL0faOUwbLF0KR9y/JdnkPVbAdbE56tY=", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductCode", "CategoryId", "DiscountRate", "Image", "MinimumQuantity", "Name", "Price" },
                values: new object[,]
                {
                    { "P00001", new Guid("379ba4ed-a814-4058-a0a0-a7436ceeb3b3"), 0.05m, "/images/products/iphone.jpeg", 1, "Smartphone", 599.99m },
                    { "P00002", new Guid("379ba4ed-a814-4058-a0a0-a7436ceeb3b3"), 0.10m, "/images/products/laptop.jpg", 1, "Laptop", 1999.99m },
                    { "P00003", new Guid("d6701001-5662-41b1-8d13-13cc55f78a30"), 0.10m, "/images/products/tshirt.jpg", 2, "T-Shirt", 19.99m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            // Create the stored procedure
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE sp_GetNextProductCode
                AS
                BEGIN
                    SELECT NEXT VALUE FOR ProductCodeSeq AS NextValue;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropSequence(
                name: "ProductCodeSeq");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetNextProductCode");
        }
    }
}
