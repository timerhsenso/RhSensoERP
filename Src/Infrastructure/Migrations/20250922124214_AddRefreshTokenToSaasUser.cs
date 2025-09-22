using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RhSensoERP.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddRefreshTokenToSaasUser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "RefreshToken",
            schema: "dbo",
            table: "SaasUsers",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "RefreshTokenExpiresAt",
            schema: "dbo",
            table: "SaasUsers",
            type: "datetime2",
            nullable: true);

        // Criar índice único para performance e segurança
        migrationBuilder.CreateIndex(
            name: "UK_SaasUsers_RefreshToken",
            schema: "dbo",
            table: "SaasUsers",
            column: "RefreshToken",
            unique: true,
            filter: "[RefreshToken] IS NOT NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "UK_SaasUsers_RefreshToken",
            schema: "dbo",
            table: "SaasUsers");

        migrationBuilder.DropColumn(
            name: "RefreshToken",
            schema: "dbo",
            table: "SaasUsers");

        migrationBuilder.DropColumn(
            name: "RefreshTokenExpiresAt",
            schema: "dbo",
            table: "SaasUsers");
    }
}