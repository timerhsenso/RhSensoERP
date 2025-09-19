// Tests/RhSensoERP.Tests.Unit/Infrastructure/Repositories/EfRepositoryTests.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Infrastructure.Persistence.Interceptors;
using RhSensoERP.Infrastructure.Repositories;
using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Tests.Unit.Infrastructure;

/// <summary>
/// Testes da classe <c>EfRepositoryTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Infrastructure/Repositories/EfRepositoryTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class EfRepositoryTests
{
    private class TestEntity : BaseEntity, ISoftDeletable
    {
        public string Name { get; set; } = "";
        public bool IsDeleted { get; set; }
    }

    private class TestDbContext : AppDbContext
    {
        public TestDbContext(DbContextOptions<AppDbContext> options, AuditSaveChangesInterceptor a)
            : base(options, a) { }

        public DbSet<TestEntity> Tests => Set<TestEntity>();
    }

    private static TestDbContext CreateContext(string name)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(name) // REQUER Microsoft.EntityFrameworkCore.InMemory
            .Options;
        var audit = new AuditSaveChangesInterceptor(new HttpContextAccessor());
        return new TestDbContext(options, audit);
    }

    [Fact]
/// <summary>
/// Adicionar buscar por id atualizar excluir Deve work.
/// </summary>
    public async Task Add_GetById_Update_Delete_Should_Work()
    {
        // Arrange
        var db = CreateContext(nameof(Add_GetById_Update_Delete_Should_Work));
        var repo = new EfRepository<TestEntity>(db);
        var e = new TestEntity { Name = "A" };

        // Act - Add
        await repo.AddAsync(e);
        await db.SaveChangesAsync();

        // Assert - GetById
        var found = await repo.GetByIdAsync(e.Id);
        found.Should().NotBeNull();
        found!.Name.Should().Be("A");

        // Act - Update
        e.Name = "B";
        await repo.UpdateAsync(e);
        await db.SaveChangesAsync();

        // Assert - Updated
        var updated = await repo.GetByIdAsync(e.Id);
        updated!.Name.Should().Be("B");

        // Act - Delete (soft)
        await repo.DeleteAsync(e);
        await db.SaveChangesAsync();

        // Assert - Soft delete flag
        e.IsDeleted.Should().BeTrue();
    }
}