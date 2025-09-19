using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Infrastructure.Persistence.Interceptors;
using Xunit;

namespace RhSensoERP.Tests.Unit.Infrastructure;

/// <summary>
/// Testes da classe <c>AuditSaveChangesInterceptorTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Infrastructure/AuditSaveChangesInterceptorTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class AuditSaveChangesInterceptorTests
{
    [Fact]
/// <summary>
/// Deve set createdandupdated fields on savechanges.
/// </summary>
    public async Task Should_Set_CreatedAndUpdated_Fields_On_SaveChanges()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddScoped<AuditSaveChangesInterceptor>();
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Filename=:memory:"));

        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();

        var user = new User { CdUsuario = "u1", DcUsuario = "Teste", TenantId = Guid.NewGuid() };
        db.Add(user);
        await db.SaveChangesAsync();

        user.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        user.CreatedBy.Should().NotBeNullOrEmpty();

        user.DcUsuario = "Alterado";
        await db.SaveChangesAsync();

        user.UpdatedAt.Should().NotBeNull();
    }
}