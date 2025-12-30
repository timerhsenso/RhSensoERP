// =============================================================================
// BASE CRUD CONTROLLER - COM SUPORTE A ORDENAÇÃO E PERMISSÕES
// =============================================================================
// Versão: 3.0
// Atualizado: 2024-12-30
// Changelog v3.0:
//   ✅ Adicionado processamento de ordenação do DataTables
//   ✅ Logs estruturados e informativos
//   ✅ Validação robusta de dados
//   ✅ Tratamento de erros completo
//   ✅ Cache de permissões integrado
//   ✅ Suporte a exclusão em lote
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Extensions;
using RhSensoERP.Web.Models.Base;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;
using RhSensoERP.Web.Services.Permissions;

namespace RhSensoERP.Web.Controllers.Base;

/// <summary>
/// Controller abstrato para operações CRUD padrão com suporte a:
/// - Paginação server-side
/// - Ordenação dinâmica
/// - Cache de permissões
/// - DataTables integration
/// - Exclusão em lote
/// </summary>
/// <typeparam name="TDto">Tipo do DTO completo</typeparam>
/// <typeparam name="TCreateDto">Tipo do DTO de criação</typeparam>
/// <typeparam name="TUpdateDto">Tipo do DTO de atualização</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
[Authorize]
public abstract class BaseCrudController<TDto, TCreateDto, TUpdateDto, TKey> : Controller
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    #region Fields & Constructor

    protected readonly IApiService<TDto, TCreateDto, TUpdateDto, TKey> _apiService;
    protected readonly IUserPermissionsCacheService _permissionsCache;
    protected readonly ILogger _logger;

    protected BaseCrudController(
        IApiService<TDto, TCreateDto, TUpdateDto, TKey> apiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger logger)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _permissionsCache = permissionsCache ?? throw new ArgumentNullException(nameof(permissionsCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    #region TempData Messages

    /// <summary>
    /// Define mensagem de sucesso.
    /// </summary>
    protected void SetSuccessMessage(string message) => TempData["SuccessMessage"] = message;

    /// <summary>
    /// Define mensagem de erro.
    /// </summary>
    protected void SetErrorMessage(string message) => TempData["ErrorMessage"] = message;

    /// <summary>
    /// Define mensagem de aviso.
    /// </summary>
    protected void SetWarningMessage(string message) => TempData["WarningMessage"] = message;

    /// <summary>
    /// Define mensagem de informação.
    /// </summary>
    protected void SetInfoMessage(string message) => TempData["InfoMessage"] = message;

    #endregion

    #region JSON Responses

    /// <summary>
    /// Retorna resposta JSON de sucesso padronizada.
    /// </summary>
    protected IActionResult JsonSuccess(string message, object? data = null)
    {
        return Json(new
        {
            success = true,
            message,
            data
        });
    }

    /// <summary>
    /// Retorna resposta JSON de erro padronizada.
    /// </summary>
    protected IActionResult JsonError(string message, object? errors = null)
    {
        return Json(new
        {
            success = false,
            message,
            errors
        });
    }

    #endregion

    #region Permission Methods

    /// <summary>
    /// Obtém o código do usuário logado a partir das claims.
    /// </summary>
    protected string? GetCurrentUserCode() => User.GetCdUsuario();

    /// <summary>
    /// Obtém as permissões do usuário para uma função específica do cache.
    /// </summary>
    /// <param name="cdFuncao">Código da função/tela</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>String com ações permitidas (ex: "IAEC")</returns>
    protected async Task<string> GetUserPermissionsAsync(string cdFuncao, CancellationToken ct = default)
    {
        var cdUsuario = GetCurrentUserCode();
        if (string.IsNullOrWhiteSpace(cdUsuario))
        {
            _logger.LogWarning("Usuário não identificado ao buscar permissões para {CdFuncao}", cdFuncao);
            return string.Empty;
        }
        return await _permissionsCache.GetPermissionsForFunctionAsync(cdUsuario, cdFuncao, ct);
    }

    /// <summary>
    /// Verifica se o usuário tem uma permissão específica.
    /// </summary>
    /// <param name="cdFuncao">Código da função</param>
    /// <param name="acao">Ação: 'I' (Incluir), 'A' (Alterar), 'E' (Excluir), 'C' (Consultar)</param>
    /// <param name="ct">Token de cancelamento</param>
    protected async Task<bool> HasPermissionAsync(string cdFuncao, char acao, CancellationToken ct = default)
    {
        var cdUsuario = GetCurrentUserCode();
        if (string.IsNullOrWhiteSpace(cdUsuario))
        {
            return false;
        }
        return await _permissionsCache.HasPermissionAsync(cdUsuario, cdFuncao, acao, ct);
    }

    /// <summary>
    /// Verifica se o usuário pode Incluir.
    /// </summary>
    protected async Task<bool> CanCreateAsync(string cdFuncao, CancellationToken ct = default)
        => await HasPermissionAsync(cdFuncao, 'I', ct);

    /// <summary>
    /// Verifica se o usuário pode Alterar.
    /// </summary>
    protected async Task<bool> CanEditAsync(string cdFuncao, CancellationToken ct = default)
        => await HasPermissionAsync(cdFuncao, 'A', ct);

    /// <summary>
    /// Verifica se o usuário pode Excluir.
    /// </summary>
    protected async Task<bool> CanDeleteAsync(string cdFuncao, CancellationToken ct = default)
        => await HasPermissionAsync(cdFuncao, 'E', ct);

    /// <summary>
    /// Verifica se o usuário pode Consultar.
    /// </summary>
    protected async Task<bool> CanViewAsync(string cdFuncao, CancellationToken ct = default)
        => await HasPermissionAsync(cdFuncao, 'C', ct);

    #endregion

    #region CRUD Actions

    /// <summary>
    /// Action para listagem com DataTables (AJAX) com suporte a ordenação server-side.
    /// </summary>
    /// <remarks>
    /// Processa automaticamente:
    /// - Paginação (start, length)
    /// - Busca global (search.value)
    /// - Ordenação (order[].column, order[].dir)
    /// - Mapeamento de colunas (columns[].name)
    /// </remarks>
    [HttpPost]
    public virtual async Task<IActionResult> List([FromBody] DataTableRequest request)
    {
        try
        {
            // =================================================================
            // ✅ VALIDAÇÃO DE ENTRADA
            // =================================================================
            if (request == null)
            {
                _logger.LogWarning("Request DataTables nulo recebido");
                return Json(CreateEmptyDataTableResponse(0));
            }

            if (request.Length <= 0 || request.Length > 1000)
            {
                _logger.LogWarning("PageSize inválido: {Length}", request.Length);
                request.Length = 25; // Valor padrão seguro
            }

            // =================================================================
            // ✅ PROCESSAMENTO DE PAGINAÇÃO
            // =================================================================
            var page = (request.Start / request.Length) + 1;
            var pageSize = request.Length;
            var search = request.Search?.Value;

            // =================================================================
            // ✅ PROCESSAMENTO DE ORDENAÇÃO
            // =================================================================
            string? orderBy = null;
            bool ascending = true;

            if (request.Order != null && request.Order.Any() && request.Columns != null)
            {
                var firstOrder = request.Order.First();
                var orderColumn = firstOrder.Column;

                // Valida índice da coluna
                if (orderColumn >= 0 && orderColumn < request.Columns.Count)
                {
                    var column = request.Columns[orderColumn];

                    // ✅ Prioriza 'Name' (PascalCase) para ordenação no backend
                    // Fallback para 'Data' se 'Name' não estiver presente
                    orderBy = !string.IsNullOrWhiteSpace(column.Name)
                        ? column.Name
                        : column.Data;

                    ascending = string.Equals(firstOrder.Dir, "asc", StringComparison.OrdinalIgnoreCase);

                    _logger.LogDebug(
                        "📊 Ordenação DataTables - Campo: {OrderBy}, Direção: {Direction}, Coluna: {ColumnIndex}",
                        orderBy,
                        ascending ? "ASC" : "DESC",
                        orderColumn
                    );
                }
                else
                {
                    _logger.LogWarning(
                        "⚠️ Índice de coluna inválido para ordenação: {ColumnIndex} (Total: {TotalColumns})",
                        orderColumn,
                        request.Columns?.Count ?? 0
                    );
                }
            }

            // =================================================================
            // ✅ CHAMADA À API COM TODOS OS PARÂMETROS
            // =================================================================
            _logger.LogDebug(
                "📤 Chamando API - Page: {Page}, PageSize: {PageSize}, Search: {Search}, OrderBy: {OrderBy}, Ascending: {Ascending}",
                page, pageSize, search ?? "null", orderBy ?? "null", ascending
            );

            var result = await _apiService.GetPagedAsync(
                page,
                pageSize,
                search,
                orderBy,      // ✅ Campo de ordenação
                ascending     // ✅ Direção
            );

            // =================================================================
            // ✅ PROCESSAMENTO DA RESPOSTA
            // =================================================================
            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning(
                    "⚠️ API retornou erro ou dados nulos - Success: {Success}, Message: {Message}",
                    result.Success,
                    result.Message ?? "null"
                );

                return Json(new DataTableResponse<TDto>
                {
                    Draw = request.Draw,
                    RecordsTotal = 0,
                    RecordsFiltered = 0,
                    Data = new List<TDto>(),
                    Error = result.Message ?? "Erro ao buscar dados"
                });
            }

            var response = new DataTableResponse<TDto>
            {
                Draw = request.Draw,
                RecordsTotal = result.Data.TotalCount,
                RecordsFiltered = result.Data.TotalCount,
                Data = result.Data.Items.ToList()
            };

            _logger.LogDebug(
                "✅ DataTables Response - Total: {Total}, Retornados: {Count}, Draw: {Draw}",
                response.RecordsTotal,
                response.Data.Count,
                response.Draw
            );

            return Json(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar listagem DataTables");

            return Json(new DataTableResponse<TDto>
            {
                Draw = request?.Draw ?? 0,
                RecordsTotal = 0,
                RecordsFiltered = 0,
                Data = new List<TDto>(),
                Error = "Erro ao buscar dados"
            });
        }
    }

    /// <summary>
    /// Action para obter registro por ID (AJAX).
    /// </summary>
    [HttpGet]
    public virtual async Task<IActionResult> GetById(TKey id)
    {
        try
        {
            if (EqualityComparer<TKey>.Default.Equals(id, default))
            {
                return JsonError("ID inválido");
            }

            _logger.LogDebug("🔍 GetById - ID: {Id}", id);

            var result = await _apiService.GetByIdAsync(id);

            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning("⚠️ Registro não encontrado - ID: {Id}", id);
                return JsonError(result.Message ?? "Registro não encontrado");
            }

            return JsonSuccess("Registro encontrado", result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao buscar registro {Id}", id);
            return JsonError("Erro ao buscar registro");
        }
    }

    /// <summary>
    /// Action para criar registro (AJAX).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Create([FromBody] TCreateDto dto)
    {
        try
        {
            if (dto == null)
            {
                return JsonError("Dados inválidos: objeto nulo");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                _logger.LogWarning("⚠️ Validação falhou ao criar registro: {Errors}",
                    string.Join(", ", errors.SelectMany(e => e.Value)));

                return JsonError("Dados inválidos", errors);
            }

            _logger.LogDebug("➕ Criando registro");

            var result = await _apiService.CreateAsync(dto);

            if (!result.Success)
            {
                _logger.LogWarning("⚠️ Falha ao criar registro: {Message}", result.Message);
                return JsonError(result.Message ?? "Erro ao criar registro", result.Errors);
            }

            _logger.LogInformation("✅ Registro criado com sucesso");
            return JsonSuccess(result.Message ?? "Registro criado com sucesso", result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao criar registro");
            return JsonError("Erro ao criar registro");
        }
    }

    /// <summary>
    /// Action para atualizar registro via PUT (REST padrão).
    /// </summary>
    [HttpPut]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Update(TKey id, [FromBody] TUpdateDto dto)
    {
        try
        {
            if (EqualityComparer<TKey>.Default.Equals(id, default))
            {
                return JsonError("ID inválido");
            }

            if (dto == null)
            {
                return JsonError("Dados inválidos: objeto nulo");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                _logger.LogWarning("⚠️ Validação falhou ao atualizar registro {Id}: {Errors}",
                    id, string.Join(", ", errors.SelectMany(e => e.Value)));

                return JsonError("Dados inválidos", errors);
            }

            _logger.LogDebug("✏️ Atualizando registro {Id}", id);

            var result = await _apiService.UpdateAsync(id, dto);

            if (!result.Success)
            {
                _logger.LogWarning("⚠️ Falha ao atualizar registro {Id}: {Message}", id, result.Message);
                return JsonError(result.Message ?? "Erro ao atualizar registro", result.Errors);
            }

            _logger.LogInformation("✅ Registro {Id} atualizado com sucesso", id);
            return JsonSuccess(result.Message ?? "Registro atualizado com sucesso", result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao atualizar registro {Id}", id);
            return JsonError("Erro ao atualizar registro");
        }
    }

    /// <summary>
    /// Action para excluir registro (AJAX).
    /// Suporta tanto DELETE quanto POST para compatibilidade com navegadores antigos.
    /// </summary>
    [HttpPost]
    [HttpDelete]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Delete(TKey id)
    {
        try
        {
            if (EqualityComparer<TKey>.Default.Equals(id, default))
            {
                return JsonError("ID inválido");
            }

            _logger.LogDebug("🗑️ Excluindo registro {Id}", id);

            var result = await _apiService.DeleteAsync(id);

            if (!result.Success)
            {
                _logger.LogWarning("⚠️ Falha ao excluir registro {Id}: {Message}", id, result.Message);
                return JsonError(result.Message ?? "Erro ao excluir registro");
            }

            _logger.LogInformation("✅ Registro {Id} excluído com sucesso", id);
            return JsonSuccess(result.Message ?? "Registro excluído com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao excluir registro {Id}", id);
            return JsonError("Erro ao excluir registro");
        }
    }

    /// <summary>
    /// Action para exclusão múltipla (AJAX).
    /// Suporta exclusão detalhada via IBatchDeleteService se disponível.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> DeleteMultiple([FromBody] List<TKey> ids)
    {
        try
        {
            if (ids == null || ids.Count == 0)
            {
                return JsonError("Nenhum registro selecionado para exclusão");
            }

            _logger.LogDebug("🗑️ Excluindo múltiplos registros - Quantidade: {Count}", ids.Count);

            // Verifica se o serviço implementa IBatchDeleteService para exclusão detalhada
            if (_apiService is IBatchDeleteService<TKey> batchService)
            {
                var result = await batchService.DeleteBatchAsync(ids);

                if (!result.Success)
                {
                    _logger.LogWarning("⚠️ Falha na exclusão em lote: {Message}", result.Message);
                    return JsonError(result.Message ?? "Erro ao excluir registros");
                }

                var batchResult = result.Data;
                var message = batchResult?.FailureCount > 0
                    ? $"{batchResult.SuccessCount} registros excluídos, {batchResult.FailureCount} falharam"
                    : $"Todos os {batchResult?.SuccessCount ?? ids.Count} registros foram excluídos com sucesso";

                _logger.LogInformation(
                    "✅ Exclusão em lote concluída - Sucesso: {Success}, Falhas: {Failures}",
                    batchResult?.SuccessCount ?? 0,
                    batchResult?.FailureCount ?? 0
                );

                return JsonSuccess(message, new
                {
                    totalRequested = ids.Count,
                    successCount = batchResult?.SuccessCount ?? 0,
                    failureCount = batchResult?.FailureCount ?? 0,
                    errors = batchResult?.Errors ?? new List<BatchDeleteErrorDto>()
                });
            }

            // Fallback: Exclusão simples (um por vez)
            _logger.LogDebug("⚠️ IBatchDeleteService não implementado, usando exclusão sequencial");

            var successCount = 0;
            var failureCount = 0;

            foreach (var id in ids)
            {
                var result = await _apiService.DeleteAsync(id);
                if (result.Success)
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                    _logger.LogWarning("⚠️ Falha ao excluir registro {Id}: {Message}", id, result.Message);
                }
            }

            var finalMessage = failureCount == 0
                ? $"Todos os {successCount} registros foram excluídos com sucesso"
                : $"{successCount} registros excluídos, {failureCount} falharam";

            _logger.LogInformation(
                "✅ Exclusão múltipla concluída - Sucesso: {Success}, Falhas: {Failures}",
                successCount,
                failureCount
            );

            return JsonSuccess(finalMessage, new
            {
                totalRequested = ids.Count,
                successCount,
                failureCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao excluir múltiplos registros");
            return JsonError("Erro ao excluir registros");
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Cria uma resposta DataTables vazia.
    /// </summary>
    private DataTableResponse<TDto> CreateEmptyDataTableResponse(int draw)
    {
        return new DataTableResponse<TDto>
        {
            Draw = draw,
            RecordsTotal = 0,
            RecordsFiltered = 0,
            Data = new List<TDto>(),
            Error = null
        };
    }

    #endregion
}