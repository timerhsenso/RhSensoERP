// =============================================================================
// RHSENSOERP GENERATOR v3.1 - WEB SERVICES TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/WebServicesTemplate.cs
// Versão: 3.1 - HttpClient services para Web MVC
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Collections.Generic;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Web Services (HttpClient).
/// </summary>
public static class WebServicesTemplate
{
    /// <summary>
    /// Gera a Interface do Service.
    /// </summary>
    public static string GenerateInterface(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.1
// Entity: {{info.EntityName}}
// =============================================================================
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using {{info.WebModelsNamespace}};
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Core.Common;

namespace {{info.WebServicesNamespace}};

/// <summary>
/// Interface do serviço de API para {{info.DisplayName}}.
/// </summary>
public interface I{{info.EntityName}}ApiService
{
    Task<Result<{{info.EntityName}}Dto>> GetByIdAsync({{pkType}} id, CancellationToken ct = default);
    Task<Result<PagedResult<{{info.EntityName}}Dto>>> GetPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<Result<{{pkType}}>> CreateAsync(Create{{info.EntityName}}Dto dto, CancellationToken ct = default);
    Task<Result<bool>> UpdateAsync({{pkType}} id, Update{{info.EntityName}}Dto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync({{pkType}} id, CancellationToken ct = default);
    Task<Result<bool>> DeleteMultipleAsync(List<{{pkType}}> ids, CancellationToken ct = default);
}
""";
    }

    /// <summary>
    /// Gera a Implementação do Service.
    /// </summary>
    public static string GenerateImplementation(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.1
// Entity: {{info.EntityName}}
// =============================================================================
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using {{info.WebModelsNamespace}};
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Core.Common;

namespace {{info.WebServicesNamespace}};

/// <summary>
/// Serviço de API para {{info.DisplayName}}.
/// </summary>
public sealed class {{info.EntityName}}ApiService : I{{info.EntityName}}ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<{{info.EntityName}}ApiService> _logger;
    private const string BaseUrl = "{{info.ApiFullRoute}}";

    public {{info.EntityName}}ApiService(
        HttpClient httpClient,
        ILogger<{{info.EntityName}}ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<{{info.EntityName}}Dto>> GetByIdAsync({{pkType}} id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Result<{{info.EntityName}}Dto>>(
                $"{BaseUrl}/{id}",
                ct);

            return response ?? Result<{{info.EntityName}}Dto>.Failure(
                Error.Failure("Api.Error", "Resposta nula da API"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {{info.DisplayName}} {Id}", id);
            return Result<{{info.EntityName}}Dto>.Failure(
                Error.Failure("Api.Error", $"Erro ao buscar: {ex.Message}"));
        }
    }

    public async Task<Result<PagedResult<{{info.EntityName}}Dto>>> GetPagedAsync(
        PagedRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var queryString = $"?page={request.Page}&pageSize={request.PageSize}&search={request.Search}";
            var response = await _httpClient.GetFromJsonAsync<Result<PagedResult<{{info.EntityName}}Dto>>>(
                $"{BaseUrl}{queryString}",
                ct);

            return response ?? Result<PagedResult<{{info.EntityName}}Dto>>.Failure(
                Error.Failure("Api.Error", "Resposta nula da API"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {{info.DisplayName}} paginado");
            return Result<PagedResult<{{info.EntityName}}Dto>>.Failure(
                Error.Failure("Api.Error", $"Erro ao buscar: {ex.Message}"));
        }
    }

    public async Task<Result<{{pkType}}>> CreateAsync(Create{{info.EntityName}}Dto dto, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto, ct);
            var result = await response.Content.ReadFromJsonAsync<Result<{{pkType}}>>(cancellationToken: ct);

            return result ?? Result<{{pkType}}>.Failure(
                Error.Failure("Api.Error", "Resposta nula da API"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar {{info.DisplayName}}");
            return Result<{{pkType}}>.Failure(
                Error.Failure("Api.Error", $"Erro ao criar: {ex.Message}"));
        }
    }

    public async Task<Result<bool>> UpdateAsync({{pkType}} id, Update{{info.EntityName}}Dto dto, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", dto, ct);
            var result = await response.Content.ReadFromJsonAsync<Result<bool>>(cancellationToken: ct);

            return result ?? Result<bool>.Failure(
                Error.Failure("Api.Error", "Resposta nula da API"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar {{info.DisplayName}} {Id}", id);
            return Result<bool>.Failure(
                Error.Failure("Api.Error", $"Erro ao atualizar: {ex.Message}"));
        }
    }

    public async Task<Result<bool>> DeleteAsync({{pkType}} id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}", ct);
            var result = await response.Content.ReadFromJsonAsync<Result<bool>>(cancellationToken: ct);

            return result ?? Result<bool>.Failure(
                Error.Failure("Api.Error", "Resposta nula da API"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir {{info.DisplayName}} {Id}", id);
            return Result<bool>.Failure(
                Error.Failure("Api.Error", $"Erro ao excluir: {ex.Message}"));
        }
    }

    public async Task<Result<bool>> DeleteMultipleAsync(List<{{pkType}}> ids, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/batch", ids, ct);
            var result = await response.Content.ReadFromJsonAsync<Result<bool>>(cancellationToken: ct);

            return result ?? Result<bool>.Failure(
                Error.Failure("Api.Error", "Resposta nula da API"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir múltiplos {{info.DisplayName}}");
            return Result<bool>.Failure(
                Error.Failure("Api.Error", $"Erro ao excluir: {ex.Message}"));
        }
    }
}
""";
    }
}