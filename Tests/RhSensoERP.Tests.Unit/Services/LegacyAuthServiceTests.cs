using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RhSensoERP.Application.Auth;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Infrastructure.Auth;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Infrastructure.Persistence.Interceptors;
using RhSensoERP.Infrastructure.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RhSensoERP.Tests.Unit.Services;

/// <summary>
/// Testes da classe <c>LegacyAuthServiceTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Services/LegacyAuthServiceTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class LegacyAuthServiceTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var http = new HttpContextAccessor();
        var audit = new AuditSaveChangesInterceptor(http);
        return new AppDbContext(options, audit);
    }

    private static IConfiguration CreateConfig() =>
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "test-issuer",
            ["Jwt:Audience"] = "test-audience",
            ["Jwt:Key"] = "dev-only-test-key-1234567890"
        }!).Build();

    private static JwtTokenService CreateJwtService(IConfiguration cfg)
    {
        var optsObj = new JwtOptions();
        var t = typeof(JwtOptions);
        void SetIfExists(string name, string? value)
        {
            var p = t.GetProperty(name);
            if (p != null && p.CanWrite && value is not null)
                p.SetValue(optsObj, value);
        }
        SetIfExists("Issuer", cfg["Jwt:Issuer"]);
        SetIfExists("Audience", cfg["Jwt:Audience"]);
        var key = cfg["Jwt:Key"];
        SetIfExists("Key", key);
        SetIfExists("Secret", key);
        SetIfExists("SecretKey", key);
        SetIfExists("SigningKey", key);

        return new JwtTokenService(Options.Create(optsObj));
    }

    [Fact]
/// <summary>
/// Authenticateasync Deve falha para inativo usuário e senha incorreta.
/// </summary>
    public async Task AuthenticateAsync_Should_Fail_For_Inactive_User_And_WrongPassword()
    {
        // Arrange
        var db = CreateContext(nameof(AuthenticateAsync_Should_Fail_For_Inactive_User_And_WrongPassword));
        db.Users.Add(new User
        {
            CdUsuario = "john",
            DcUsuario = "John",
            SenhaUser = "right",
            FlAtivo = 'N' // inativo
        });
        await db.SaveChangesAsync();

        var cfg = CreateConfig();
        var jwt = CreateJwtService(cfg);
        var svc = new LegacyAuthService(db, jwt, cfg);

        // Act
        var rInactive = await svc.AuthenticateAsync("john", "right");
        var rWrongPwd = await svc.AuthenticateAsync("john", "wrong");

        // Assert
        rInactive.Success.Should().BeFalse("usuário inativo não deve autenticar");
        rInactive.AccessToken.Should().BeNull();

        rWrongPwd.Success.Should().BeFalse("senha errada não deve autenticar");
        rWrongPwd.AccessToken.Should().BeNull();
    }
}