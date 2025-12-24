// =============================================================================
// RHSENSOERP WEB - BASE CRUD CONTROLLER (ATUALIZADO COM CACHE DE PERMISSÕES)
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
/// Controller abstrato para operações CRUD padrão com suporte a cache de permissões.
/// Fornece métodos reutilizáveis para listagem, criação, edição e exclusão.
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
    protected readonly IApiService<TDto, TCreateDto, TUpdateDto, TKey> _apiService;
    protected readonly IUserPermissionsCacheService _permissionsCache;
    protected readonly ILogger _logger;

    protected BaseCrudController(
        IApiService<TDto, TCreateDto, TUpdateDto, TKey> apiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger logger)
    {
        _apiService = apiService;
        _permissionsCache = permissionsCache;
        _logger = logger;
    }

    #region Mensagens TempData

    /// <summary>
    /// Define mensagem de sucesso.
    /// </summary>
    protected void SetSuccessMessage(string message)
    {
        TempData["SuccessMessage"] = message;
    }

    /// <summary>
    /// Define mensagem de erro.
    /// </summary>
    protected void SetErrorMessage(string message)
    {
        TempData["ErrorMessage"] = message;
    }

    /// <summary>
    /// Define mensagem de aviso.
    /// </summary>
    protected void SetWarningMessage(string message)
    {
        TempData["WarningMessage"] = message;
    }

    /// <summary>
    /// Define mensagem de informação.
    /// </summary>
    protected void SetInfoMessage(string message)
    {
        TempData["InfoMessage"] = message;
    }

    #endregion

    #region Respostas JSON Padronizadas

    /// <summary>
    /// Retorna resposta JSON de sucesso.
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
    /// Retorna resposta JSON de erro.
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

    #region Métodos de Permissão

    /// <summary>
    /// Obtém o código do usuário logado a partir das claims.
    /// </summary>
    protected string? GetCurrentUserCode() => User.GetCdUsuario();

    /// <summary>
    /// Obtém as permissões do usuário para uma função específica, buscando do cache.
    /// Formato retornado: "IAEC", "IAC", "C", etc.
    /// </summary>
    /// <param name="cdFuncao">Código da função/tela</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>String com ações permitidas</returns>
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
    /// Verifica se o usuário tem uma permissão específica para uma função.
    /// </summary>
    /// <param name="cdFuncao">Código da função</param>
    /// <param name="acao">Ação: 'I' (Incluir), 'A' (Alterar), 'E' (Excluir), 'C' (Consultar)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>True se tem permissão</returns>
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

    #region Actions CRUD Virtuais

    /// <summary>
    /// Action para listagem com DataTables (AJAX).
    /// </summary>
    [HttpPost]
    public virtual async Task<IActionResult> List([FromBody] DataTableRequest request)
    {
        try
        {
            var page = (request.Start / request.Length) + 1;
            var pageSize = request.Length;
            var search = request.Search?.Value;

            var result = await _apiService.GetPagedAsync(page, pageSize, search);

            if (!result.Success || result.Data == null)
            {
                return Json(new DataTableResponse<TDto>
                {
                    Draw = request.Draw,
                    RecordsTotal = 0,
                    RecordsFiltered = 0,
                    Data = new List<TDto>(),
                    Error = result.Message ?? "Erro ao buscar dados"
                });
            }

            return Json(new DataTableResponse<TDto>
            {
                Draw = request.Draw,
                RecordsTotal = result.Data.TotalCount,
                RecordsFiltered = result.Data.TotalCount,
                Data = result.Data.Items.ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar listagem DataTables");
            return Json(new DataTableResponse<TDto>
            {
                Draw = request.Draw,
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
            var result = await _apiService.GetByIdAsync(id);

            if (!result.Success || result.Data == null)
            {
                return JsonError(result.Message ?? "Registro não encontrado");
            }

            return JsonSuccess("Registro encontrado", result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar registro {Id}", id);
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return JsonError("Dados inválidos", errors);
            }

            var result = await _apiService.CreateAsync(dto);

            if (!result.Success)
            {
                return JsonError(result.Message ?? "Erro ao criar registro", result.Errors);
            }

            return JsonSuccess(result.Message ?? "Registro criado com sucesso", result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar registro");
            return JsonError("Erro ao criar registro");
        }
    }

    /// <summary>
    /// Action para atualizar registro (AJAX).
    /// </summary>
    [HttpPut]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Update(TKey id, [FromBody] TUpdateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return JsonError("Dados inválidos", errors);
            }

            var result = await _apiService.UpdateAsync(id, dto);

            if (!result.Success)
            {
                return JsonError(result.Message ?? "Erro ao atualizar registro", result.Errors);
            }

            return JsonSuccess(result.Message ?? "Registro atualizado com sucesso", result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar registro {Id}", id);
            return JsonError("Erro ao atualizar registro");
        }
    }

    /// <summary>
    /// Action para excluir registro (AJAX).
    /// </summary>
    [HttpDelete]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Delete(TKey id)
    {
        try
        {
            var result = await _apiService.DeleteAsync(id);

            if (!result.Success)
            {
                return JsonError(result.Message ?? "Erro ao excluir registro");
            }

            return JsonSuccess(result.Message ?? "Registro excluído com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir registro {Id}", id);
            return JsonError("Erro ao excluir registro");
        }
    }

    /// <summary>
    /// Action para exclusão múltipla (AJAX).
    /// Detecta automaticamente se o serviço implementa IBatchDeleteService para usar exclusão detalhada.
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

            // Verifica se o serviço implementa IBatchDeleteService para exclusão detalhada
            if (_apiService is IBatchDeleteService<TKey> batchService)
            {
                var result = await batchService.DeleteBatchAsync(ids);

                if (!result.Success)
                {
                    return JsonError(result.Message ?? "Erro ao excluir registros");
                }

                // Usa as propriedades corretas do BatchDeleteResultDto
                var batchResult = result.Data;
                return JsonSuccess(result.Message ?? "Operação concluída", new
                {
                    totalRequested = ids.Count,
                    successCount = batchResult?.SuccessCount ?? 0,
                    failureCount = batchResult?.FailureCount ?? 0,
                    errors = batchResult?.Errors ?? new List<BatchDeleteErrorDto>()
                });
            }

            // Fallback: Exclusão simples (sem detalhamento)
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
                }
            }

            var message = failureCount == 0
                ? $"Todos os {successCount} registros foram excluídos com sucesso"
                : $"{successCount} registros excluídos, {failureCount} falharam";

            return JsonSuccess(message, new
            {
                totalRequested = ids.Count,
                successCount,
                failureCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir múltiplos registros");
            return JsonError("Erro ao excluir registros");
        }
    }

    #endregion
}
