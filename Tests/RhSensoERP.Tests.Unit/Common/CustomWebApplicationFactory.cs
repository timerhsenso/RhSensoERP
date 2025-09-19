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

// WebApplicationFactory configurada para testes de API
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover AppDbContext real (SQL Server) se existir
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            // Criar e manter uma conexão SQLite em memória aberta durante os testes
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(_connection));

            // Substituir ILegacyAuthService por um mock simples baseado em DI
            services.RemoveAll<ILegacyAuthService>();
            services.AddSingleton<ILegacyAuthService>(sp => new FakeLegacyAuthService());

            // Garantir que o banco foi criado
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }

    // Serviço fake básico para testes de API
    private sealed class FakeLegacyAuthService : ILegacyAuthService
    {
        public Task<AuthResult> AuthenticateAsync(string cdusuario, string senha, CancellationToken ct = default)
        {
            if (cdusuario == "admin" && senha == "123456")
            {
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
            });
        }

        public bool CheckHabilitacao(string cdsistema, string cdfuncao, UserPermissions permissions) => true;
        public bool CheckBotao(string cdsistema, string cdfuncao, char acao, UserPermissions permissions) => true;
        public char CheckRestricao(string cdsistema, string cdfuncao, UserPermissions permissions) => 'N';
    }
}