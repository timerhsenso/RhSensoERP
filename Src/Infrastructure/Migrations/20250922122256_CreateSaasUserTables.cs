using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RhSensoERP.Infrastructure.Migrations
{
    /// <summary>
    /// Migração para criar tabelas SaaS independentes do sistema legacy
    /// </summary>
    public partial class CreateSaasUserTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tabela de Tenants SaaS
            migrationBuilder.CreateTable(
                name: "SaasTenants",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    CompanyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaxUsers = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    PlanType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Basic"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasTenants", x => x.Id);
                });

            // Tabela de Usuários SaaS
            migrationBuilder.CreateTable(
                name: "SaasUsers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmailNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, computedColumnSql: "UPPER([Email])", stored: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),

                    // Controle de acesso
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),

                    // Tokens de segurança
                    EmailConfirmationToken = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),

                    // Multi-tenant
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),

                    // Auditoria e controle
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoginAttempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LockedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),

                    // Metadados
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaasUsers_SaasTenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "dbo",
                        principalTable: "SaasTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Tabela de Convites SaaS
            migrationBuilder.CreateTable(
                name: "SaasInvitations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmailNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, computedColumnSql: "UPPER([Email])", stored: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvitedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),

                    // Token e controle
                    InvitationToken = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),

                    // Metadados
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "User"),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),

                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaasInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaasInvitations_SaasTenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "dbo",
                        principalTable: "SaasTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaasInvitations_SaasUsers_InvitedById",
                        column: x => x.InvitedById,
                        principalSchema: "dbo",
                        principalTable: "SaasUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Índices para performance e unicidade
            migrationBuilder.CreateIndex(
                name: "UK_SaasTenants_Domain",
                schema: "dbo",
                table: "SaasTenants",
                column: "Domain",
                unique: true,
                filter: "[Domain] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SaasTenants_IsActive",
                schema: "dbo",
                table: "SaasTenants",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UK_SaasUsers_EmailNormalized",
                schema: "dbo",
                table: "SaasUsers",
                column: "EmailNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaasUsers_TenantId",
                schema: "dbo",
                table: "SaasUsers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SaasUsers_EmailConfirmationToken",
                schema: "dbo",
                table: "SaasUsers",
                column: "EmailConfirmationToken",
                filter: "[EmailConfirmationToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SaasUsers_PasswordResetToken",
                schema: "dbo",
                table: "SaasUsers",
                column: "PasswordResetToken",
                filter: "[PasswordResetToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SaasUsers_IsActive_EmailConfirmed",
                schema: "dbo",
                table: "SaasUsers",
                columns: new[] { "IsActive", "EmailConfirmed" });

            migrationBuilder.CreateIndex(
                name: "UK_SaasInvitations_EmailNormalized_TenantId",
                schema: "dbo",
                table: "SaasInvitations",
                columns: new[] { "EmailNormalized", "TenantId" },
                unique: true,
                filter: "[IsAccepted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SaasInvitations_InvitationToken",
                schema: "dbo",
                table: "SaasInvitations",
                column: "InvitationToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaasInvitations_ExpiresAt",
                schema: "dbo",
                table: "SaasInvitations",
                column: "ExpiresAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaasInvitations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SaasUsers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SaasTenants",
                schema: "dbo");
        }
    }
}