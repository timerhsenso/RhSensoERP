using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Contexts;

/// <summary>
/// Factory para criação do DbContext em tempo de design (CLI do EF Core).
/// Permite rodar 'dotnet ef migrations add' a partir do projeto Infrastructure.
/// </summary>
public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile(Path.Combine("..", "..", "API", "appsettings.json"), optional: true)
            .AddJsonFile(Path.Combine("..", "..", "API", "appsettings.Development.json"), optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=.;Database=bd_rhu_copenor;Trusted_Connection=True;TrustServerCertificate=true;";

        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new IdentityDbContext(optionsBuilder.Options);
    }
}