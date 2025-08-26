using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shorten.url.infrastructure.Persistence.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            
            migrationBuilder.CreateTable(
                name: "ApiClients",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UniqueId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ShortUrl = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RedirectUrl = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Hits = table.Column<int>(type: "int", nullable: false),
                    ApiClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_ApiClients_ApiClientId",
                        column: x => x.ApiClientId,
                        principalSchema: "dbo",
                        principalTable: "ApiClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"CREATE TABLE [dbo].[DistributedCache](
	                [Id] [nvarchar](449) NOT NULL,
	                [Value] [varbinary](max) NOT NULL,
	                [ExpiresAtTime] [datetimeoffset](7) NOT NULL,
	                [SlidingExpirationInSeconds] [bigint] NULL,
	                [AbsoluteExpiration] [datetimeoffset](7) NULL,
                 CONSTRAINT [PK_DistributedCache] PRIMARY KEY CLUSTERED 
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                GO");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ApiClientId",
                schema: "dbo",
                table: "Addresses",
                column: "ApiClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ShortUrl",
                schema: "dbo",
                table: "Addresses",
                column: "ShortUrl",
                unique: true,
                filter: "[ShortUrl] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UniqueId",
                schema: "dbo",
                table: "Addresses",
                column: "UniqueId",
                unique: true,
                filter: "[UniqueId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApiClients_ApiKey",
                schema: "dbo",
                table: "ApiClients",
                column: "ApiKey",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ApiClients",
                schema: "dbo");
        }
    }
}
