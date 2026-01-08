// =============================================================================
// GERADOR FULL-STACK v4.0 - FULL STACK GENERATOR SERVICE
// Orquestra todos os templates para gerar código completo
// ⭐ v4.0 - SELECT2 LOOKUP AUTOMÁTICO
// v3.2 - Suporte a ApiRoute do manifesto e organização por módulo
// ✅ v4.0.1 - CORRIGIDO: Logs de diagnóstico para colunas extras
// =============================================================================

using GeradorEntidades.Models;
using GeradorEntidades.Templates;

namespace GeradorEntidades.Services;

/// <summary>
/// Serviço que orquestra a geração de todos os arquivos do Full-Stack.
/// v4.0: Geração automática de DTOs de Lookup para Select2.
/// v3.2: Suporte a ApiRoute do manifesto e organização por módulo.
/// </summary>
public class FullStackGeneratorService
{
    private readonly ILogger<FullStackGeneratorService> _logger;

    public FullStackGeneratorService(ILogger<FullStackGeneratorService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gera todos os arquivos do Full-Stack.
    /// </summary>
    public FullStackResult Generate(TabelaInfo tabela, FullStackRequest request)
    {
        var result = new FullStackResult
        {
            NomeTabela = tabela.NomeTabela,
            NomeEntidade = tabela.NomePascalCase
        };

        try
        {
            _logger.LogInformation("Iniciando geração Full-Stack v4.0 para {Tabela}", tabela.NomeTabela);
            _logger.LogInformation("ApiRoute: {ApiRoute}", request.ApiRoute ?? "(não definida)");
            _logger.LogInformation("Módulo: {Modulo}", request.Modulo);

            // Validações
            var validationErrors = ValidateRequest(tabela, request);
            if (validationErrors.Count > 0)
            {
                result.Success = false;
                result.Error = string.Join("; ", validationErrors);
                return result;
            }

            // Warnings
            result.Warnings = GetWarnings(tabela, request);

            // Preenche configurações default se não fornecidas
            EnsureDefaultConfigurations(tabela, request);

            // Cria EntityConfig a partir dos dados
            _logger.LogInformation(
                "Processando configuração: {ListCount} colunas listagem, {FormCount} colunas formulário",
                request.ColunasListagem?.Count ?? 0,
                request.ColunasFormulario?.Count ?? 0);

            var entityConfig = EntityConfig.FromTabela(tabela, request);

            _logger.LogInformation(
                "EntityConfig criado: {PropertyCount} propriedades, PK={PrimaryKey}, ApiRoute={ApiRoute}",
                entityConfig.Properties.Count,
                entityConfig.PrimaryKey?.Name ?? "Não definida",
                entityConfig.ApiRoute);

            // =========================================================================
            // GERAÇÃO: BACKEND
            // =========================================================================

            if (request.GerarEntidade)
            {
                _logger.LogDebug("Gerando Entidade para {Entity}...", entityConfig.Name);
                var navigations = new List<string>();
                result.Entidade = EntityTemplate.Generate(tabela, request, navigations);
                result.NavigationsGeradas = navigations;
                _logger.LogDebug("Entidade gerada com {NavCount} navegações", navigations.Count);
            }

            // =========================================================================
            // GERAÇÃO: FRONTEND - CONTROLLERS
            // =========================================================================

            if (request.GerarWebController)
            {
                _logger.LogDebug("Gerando WebController para {Entity}...", entityConfig.Name);
                result.WebController = WebControllerTemplate.Generate(entityConfig);
                _logger.LogDebug("WebController gerado: {Path}", result.WebController.RelativePath);
            }

            // =========================================================================
            // GERAÇÃO: FRONTEND - MODELS
            // =========================================================================

            if (request.GerarWebModels)
            {
                _logger.LogDebug("Gerando WebModels para {Entity}...", entityConfig.Name);
                result.Dto = WebModelsTemplate.GenerateDto(entityConfig);
                result.CreateRequest = WebModelsTemplate.GenerateCreateRequest(entityConfig);
                result.UpdateRequest = WebModelsTemplate.GenerateUpdateRequest(entityConfig);
                result.ListViewModel = WebModelsTemplate.GenerateListViewModel(entityConfig);

                // ⭐ v4.0: GERA DTOS DE LOOKUP PARA SELECT2
                if (entityConfig.Select2Lookups.Any())
                {
                    _logger.LogDebug("Gerando {Count} DTOs de Lookup para Select2...", entityConfig.Select2Lookups.Count);
                    var lookupDtos = WebModelsTemplate.GenerateSelect2LookupDtos(entityConfig);
                    result.Select2LookupDtos = lookupDtos;

                    _logger.LogInformation(
                        "Gerados {Count} DTOs de Lookup: {Names}",
                        lookupDtos.Count,
                        string.Join(", ", entityConfig.Select2Lookups.Select(l => l.DtoName)));
                }

                _logger.LogDebug("WebModels gerados em: {Path}", result.Dto.RelativePath.Replace(result.Dto.FileName, ""));
            }

            // =========================================================================
            // GERAÇÃO: FRONTEND - SERVICES
            // =========================================================================

            if (request.GerarWebServices)
            {
                _logger.LogDebug("Gerando WebServices para {Entity}...", entityConfig.Name);
                result.ServiceInterface = WebServicesTemplate.GenerateInterface(entityConfig);
                result.ServiceImplementation = WebServicesTemplate.GenerateImplementation(entityConfig);
                _logger.LogDebug("WebServices gerados em: {Path}", result.ServiceInterface.RelativePath.Replace(result.ServiceInterface.FileName, ""));
            }

            // =========================================================================
            // GERAÇÃO: FRONTEND - VIEW
            // =========================================================================

            if (request.GerarView)
            {
                _logger.LogDebug("Gerando View para {Entity}...", entityConfig.Name);
                result.View = ViewTemplate.Generate(entityConfig);
                _logger.LogDebug("View gerada: {Path}", result.View.RelativePath);
            }

            // =========================================================================
            // GERAÇÃO: FRONTEND - JAVASCRIPT
            // =========================================================================

            if (request.GerarJavaScript)
            {
                _logger.LogDebug("Gerando JavaScript para {Entity}...", entityConfig.Name);
                result.JavaScript = JavaScriptTemplate.Generate(entityConfig);
                _logger.LogDebug("JavaScript gerado: {Path}", result.JavaScript.RelativePath);
            }

            result.Success = true;

            _logger.LogInformation(
                "Geração Full-Stack v4.0 concluída para {Tabela}: {FileCount} arquivos gerados",
                tabela.NomeTabela,
                result.AllFiles.Count());

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na geração Full-Stack para {Tabela}", tabela.NomeTabela);
            result.Success = false;
            result.Error = ex.Message;
            return result;
        }
    }

    /// <summary>
    /// Valida o request antes da geração.
    /// </summary>
    private List<string> ValidateRequest(TabelaInfo tabela, FullStackRequest request)
    {
        var errors = new List<string>();

        // Tabela sem PK no banco E sem PK definida pelo usuário - BLOQUEIA
        if (!tabela.HasPrimaryKey && (request.ColunasPkDefinidas == null || request.ColunasPkDefinidas.Count == 0))
        {
            errors.Add($"Tabela '{tabela.NomeTabela}' não possui Primary Key. Defina pelo menos uma coluna como chave primária.");
        }

        // CdFuncao obrigatório
        if (string.IsNullOrWhiteSpace(request.CdFuncao))
        {
            errors.Add("Código da Função (CdFuncao) é obrigatório.");
        }

        // Módulo vazio e sem CdSistema
        if (string.IsNullOrWhiteSpace(request.Modulo) && string.IsNullOrWhiteSpace(request.CdSistema))
        {
            errors.Add("Módulo ou CdSistema deve ser fornecido.");
        }

        return errors;
    }

    /// <summary>
    /// Gera warnings (não bloqueiam, mas informam).
    /// </summary>
    private List<string> GetWarnings(TabelaInfo tabela, FullStackRequest request)
    {
        var warnings = new List<string>();

        // PK Composta (do banco ou definida)
        var totalPks = tabela.PrimaryKeyColumns.Count + (request.ColunasPkDefinidas?.Count ?? 0);
        if (tabela.HasCompositePrimaryKey || totalPks > 1)
        {
            var pkCols = tabela.PrimaryKeyColumns.Select(c => c.Nome).ToList();
            if (request.ColunasPkDefinidas != null)
            {
                pkCols.AddRange(request.ColunasPkDefinidas.Select(p => p.Nome));
            }
            warnings.Add($"Tabela possui PK composta ({string.Join(", ", pkCols)}). Algumas funcionalidades podem ter limitações.");
        }

        // PK definida manualmente
        if (request.ColunasPkDefinidas != null && request.ColunasPkDefinidas.Count > 0)
        {
            var pkNames = string.Join(", ", request.ColunasPkDefinidas.Select(p => p.Nome));
            warnings.Add($"Chave primária definida manualmente: {pkNames}. Certifique-se de que essas colunas identificam registros únicos.");
        }

        // FK Composta
        if (tabela.HasCompositeForeignKeys)
        {
            var fksCompostas = tabela.ForeignKeys
                .Where(fk => fk.IsParteDeFkComposta)
                .Select(fk => fk.Nome)
                .Distinct()
                .ToList();

            warnings.Add($"Tabela possui FK(s) composta(s) ({string.Join(", ", fksCompostas)}). Navegações não serão geradas para essas FKs.");
        }

        // Tabela sem descrição
        if (string.IsNullOrWhiteSpace(tabela.Descricao))
        {
            warnings.Add("Tabela não possui Extended Property 'MS_Description'. Considere documentar a tabela.");
        }

        // ApiRoute não definida
        if (string.IsNullOrWhiteSpace(request.ApiRoute))
        {
            warnings.Add("ApiRoute não definida no manifesto. Será construída automaticamente.");
        }

        return warnings;
    }

    /// <summary>
    /// ✅ v4.0.1 CORRIGIDO: Garante que configurações default sejam preenchidas.
    /// Adicionados logs detalhados para diagnóstico de colunas extras.
    /// </summary>
    private void EnsureDefaultConfigurations(TabelaInfo tabela, FullStackRequest request)
    {
        // ═════════════════════════════════════════════════════════════════════
        // ✅ LOGS DE DIAGNÓSTICO
        // ═════════════════════════════════════════════════════════════════════

        _logger.LogInformation("═══════════════════════════════════════════════════════");
        _logger.LogInformation("🔍 EnsureDefaultConfigurations - DIAGNÓSTICO");
        _logger.LogInformation("═══════════════════════════════════════════════════════");
        _logger.LogInformation("📊 ColunasListagem recebidas: {Count}", request.ColunasListagem?.Count ?? 0);
        _logger.LogInformation("📝 ColunasFormulario recebidas: {Count}", request.ColunasFormulario?.Count ?? 0);

        if (request.ColunasListagem != null && request.ColunasListagem.Count > 0)
        {
            _logger.LogInformation("📋 Colunas de listagem recebidas do usuário:");
            for (int i = 0; i < request.ColunasListagem.Count; i++)
            {
                _logger.LogInformation("  {Index}. {Nome} (Visible: {Visible})",
                    i + 1, request.ColunasListagem[i].Nome, request.ColunasListagem[i].Visible);
            }
        }

        _logger.LogInformation("───────────────────────────────────────────────────────");

        // DisplayName
        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            request.DisplayName = FormatDisplayName(tabela.NomePascalCase);
        }

        // Módulo
        var modulo = ModuloConfig.GetModulos()
            .FirstOrDefault(m => m.CdSistema.Equals(request.CdSistema, StringComparison.OrdinalIgnoreCase));

        if (modulo != null)
        {
            if (string.IsNullOrWhiteSpace(request.Modulo))
                request.Modulo = modulo.Nome;
            if (string.IsNullOrWhiteSpace(request.ModuloRota))
                request.ModuloRota = modulo.Rota;
        }

        // ApiRoute default se não fornecida
        if (string.IsNullOrWhiteSpace(request.ApiRoute))
        {
            request.ApiRoute = $"api/{request.ModuloRota?.ToLowerInvariant() ?? "common"}/{tabela.NomePascalCase.ToLowerInvariant()}";
            _logger.LogDebug("ApiRoute construída automaticamente: {ApiRoute}", request.ApiRoute);
        }

        // ═════════════════════════════════════════════════════════════════════
        // ✅ COLUNAS DE LISTAGEM - RESPEITAR O QUE O USUÁRIO ENVIOU!
        // ═════════════════════════════════════════════════════════════════════

        if (request.ColunasListagem == null || request.ColunasListagem.Count == 0)
        {
            _logger.LogWarning("⚠️ Nenhuma coluna de listagem fornecida pelo usuário");
            _logger.LogInformation("🤖 Gerando automaticamente até 8 colunas da tabela");

            request.ColunasListagem = new List<ColumnListConfig>();
            var order = 0;

            foreach (var coluna in tabela.Colunas.Where(c => !c.IsPrimaryKey && !c.IsGuid))
            {
                request.ColunasListagem.Add(new ColumnListConfig
                {
                    Nome = coluna.Nome,
                    Visible = true,
                    Order = order++,
                    Title = FormatDisplayName(coluna.NomePascalCase),
                    Format = GetDefaultFormat(coluna),
                    Sortable = true
                });

                // Limita a 8 colunas por default
                if (order >= 8) break;
            }

            _logger.LogInformation("✅ {Count} colunas geradas automaticamente", request.ColunasListagem.Count);
        }
        else
        {
            // ✅ USUÁRIO JÁ ENVIOU COLUNAS - NÃO FAZER NADA!
            _logger.LogInformation("✅ Usando {Count} colunas fornecidas pelo usuário", request.ColunasListagem.Count);
            _logger.LogInformation("📌 NÃO gerando colunas extras (respeitando seleção do usuário)");

            // ❌ NÃO ADICIONAR NENHUMA COLUNA EXTRA AQUI!
            // ❌ NÃO FAZER: request.ColunasListagem.Add(...)
            // ❌ NÃO FAZER: foreach adicionar colunas

            // APENAS USE O QUE O USUÁRIO ENVIOU!
        }

        // ═════════════════════════════════════════════════════════════════════
        // ✅ COLUNAS DE FORMULÁRIO
        // ═════════════════════════════════════════════════════════════════════

        if (request.ColunasFormulario == null || request.ColunasFormulario.Count == 0)
        {
            _logger.LogWarning("⚠️ Nenhum campo de formulário fornecido pelo usuário");
            _logger.LogInformation("🤖 Gerando automaticamente campos da tabela");

            request.ColunasFormulario = new List<ColumnFormConfig>();
            var order = 0;

            foreach (var coluna in tabela.Colunas.Where(c => !c.IsPrimaryKey && !c.IsComputed))
            {
                request.ColunasFormulario.Add(new ColumnFormConfig
                {
                    Nome = coluna.Nome,
                    Visible = true,
                    Order = order++,
                    Label = FormatDisplayName(coluna.NomePascalCase),
                    InputType = GetDefaultInputType(coluna),
                    ColSize = GetDefaultColSize(coluna),
                    Required = !coluna.IsNullable
                });
            }

            _logger.LogInformation("✅ {Count} campos gerados automaticamente", request.ColunasFormulario.Count);
        }
        else
        {
            _logger.LogInformation("✅ Usando {Count} campos fornecidos pelo usuário", request.ColunasFormulario.Count);
            _logger.LogInformation("📌 NÃO gerando campos extras (respeitando seleção do usuário)");
        }

        // ═════════════════════════════════════════════════════════════════════
        // ✅ LOG FINAL
        // ═════════════════════════════════════════════════════════════════════

        _logger.LogInformation("═══════════════════════════════════════════════════════");
        _logger.LogInformation("✅ CONFIGURAÇÕES FINALIZADAS:");
        _logger.LogInformation("  - Colunas Listagem FINAL: {Count}", request.ColunasListagem.Count);
        _logger.LogInformation("  - Campos Formulário FINAL: {Count}", request.ColunasFormulario.Count);

        if (request.ColunasListagem.Count > 0)
        {
            _logger.LogInformation("📊 Colunas FINAIS da Grid:");
            for (int i = 0; i < request.ColunasListagem.Count; i++)
            {
                _logger.LogInformation("  {Index}. {Nome}", i + 1, request.ColunasListagem[i].Nome);
            }
        }

        _logger.LogInformation("═══════════════════════════════════════════════════════");
    }

    #region Helpers

    private static string FormatDisplayName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var sb = new System.Text.StringBuilder();

        for (int i = 0; i < name.Length; i++)
        {
            var c = name[i];

            if (i > 0 && char.IsUpper(c) && !char.IsUpper(name[i - 1]))
            {
                sb.Append(' ');
            }

            sb.Append(c);
        }

        return sb.ToString()
            .Replace("Cd ", "Código ")
            .Replace("Dc ", "")
            .Replace("Dt ", "Data ")
            .Replace("Nr ", "Número ")
            .Replace("Nm ", "")
            .Replace("Fl ", "")
            .Replace("Vl ", "Valor ")
            .Replace("Qt ", "Quantidade ")
            .Replace("Sg ", "Sigla ")
            .Replace("No ", "Número ")
            .Replace("Id ", "")
            .Trim();
    }

    private static string GetDefaultFormat(ColunaInfo coluna)
    {
        if (coluna.IsData) return "date";
        if (coluna.Tipo.ToLower() is "decimal" or "money" or "numeric") return "currency";
        if (coluna.IsBool) return "boolean";
        return "text";
    }

    private static string GetDefaultInputType(ColunaInfo coluna)
    {
        if (coluna.IsData) return coluna.Tipo.ToLower() == "date" ? "date" : "datetime-local";
        if (coluna.IsBool) return "checkbox";
        if (coluna.IsNumerico) return "number";
        if (coluna.IsTexto && coluna.Tamanho > 255) return "textarea";
        if (coluna.ForeignKey != null) return "select";
        return "text";
    }

    private static int GetDefaultColSize(ColunaInfo coluna)
    {
        if (coluna.IsBool) return 2;
        if (coluna.IsData) return 4;
        if (coluna.IsNumerico) return 3;
        if (coluna.Tamanho.HasValue && coluna.Tamanho < 20) return 3;
        if (coluna.Tamanho.HasValue && coluna.Tamanho > 255) return 12;
        return 6;
    }

    #endregion
}