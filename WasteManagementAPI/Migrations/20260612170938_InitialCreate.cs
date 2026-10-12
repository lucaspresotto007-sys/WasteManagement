using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WasteManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CollectionRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionRoutes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationalTips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MaterialCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationalTips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RecyclingPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WasteBins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaterialType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaxCapacityKg = table.Column<double>(type: "float", nullable: false),
                    CurrentFillLevelKg = table.Column<double>(type: "float", nullable: false),
                    IsFullAlertTriggered = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WasteBins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DisposalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WasteBinId = table.Column<int>(type: "int", nullable: false),
                    WeightKg = table.Column<double>(type: "float", nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisposalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisposalRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisposalRecords_WasteBins_WasteBinId",
                        column: x => x.WasteBinId,
                        principalTable: "WasteBins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EducationalTips",
                columns: new[] { "Id", "Content", "MaterialCategory", "Title" },
                values: new object[,]
                {
                    { 1, "Lave as embalagens plásticas para retirar os resíduos de alimentos antes de descartá-las no coletor vermelho. Isso evita mau cheiro e atração de vetores, facilitando a reciclagem.", "Plastic", "Como descartar plástico corretamente" },
                    { 2, "Papéis sujos, engordurados (como caixas de pizza) ou molhados não devem ser colocados na lixeira azul de recicláveis, pois contaminam o lote. Descarte-os no lixo comum/orgânico.", "Paper", "Papel molhado não é reciclável!" },
                    { 3, "Sempre embale vidros quebrados em caixas de papelão ou garrafas pet cortadas e identifique o conteúdo para evitar acidentes com os profissionais da coleta seletiva.", "Glass", "Atenção com vidros quebrados" }
                });

            migrationBuilder.InsertData(
                table: "WasteBins",
                columns: new[] { "Id", "CurrentFillLevelKg", "IsFullAlertTriggered", "Location", "MaterialType", "MaxCapacityKg" },
                values: new object[,]
                {
                    { 1, 15.0, false, "Setor Norte - Quadra 10", "Plastic", 100.0 },
                    { 2, 73.0, true, "Setor Norte - Quadra 10", "Paper", 80.0 },
                    { 3, 30.0, false, "Setor Sul - Centro Comercial", "Glass", 150.0 },
                    { 4, 10.0, false, "Avenida Principal - Km 2", "Metal", 120.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisposalRecords_UserId",
                table: "DisposalRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DisposalRecords_WasteBinId",
                table: "DisposalRecords",
                column: "WasteBinId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionRoutes");

            migrationBuilder.DropTable(
                name: "DisposalRecords");

            migrationBuilder.DropTable(
                name: "EducationalTips");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WasteBins");
        }
    }
}
