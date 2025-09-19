// Tests/RhSensoERP.Tests.Unit/Infrastructure/Configurations/UserConfigTests.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Http;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Infrastructure.Persistence.Interceptors;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Tests.Unit.Infrastructure;

/// <summary>
/// Testes da classe <c>UserConfigTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Infrastructure/Configurations/UserConfigTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class UserConfigTests
{
    private static AppDbContext CreateContext()
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opt, new AuditSaveChangesInterceptor(new HttpContextAccessor()));
    }

    [Fact]
/// <summary>
/// Deve map usuário to tuse1 with cdusuario como key e unique id index.
/// </summary>
    public void Should_Map_User_To_tuse1_With_CdUsuario_As_Key_And_Unique_Id_Index()
    {
        using var ctx = CreateContext();

        var entity = ctx.Model.FindEntityType(typeof(User));
        entity.Should().NotBeNull();

        // Tabela
        entity!.GetTableName().Should().Be("tuse1");
        entity.GetSchema().Should().Be("dbo");

        // Chave
        entity.FindPrimaryKey()!.Properties.Select(p => p.Name).Should().ContainSingle()
            .Which.Should().Be(nameof(User.CdUsuario));

        // Índice único em Id
        var hasUniqueIdIndex = entity.GetIndexes().Any(i =>
            i.IsUnique &&
            i.Properties.Any(p => p.Name == nameof(User.Id)));
        hasUniqueIdIndex.Should().BeTrue();

        // Propriedades ignoradas principais (ver UserConfig)
        var ignored = new[]
        {
            nameof(User.CreatedAt), nameof(User.CreatedBy),
            nameof(User.UpdatedAt), nameof(User.UpdatedBy),
            nameof(User.Username), nameof(User.DisplayName),
            nameof(User.Email), nameof(User.Active), nameof(User.PasswordHash)
        };

        foreach (var prop in ignored)
        {
            entity.FindProperty(prop).Should().BeNull($"'{prop}' é ignorada no mapeamento");
        }
    }
}