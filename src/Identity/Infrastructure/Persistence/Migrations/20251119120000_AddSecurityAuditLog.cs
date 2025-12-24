// src/Identity/Infrastructure/Persistence/Migrations/YYYYMMDDHHMMSS_AddSecurityAuditLog.cs

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RhSensoERP.Identity.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Migration para adicionar tabela SecurityAuditLogs.
    /// ✅ FASE 5: Auditoria completa de eventos de segurança
    /// 
    /// INSTRUÇÕES:
    /// 1. Copie este arquivo para a pasta Migrations do projeto Identity
    /// 2. Renomeie para: YYYYMMDDHHMMSS_AddSecurityAuditLog.cs
    ///    (substitua YYYYMMDDHHMMSS pela data/hora atual, ex: 20251119120000_AddSecurityAuditLog.cs)
    /// 3. Execute: dotnet ef database update
    /// </summary>
    public partial class AddSecurityAuditLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SEG_SecurityAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUserSecurity = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AdditionalData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SEG_SecurityAuditLogs", x => x.Id);
                    table.ForeignKey(
name: "FK_SEG_SecurityAuditLogs_UserSecurity_IdUserSecurity",
                        column: x => x.IdUserSecurity,
                        principalTable: "UserSecurity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SEG_SecurityAuditLogs_OccurredAt",
                table: "SEG_SecurityAuditLogs",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_SEG_SecurityAuditLogs_EventType",
                table: "SEG_SecurityAuditLogs",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_SEG_SecurityAuditLogs_Severity",
                table: "SEG_SecurityAuditLogs",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_SEG_SecurityAuditLogs_IdUserSecurity",
                table: "SEG_SecurityAuditLogs",
                column: "IdUserSecurity");

            migrationBuilder.CreateIndex(
                name: "IX_SEG_SecurityAuditLogs_IpAddress",
                table: "SEG_SecurityAuditLogs",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_SEG_SecurityAuditLogs_OccurredAt_EventType_Severity",
                table: "SEG_SecurityAuditLogs",
                columns: new[] { "OccurredAt", "EventType", "Severity" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SEG_SecurityAuditLogs");
        }
    }
}
