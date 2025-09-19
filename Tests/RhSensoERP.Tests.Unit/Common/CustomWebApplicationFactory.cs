using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Data.Sqlite;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Application.Security.Auth.Services;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.API;

namespace RhSensoERP.Tests.Unit.Common;

/// <summary>
/// Fábrica para hospedar a API em memória durante os testes de integração.
/// - Usa SQLite In-Memory com conexão aberta (evita dispose prematuro).
/// - Substitui ILegacyAuthService por uma implementação fake previsível.
/// - Garante EnsureCreated() no AppDbContext (sem migrations nos testes).
/// </summary>
/// <summary>
/// Testes da classe <c>CustomWebApplicationFactory</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Common/CustomWebApplicationFactory.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    /// <summary>
    /// Configura o host de testes: EF Core com SQLite In-Memory e substituições de serviços.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o AppDbContext real (SQL Server) se registrado
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            // Conexão SQLite in-memory mantida aberta durante toda a execução dos testes
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Registra o DbContext com a conexão em memória
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(_connection));

            // Substitui autenticação por fake service para cenários controlados
            services.RemoveAll<ILegacyAuthService>();
            services.AddSingleton<ILegacyAuthService>(sp => new FakeLegacyAuthService());

            // Garante que o schema foi criado antes de rodar os testes
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    /// <summary>
    /// Fecha e descarta a conexão em memória ao finalizar os testes.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }

    /// <summary>
    /// Implementação fake de ILegacyAuthService para testes.
    /// Retorna sucesso quando (user, senha) = (admin, 123456); caso contrário, falha.
    /// </summary>
    private sealed class FakeLegacyAuthService : ILegacyAuthService
    {
        public Task<AuthResult> AuthenticateAsync(string cdusuario, string senha, CancellationToken ct = default)
        {
            if (cdusuario == "admin" && senha == "123456")
            {
                // Constrói um usuário mínimo válido para os cenários de teste
                var user = new UserSessionData(
                    cdusuario,           // CdUsuario
                    "Administrador",     // DcUsuario
                    null,                // NmImpCche
                    'A',                 // TpUsuario
                    null,                // NoMatric
                    null,                // CdEmpresa
                    null,                // CdFilial
                    1,                   // NoUser
                    "admin@local",       // EmailUsuario
                    'S',                 // FlAtivo
                    Guid.NewGuid(),      // Id
                    "ADMIN",             // NormalizedUsername
                    null,                // IdFuncionario
                    null                 // FlNaoRecebeEmail
                );

                return Task.FromResult(new AuthResult(true, null, user, "fake-token"));
            }
            return Task.FromResult(new AuthResult(false, "INVALID_CREDENTIALS", null, null));
        }

        public Task<UserPermissions> GetUserPermissionsAsync(string cdusuario, CancellationToken ct = default)
        {
            var user = new UserSessionData(
                cdusuario, "Teste", null, 'A', null, null, null, 2, "t@local", 'S',
                Guid.NewGuid(), "TESTE", null, null
            );

            return Task.FromResult(new UserPermissions
            {
                UserData = user,
                // Groups e Permissions podem ser populados se o teste exigir
            });
        }

        public bool CheckHabilitacao(string cdsistema, string cdfuncao, UserPermissions permissions) => true;
        public bool CheckBotao(string cdsistema, string cdfuncao, char acao, UserPermissions permissions) => true;
        public char CheckRestricao(string cdsistema, string cdfuncao, UserPermissions permissions) => 'N';
    }
}