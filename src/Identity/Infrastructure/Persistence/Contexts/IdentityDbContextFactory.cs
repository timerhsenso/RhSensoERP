using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Contexts;

/// <summary>
/// Factory para criação do <see cref="IdentityDbContext"/> em tempo de design (CLI do EF Core).
/// 
/// Permite executar comandos do EF Core a partir do projeto Infrastructure, como:
/// - dotnet ef migrations add NomeDaMigration
/// - dotnet ef database update
/// - dotnet ef migrations script
/// 
/// Esta factory procura por arquivos appsettings.json em múltiplos locais para
/// garantir compatibilidade com diferentes estruturas de pastas do projeto.
/// </summary>
public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    /// <summary>
    /// Cria uma instância do <see cref="IdentityDbContext"/> para uso em tempo de design.
    /// </summary>
    /// <param name="args">Argumentos da linha de comando.</param>
    /// <returns>Instância configurada do DbContext.</returns>
    public IdentityDbContext CreateDbContext(string[] args)
    {
        // Diretório base para busca de configurações
        var basePath = Directory.GetCurrentDirectory();

        // =====================================================================
        // Configuração multi-camada para suportar diferentes estruturas
        // =====================================================================
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            // Tenta encontrar appsettings no projeto Infrastructure
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            // Tenta encontrar appsettings no projeto API (2 níveis acima)
            .AddJsonFile(Path.Combine("..", "..", "API", "appsettings.json"), optional: true)
            .AddJsonFile(Path.Combine("..", "..", "API", "appsettings.Development.json"), optional: true)
            // Variáveis de ambiente têm precedência
            .AddEnvironmentVariables()
            .Build();

        // =====================================================================
        // Connection String com fallback
        // =====================================================================
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=.;Database=bd_rhu_copenor;Trusted_Connection=True;TrustServerCertificate=true;";

        // =====================================================================
        // Configuração do DbContext
        // =====================================================================
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseSqlServer(
            connectionString,
            sqlOptions =>
            {
                // Configurações adicionais de resiliência
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null
                );

                // Timeout para migrations longas
                sqlOptions.CommandTimeout(300); // 5 minutos
            }
        );

        // Log da connection string (sanitizada) em desenvolvimento
#if DEBUG
        var sanitizedCs = SanitizeConnectionString(connectionString);
        Console.WriteLine($"[IdentityDbContextFactory] 🔧 Connection String: {sanitizedCs}");
#endif

        return new IdentityDbContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Remove informações sensíveis da connection string para logging.
    /// </summary>
    private static string SanitizeConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return "[VAZIA]";

        // Remove passwords e user ids
        var sanitized = connectionString;

        if (sanitized.Contains("Password=", StringComparison.OrdinalIgnoreCase))
        {
            var parts = sanitized.Split(';');
            sanitized = string.Join(";", parts.Select(p =>
                p.TrimStart().StartsWith("Password=", StringComparison.OrdinalIgnoreCase)
                    ? "Password=***"
                    : p
            ));
        }

        if (sanitized.Contains("User Id=", StringComparison.OrdinalIgnoreCase))
        {
            var parts = sanitized.Split(';');
            sanitized = string.Join(";", parts.Select(p =>
                p.TrimStart().StartsWith("User Id=", StringComparison.OrdinalIgnoreCase)
                    ? "User Id=***"
                    : p
            ));
        }

        return sanitized;
    }
}