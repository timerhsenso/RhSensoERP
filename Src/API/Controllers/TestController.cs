using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.API.Controllers;

/// <summary>
/// Controller para testar banco via API
/// REMOVER EM PRODUÇÃO!
/// </summary>
[ApiController]
[Route("api/v1/test")]
public class TestController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Teste completo de todas as tabelas
    /// </summary>
    [HttpGet("banco")]
    public async Task<ActionResult<ApiResponse<object>>> TestarBanco()
    {
        try
        {
            // Testa conectividade
            var conectou = await _context.Database.CanConnectAsync();
            if (!conectou)
            {
                return BadRequest(ApiResponse<object>.Fail("Não conseguiu conectar com o banco"));
            }

            // Conta registros de todas as tabelas
            var resultado = new
            {
                Conectado = true,
                Banco = _context.Database.GetDbConnection().Database,
                Tabelas = new
                {
                    Users = await _context.Users.CountAsync(),
                    Sistemas = await _context.Sistemas.CountAsync(),
                    Funcoes = await _context.Funcoes.CountAsync(),
                    BotoesFuncao = await _context.BotoesFuncao.CountAsync(),
                    UserGroups = await _context.UserGroups.CountAsync(),
                    GruposDeUsuario = await _context.GruposDeUsuario.CountAsync(),
                    GruposFuncoes = await _context.GruposFuncoes.CountAsync()
                },
                Filtros = new
                {
                    UsuariosAtivos = await _context.Users.Ativos().CountAsync(),
                    SistemasAtivos = await _context.Sistemas.Ativos().CountAsync()
                }
            };

            return Ok(ApiResponse<object>.Ok(resultado, "🎉 TODAS AS TABELAS FUNCIONANDO!"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail($"Erro: {ex.Message}"));
        }
    }

    [HttpGet("onde-conectou")]
    public async Task<ActionResult> OndeConectou()
    {
        var info = new
        {
            CanConnect = await _context.Database.CanConnectAsync(),
            Server = _context.Database.GetDbConnection().DataSource,
            Database = _context.Database.GetDbConnection().Database,
            ConnectionString = _context.Database.GetDbConnection().ConnectionString
        };

        return Ok(info);
    }

    /// <summary>
    /// Teste de relacionamentos
    /// </summary>
    [HttpGet("relacionamentos")]
    public async Task<ActionResult<ApiResponse<object>>> TestarRelacionamentos()
    {
        try
        {
            // Testa relacionamento Sistema -> Funcoes
            var sistemaComFuncoes = await _context.Sistemas
                .Include(s => s.Funcoes)
                .FirstOrDefaultAsync();

            // Testa relacionamento Funcao -> Botoes  
            var funcaoComBotoes = await _context.Funcoes
                .Include(f => f.Botoes)
                .FirstOrDefaultAsync();

            var resultado = new
            {
                SistemaComFuncoes = sistemaComFuncoes != null ? new
                {
                    Sistema = sistemaComFuncoes.CdSistema,
                    Descricao = sistemaComFuncoes.DcSistema,
                    QuantidadeFuncoes = sistemaComFuncoes.Funcoes.Count
                } : null,

                FuncaoComBotoes = funcaoComBotoes != null ? new
                {
                    Funcao = funcaoComBotoes.CdFuncao,
                    Sistema = funcaoComBotoes.CdSistema,
                    QuantidadeBotoes = funcaoComBotoes.Botoes.Count
                } : null,

                Status = "Relacionamentos funcionando!"
            };

            return Ok(ApiResponse<object>.Ok(resultado, "🎉 RELACIONAMENTOS FUNCIONANDO!"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail($"Erro: {ex.Message}"));
        }
    }
}

// ==================================================================================
// INSTRUÇÕES PARA USAR
// ==================================================================================

/*
🎯 COMO USAR ESTES TESTES:

1. ATUALIZAR CONNECTION STRING:
   - No arquivo DatabaseTests.cs, linha 21
   - Trocar "localhost" pelo seu servidor
   - Exemplo: "Server=.\\SQLEXPRESS;Database=bd_rhu_copenor;..."

2. EXECUTAR TESTES UNITÁRIOS:
   cd Tests/RhSensoERP.Tests.Unit
   dotnet test --filter "DatabaseTests"
   
   Ou testes específicos:
   dotnet test --filter "Deve_Conectar_Com_Banco"
   dotnet test --filter "Teste_Completo_Todas_Tabelas"

3. TESTAR VIA API:
   - Rodar: dotnet run --project Src/API
   - Abrir: https://localhost:57148/swagger
   - Executar: GET /api/v1/test/banco
   - Executar: GET /api/v1/test/relacionamentos

4. RESULTADO ESPERADO:
   ✅ Todos os testes passam
   ✅ API retorna contagem de todas as tabelas
   ✅ Relacionamentos funcionam

5. SE DER ERRO:
   ❌ Connection string errada
   ❌ Servidor SQL não rodando  
   ❌ Banco bd_rhu_copenor não existe
   ❌ Configuração EF Core errada

6. REMOVER EM PRODUÇÃO:
   - TestController.cs (arquivo inteiro)
   - DatabaseTests.cs (manter apenas se for ambiente de desenvolvimento)

🚀 ESTES TESTES SUBSTITUEM TODOS OS OUTROS!
*/