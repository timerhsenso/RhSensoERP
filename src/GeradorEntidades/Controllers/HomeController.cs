// =============================================================================
// GERADOR FULL-STACK v3.0 - HOME CONTROLLER
// Controller principal com suporte a geração Full-Stack
// =============================================================================

using GeradorEntidades.Models;
using GeradorEntidades.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace GeradorEntidades.Controllers;

public class HomeController : Controller
{
    private readonly DatabaseService _dbService;
    private readonly ManifestService _manifestService;
    private readonly CodeGeneratorService _codeGenerator;
    private readonly FullStackGeneratorService _fullStackGenerator;
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(
        DatabaseService dbService,
        ManifestService manifestService,
        CodeGeneratorService codeGenerator,
        FullStackGeneratorService fullStackGenerator,
        ILogger<HomeController> logger,
        IConfiguration configuration)
    {
        _dbService = dbService;
        _manifestService = manifestService;
        _codeGenerator = codeGenerator;
        _fullStackGenerator = fullStackGenerator;
        _logger = logger;
        _configuration = configuration;
    }

    // =========================================================================
    // PÁGINA PRINCIPAL
    // =========================================================================

    /// <summary>
    /// Página inicial com lista de módulos (Novo Wizard).
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var modules = await _manifestService.GetModulesAsync();
            ViewBag.ApiUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7193";
            return View(modules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar módulos");
            return View(new List<ModuleInfo>());
        }
    }

    // =========================================================================
    // APIs DE CONSULTA (AJAX)
    // =========================================================================

    /// <summary>
    /// Busca tabelas (AJAX).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> SearchTabelas(string termo)
    {
        var tabelas = await _dbService.SearchTabelasAsync(termo);

        return Json(tabelas.Select(t => new
        {
            t.NomeTabela,
            t.NomePascalCase,
            t.Descricao,
            ColunasCount = t.Colunas.Count,
            PrimaryKey = t.PrimaryKey?.NomePascalCase,
            HasPrimaryKey = t.HasPrimaryKey,
            HasCompositePrimaryKey = t.HasCompositePrimaryKey,
            HasForeignKeys = t.ForeignKeys.Any(),
            ForeignKeysCount = t.ForeignKeys.Count,
            HasCompositeForeignKeys = t.HasCompositeForeignKeys
        }));
    }

    /// <summary>
    /// Obtém detalhes de uma tabela (AJAX).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTabela(string nome)
    {
        _logger.LogInformation("GetTabela chamado para: {Nome}", nome);

        var tabela = await _dbService.GetTabelaAsync(nome);

        if (tabela == null)
        {
            _logger.LogWarning("Tabela não encontrada: {Nome}", nome);
            return NotFound($"Tabela '{nome}' não encontrada.");
        }

        _logger.LogInformation("Tabela {Nome} encontrada: {Colunas} colunas, {FKs} FKs, {Indices} índices",
            nome, tabela.Colunas.Count, tabela.ForeignKeys.Count, tabela.Indices.Count);

        return Json(new
        {
            tabela.NomeTabela,
            tabela.NomePascalCase,
            tabela.NomePlural,
            tabela.Descricao,
            HasPrimaryKey = tabela.HasPrimaryKey,
            HasCompositePrimaryKey = tabela.HasCompositePrimaryKey,
            HasCompositeForeignKeys = tabela.HasCompositeForeignKeys,
            PrimaryKey = tabela.PrimaryKey != null ? new
            {
                tabela.PrimaryKey.Nome,
                tabela.PrimaryKey.NomePascalCase,
                tabela.PrimaryKey.TipoCSharp
            } : null,
            PrimaryKeyColumns = tabela.PrimaryKeyColumns.Select(c => new
            {
                c.Nome,
                c.NomePascalCase,
                c.TipoCSharp
            }),
            Colunas = tabela.Colunas.Select(c => new
            {
                c.Nome,
                c.NomePascalCase,
                c.Tipo,
                c.TipoCSharp,
                c.Tamanho,
                c.IsNullable,
                c.IsPrimaryKey,
                c.IsIdentity,
                c.IsComputed,
                c.IsGuid,
                c.Descricao,
                ForeignKey = c.ForeignKey != null ? new
                {
                    c.ForeignKey.TabelaDestino,
                    c.ForeignKey.ColunaDestino,
                    c.ForeignKey.EntidadeDestino,
                    c.ForeignKey.NavigationPropertyName,
                    c.ForeignKey.ColunaDisplay,
                    c.ForeignKey.IsFkByGuid,
                    c.ForeignKey.IsFkByCodigo,
                    c.ForeignKey.IsParteDeFkComposta
                } : null
            }),
            ForeignKeys = tabela.ForeignKeys.Select(fk => new
            {
                fk.Nome,
                fk.ColunaOrigem,
                fk.TabelaDestino,
                fk.ColunaDestino,
                fk.EntidadeDestino,
                fk.NavigationPropertyName,
                fk.ColunaDisplay,
                fk.IsFkByGuid,
                fk.IsFkByCodigo,
                fk.IsParteDeFkComposta,
                fk.TodasColunas
            }),
            Indices = tabela.Indices.Select(i => new
            {
                i.Nome,
                i.IsUnique,
                i.Colunas
            })
        });
    }

    /// <summary>
    /// Obtém lista de funções do sistema para pesquisa (AJAX).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetFuncoes()
    {
        try
        {
            var funcoes = await _dbService.GetFuncoesAsync();

            return Json(funcoes.Select(f => new
            {
                cdFuncao = f.CdFuncao,
                dcFuncao = f.DcFuncao,
                cdSistema = f.CdSistema
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar funções");
            return StatusCode(500, $"Erro ao buscar funções: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém lista de módulos disponíveis.
    /// </summary>
    [HttpGet]
    public IActionResult GetModulos()
    {
        var modulos = ModuloConfig.GetModulos();
        return Json(modulos.Select(m => new
        {
            m.Nome,
            m.Rota,
            m.CdSistema,
            m.Namespace,
            m.Icone
        }));
    }

    // =========================================================================
    // GERAÇÃO FULL-STACK (NOVO)
    // =========================================================================

    /// <summary>
    /// Gera código Full-Stack completo (AJAX).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GerarFullStack([FromBody] FullStackRequest request)
    {
        _logger.LogInformation("GerarFullStack iniciado para tabela: {Tabela}", request.NomeTabela);

        if (string.IsNullOrWhiteSpace(request.NomeTabela))
        {
            _logger.LogWarning("GerarFullStack: Nome da tabela vazio");
            return BadRequest(new { error = "Nome da tabela é obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(request.CdFuncao))
        {
            _logger.LogWarning("GerarFullStack: Código da função vazio para tabela {Tabela}", request.NomeTabela);
            return BadRequest(new { error = "Código da função é obrigatório." });
        }

        var tabela = await _dbService.GetTabelaAsync(request.NomeTabela);

        if (tabela == null)
        {
            _logger.LogWarning("GerarFullStack: Tabela não encontrada: {Tabela}", request.NomeTabela);
            return NotFound(new { error = $"Tabela '{request.NomeTabela}' não encontrada." });
        }

        _logger.LogInformation("GerarFullStack: Gerando código para {Tabela} (CdFuncao={CdFuncao})",
            request.NomeTabela, request.CdFuncao);

        var result = _fullStackGenerator.Generate(tabela, request);

        if (!result.Success)
        {
            _logger.LogError("GerarFullStack: Erro na geração para {Tabela}: {Error}",
                request.NomeTabela, result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation("GerarFullStack: Sucesso para {Tabela} - {FileCount} arquivos gerados",
            request.NomeTabela, result.AllFiles.Count());

        return Json(new
        {
            success = true,
            nomeEntidade = result.NomeEntidade,
            warnings = result.Warnings,
            navigationsGeradas = result.NavigationsGeradas,
            files = result.AllFiles.Select(f => new
            {
                f.FileName,
                f.RelativePath,
                f.FileType,
                contentPreview = f.Content.Length > 500
                    ? f.Content[..500] + "..."
                    : f.Content,
                contentLength = f.Content.Length
            })
        });
    }

    /// <summary>
    /// Preview do código Full-Stack (sem download).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PreviewFullStack([FromBody] FullStackRequest request)
    {
        _logger.LogInformation("PreviewFullStack iniciado para tabela: {Tabela}", request.NomeTabela);

        if (string.IsNullOrWhiteSpace(request.NomeTabela))
        {
            _logger.LogWarning("PreviewFullStack: Nome da tabela vazio");
            return BadRequest(new { error = "Nome da tabela é obrigatório." });
        }

        var tabela = await _dbService.GetTabelaAsync(request.NomeTabela);

        if (tabela == null)
        {
            _logger.LogWarning("PreviewFullStack: Tabela não encontrada: {Tabela}", request.NomeTabela);
            return NotFound(new { error = $"Tabela '{request.NomeTabela}' não encontrada." });
        }

        // Para preview, preenchemos CdFuncao se não informado
        if (string.IsNullOrWhiteSpace(request.CdFuncao))
        {
            request.CdFuncao = "PREVIEW";
            _logger.LogDebug("PreviewFullStack: CdFuncao preenchido com padrão 'PREVIEW'");
        }

        _logger.LogInformation("PreviewFullStack: Gerando preview para {Tabela}", request.NomeTabela);

        var result = _fullStackGenerator.Generate(tabela, request);

        if (!result.Success)
        {
            _logger.LogError("PreviewFullStack: Erro na geração para {Tabela}: {Error}",
                request.NomeTabela, result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation("PreviewFullStack: Sucesso para {Tabela} - {FileCount} arquivos gerados",
            request.NomeTabela, result.AllFiles.Count());

        return Json(new
        {
            success = true,
            nomeEntidade = result.NomeEntidade,
            warnings = result.Warnings,
            navigationsGeradas = result.NavigationsGeradas,
            files = result.AllFiles.Select(f => new
            {
                f.FileName,
                f.RelativePath,
                f.FileType,
                content = f.Content
            })
        });
    }

    /// <summary>
    /// Download do ZIP com todos os arquivos Full-Stack.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DownloadFullStackZip([FromBody] FullStackRequest request)
    {
        _logger.LogInformation("DownloadFullStackZip iniciado para tabela: {Tabela}", request.NomeTabela);

        if (string.IsNullOrWhiteSpace(request.NomeTabela))
        {
            _logger.LogWarning("DownloadFullStackZip: Nome da tabela vazio");
            return BadRequest("Nome da tabela é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.CdFuncao))
        {
            _logger.LogWarning("DownloadFullStackZip: Codigo da funcao vazio para tabela {Tabela}", request.NomeTabela);
            return BadRequest("Codigo da funcao eh obrigatorio.");
        }

        var tabela = await _dbService.GetTabelaAsync(request.NomeTabela);

        if (tabela == null)
        {
            _logger.LogWarning("DownloadFullStackZip: Tabela nao encontrada: {Tabela}", request.NomeTabela);
            return NotFound($"Tabela '{request.NomeTabela}' nao encontrada.");
        }

        _logger.LogInformation("DownloadFullStackZip: Gerando ZIP para {Tabela} (CdFuncao={CdFuncao})",
            request.NomeTabela, request.CdFuncao);

        var result = _fullStackGenerator.Generate(tabela, request);

        if (!result.Success)
        {
            _logger.LogError("DownloadFullStackZip: Erro na geracao para {Tabela}: {Error}",
                request.NomeTabela, result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("DownloadFullStackZip: Criando ZIP com {FileCount} arquivos para {Tabela}",
            result.AllFiles.Count(), request.NomeTabela);

        // Cria ZIP com todos os arquivos
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in result.AllFiles)
            {
                var entry = archive.CreateEntry(file.RelativePath);
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream, Encoding.UTF8);
                await writer.WriteAsync(file.Content);
            }

            // Adiciona README
            var readmeEntry = archive.CreateEntry("README.md");
            using (var entryStream = readmeEntry.Open())
            using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
            {
                await writer.WriteAsync(GenerateReadme(result, request));
            }
        }

        memoryStream.Position = 0;
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return File(memoryStream.ToArray(), "application/zip", $"{result.NomeEntidade}_FullStack_{timestamp}.zip");
    }

    // =========================================================================
    // GERAÇÃO LEGADA (Entidade + JSON) - COMPATIBILIDADE
    // =========================================================================

    /// <summary>
    /// Gera código para uma tabela (AJAX) - modo legado.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Gerar([FromBody] GeracaoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NomeTabela))
            return BadRequest(new { error = "Nome da tabela é obrigatório." });

        if (string.IsNullOrWhiteSpace(request.CdFuncao))
            return BadRequest(new { error = "Código da função é obrigatório." });

        var tabela = await _dbService.GetTabelaAsync(request.NomeTabela);

        if (tabela == null)
            return NotFound(new { error = $"Tabela '{request.NomeTabela}' não encontrada." });

        var result = _codeGenerator.Gerar(tabela, request);

        if (!result.Success)
            return BadRequest(new { error = result.Error });

        return Json(new
        {
            success = true,
            nomeEntidade = result.NomeEntidade,
            nomeArquivoEntidade = result.NomeArquivoEntidade,
            nomeArquivoJson = result.NomeArquivoJson,
            entidade = result.CodigoEntidade,
            config = result.JsonConfig,
            navigationsGeradas = result.NavigationsGeradas
        });
    }

    /// <summary>
    /// Download do arquivo .cs da entidade (legado).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DownloadEntidade([FromBody] GeracaoRequest request)
    {
        var tabela = await _dbService.GetTabelaAsync(request.NomeTabela);
        if (tabela == null)
            return NotFound();

        var result = _codeGenerator.Gerar(tabela, request);
        if (!result.Success)
            return BadRequest(result.Error);

        var bytes = Encoding.UTF8.GetBytes(result.CodigoEntidade);
        return File(bytes, "text/plain", result.NomeArquivoEntidade);
    }

    /// <summary>
    /// Download do arquivo .json de configuração (legado).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DownloadJson([FromBody] GeracaoRequest request)
    {
        var tabela = await _dbService.GetTabelaAsync(request.NomeTabela);
        if (tabela == null)
            return NotFound();

        var result = _codeGenerator.Gerar(tabela, request);
        if (!result.Success)
            return BadRequest(result.Error);

        var bytes = Encoding.UTF8.GetBytes(result.JsonConfig);
        return File(bytes, "application/json", result.NomeArquivoJson);
    }

    /// <summary>
    /// Download de ambos arquivos em ZIP (legado).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DownloadZip([FromBody] GeracaoRequest request)
    {
        var tabela = await _dbService.GetTabelaAsync(request.NomeTabela);
        if (tabela == null)
            return NotFound();

        var result = _codeGenerator.Gerar(tabela, request);
        if (!result.Success)
            return BadRequest(result.Error);

        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var entidadeEntry = archive.CreateEntry(result.NomeArquivoEntidade);
            using (var entryStream = entidadeEntry.Open())
            using (var writer = new StreamWriter(entryStream))
            {
                await writer.WriteAsync(result.CodigoEntidade);
            }

            var jsonEntry = archive.CreateEntry(result.NomeArquivoJson);
            using (var entryStream = jsonEntry.Open())
            using (var writer = new StreamWriter(entryStream))
            {
                await writer.WriteAsync(result.JsonConfig);
            }
        }

        memoryStream.Position = 0;
        return File(memoryStream.ToArray(), "application/zip", $"{result.NomeEntidade}.zip");
    }

    // =========================================================================
    // GERAÇÃO EM LOTE
    // =========================================================================

    /// <summary>
    /// Geração Full-Stack em lote (múltiplas tabelas).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GerarLoteFullStack([FromBody] List<FullStackRequest> requests)
    {
        if (requests == null || requests.Count == 0)
            return BadRequest(new { error = "Nenhuma tabela selecionada." });

        var results = new List<object>();

        foreach (var request in requests)
        {
            var tabela = await _dbService.GetTabelaAsync(request.NomeTabela);
            if (tabela == null)
            {
                results.Add(new { nomeTabela = request.NomeTabela, success = false, error = "Tabela não encontrada" });
                continue;
            }

            var result = _fullStackGenerator.Generate(tabela, request);
            results.Add(new
            {
                nomeTabela = request.NomeTabela,
                success = result.Success,
                error = result.Error,
                nomeEntidade = result.NomeEntidade,
                warnings = result.Warnings,
                filesCount = result.AllFiles.Count()
            });
        }

        return Json(new { results });
    }

    /// <summary>
    /// Download em lote Full-Stack (ZIP com todos os arquivos de todas as tabelas).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DownloadLoteFullStackZip([FromBody] List<FullStackRequest> requests)
    {
        if (requests == null || requests.Count == 0)
            return BadRequest("Nenhuma tabela selecionada.");

        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var request in requests)
            {
                var tabela = await _dbService.GetTabelaAsync(request.NomeTabela);
                if (tabela == null) continue;

                var result = _fullStackGenerator.Generate(tabela, request);
                if (!result.Success) continue;

                foreach (var file in result.AllFiles)
                {
                    // Agrupa por entidade
                    var entry = archive.CreateEntry($"{result.NomeEntidade}/{file.RelativePath}");
                    using var entryStream = entry.Open();
                    using var writer = new StreamWriter(entryStream, Encoding.UTF8);
                    await writer.WriteAsync(file.Content);
                }
            }
        }

        memoryStream.Position = 0;
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return File(memoryStream.ToArray(), "application/zip", $"FullStack_Lote_{timestamp}.zip");
    }

    // =========================================================================
    // UTILITÁRIOS
    // =========================================================================

    /// <summary>
    /// Recarrega cache de tabelas.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RefreshCache()
    {
        _dbService.InvalidateCache();
        var tabelas = await _dbService.GetTabelasAsync(forceRefresh: true);
        return Json(new { success = true, count = tabelas.Count });
    }

    /// <summary>
    /// Gera README para o ZIP.
    /// </summary>
    private static string GenerateReadme(FullStackResult result, FullStackRequest request)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# {result.NomeEntidade} - Full-Stack Generated Code");
        sb.AppendLine();
        sb.AppendLine($"**Gerado em:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"**Tabela:** {result.NomeTabela}");
        sb.AppendLine($"**Módulo:** {request.Modulo}");
        sb.AppendLine($"**CdFuncao:** {request.CdFuncao}");
        sb.AppendLine();

        sb.AppendLine("## Arquivos Gerados");
        sb.AppendLine();

        foreach (var file in result.AllFiles)
        {
            sb.AppendLine($"- `{file.RelativePath}` ({file.FileType})");
        }

        sb.AppendLine();
        sb.AppendLine("## Instruções de Instalação");
        sb.AppendLine();
        sb.AppendLine("1. Copie os arquivos para os diretórios correspondentes no projeto:");
        sb.AppendLine("   - `Domain/Entities/` → Módulo Domain");
        sb.AppendLine("   - `Web/Controllers/` → Projeto Web");
        sb.AppendLine("   - `Web/Models/` → Projeto Web");
        sb.AppendLine("   - `Web/Services/` → Projeto Web");
        sb.AppendLine("   - `Web/Views/` → Projeto Web");
        sb.AppendLine("   - `Web/wwwroot/js/` → Projeto Web");
        sb.AppendLine();
        sb.AppendLine("2. Registre o Service no DI:");
        sb.AppendLine($"   ```csharp");
        sb.AppendLine($"   services.AddHttpClient<I{result.NomeEntidade}ApiService, {result.NomeEntidade}ApiService>();");
        sb.AppendLine($"   ```");
        sb.AppendLine();
        sb.AppendLine("3. Compile o projeto para que o Source Generator crie o backend.");
        sb.AppendLine();

        if (result.Warnings.Count > 0)
        {
            sb.AppendLine("## Avisos");
            sb.AppendLine();
            foreach (var warning in result.Warnings)
            {
                sb.AppendLine($"⚠️ {warning}");
            }
            sb.AppendLine();
        }

        if (result.NavigationsGeradas.Count > 0)
        {
            sb.AppendLine("## Navigation Properties Geradas");
            sb.AppendLine();
            foreach (var nav in result.NavigationsGeradas)
            {
                sb.AppendLine($"- {nav}");
            }
        }

        return sb.ToString();
    }
}
