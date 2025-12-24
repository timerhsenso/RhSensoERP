<# 
.SYNOPSIS
    Gerador de CRUD para RhSensoERP - Gera arquivos nos projetos corretos
.DESCRIPTION
    Este script gera Controllers, Models e Services para Web e API
    diretamente nos projetos corretos.
.PARAMETER EntityName
    Nome da Entity (ex: Sistema, Usuario, Funcao)
.PARAMETER DisplayName
    Nome amigável para exibição (ex: "Sistema", "Usuário")
.PARAMETER PkType
    Tipo da chave primária (string, int, Guid). Padrão: string
.PARAMETER PkProperty
    Nome da propriedade PK (ex: CdSistema, Id). Padrão: Id
.PARAMETER CdSistema
    Código do sistema para permissões (ex: SEG, RHU)
.PARAMETER CdFuncao
    Código da função para permissões (ex: SEG_FM_TSISTEMA)
.PARAMETER Module
    Nome do módulo (ex: Identity, RHU). Padrão: Identity
.PARAMETER GenerateApi
    Gerar API Controller. Padrão: $true
.PARAMETER GenerateWeb
    Gerar Web Controller, Models e Services. Padrão: $true
.EXAMPLE
    .\Generate-Crud.ps1 -EntityName "Sistema" -DisplayName "Sistema" -PkType "string" -PkProperty "CdSistema" -CdSistema "SEG" -CdFuncao "SEG_FM_TSISTEMA"
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$EntityName,
    
    [Parameter(Mandatory=$true)]
    [string]$DisplayName,
    
    [string]$PkType = "string",
    
    [string]$PkProperty = "Id",
    
    [Parameter(Mandatory=$true)]
    [string]$CdSistema,
    
    [Parameter(Mandatory=$true)]
    [string]$CdFuncao,
    
    [string]$Module = "Identity",
    
    [bool]$GenerateApi = $true,
    
    [bool]$GenerateWeb = $true,
    
    [string]$SolutionRoot = "."
)

# ============================================================================
# CONFIGURAÇÃO
# ============================================================================

$PluralName = if ($EntityName -match "ao$") { $EntityName -replace "ao$", "oes" }
              elseif ($EntityName -match "l$") { $EntityName -replace "l$", "is" }
              elseif ($EntityName -match "m$") { $EntityName -replace "m$", "ns" }
              elseif ($EntityName -match "r$") { "${EntityName}es" }
              elseif ($EntityName -match "s$") { $EntityName }
              else { "${EntityName}s" }

$PluralNameLower = $PluralName.ToLower()
$EntityNameLower = $EntityName.ToLower()

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "RHSENSOERP CRUD GENERATOR v3.0" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Entity: $EntityName ($PluralName)" -ForegroundColor Yellow
Write-Host "Display: $DisplayName" -ForegroundColor Yellow
Write-Host "PK: $PkProperty ($PkType)" -ForegroundColor Yellow
Write-Host "Permissões: $CdSistema / $CdFuncao" -ForegroundColor Yellow
Write-Host "Module: $Module" -ForegroundColor Yellow
Write-Host ""

# Paths
$ApiPath = Join-Path $SolutionRoot "src/RhSensoERP.API/Controllers/$Module"
$WebControllersPath = Join-Path $SolutionRoot "src/Web/Controllers"
$WebModelsPath = Join-Path $SolutionRoot "src/Web/Models/$PluralName"
$WebServicesPath = Join-Path $SolutionRoot "src/Web/Services/$PluralName"

# ============================================================================
# TEMPLATES
# ============================================================================

# --- API CONTROLLER ---
$ApiControllerTemplate = @"
// =============================================================================
// ARQUIVO GERADO POR Generate-Crud.ps1
// Entity: $EntityName
// =============================================================================
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.$Module.Application.DTOs.$PluralName;
using RhSensoERP.$Module.Application.Features.$PluralName.Commands;
using RhSensoERP.$Module.Application.Features.$PluralName.Queries;
using RhSensoERP.Shared.Contracts.Common;

namespace RhSensoERP.API.Controllers.$Module;

/// <summary>
/// Controller da API para $DisplayName.
/// </summary>
[ApiController]
[Route("api/$($Module.ToLower())/$PluralNameLower")]
[Authorize]
public class ${PluralName}Controller : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<${PluralName}Controller> _logger;

    public ${PluralName}Controller(IMediator mediator, ILogger<${PluralName}Controller> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lista $DisplayName com paginação.
    /// </summary>
    [HttpPost("list")]
    [ProducesResponseType(typeof(PagedResult<${EntityName}Dto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromBody] PagedRequest request, CancellationToken ct)
    {
        _logger.LogDebug("Listando $DisplayName - Página {Page}", request.Page);
        
        var query = new Get${PluralName}PagedQuery(request);
        var result = await _mediator.Send(query, ct);
        
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error.Message });
            
        return Ok(result.Value);
    }

    /// <summary>
    /// Busca $DisplayName por ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(${EntityName}Dto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById($PkType id, CancellationToken ct)
    {
        var query = new GetBy${EntityName}IdQuery(id);
        var result = await _mediator.Send(query, ct);
        
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error.Message });
            
        return Ok(result.Value);
    }

    /// <summary>
    /// Cria novo $DisplayName.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof($PkType), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] Create${EntityName}Request request, CancellationToken ct)
    {
        _logger.LogInformation("Criando $DisplayName...");
        
        var command = new Create${EntityName}Command(request);
        var result = await _mediator.Send(command, ct);
        
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error.Message });
            
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Atualiza $DisplayName existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update($PkType id, [FromBody] Update${EntityName}Request request, CancellationToken ct)
    {
        _logger.LogInformation("Atualizando $DisplayName {Id}...", id);
        
        var command = new Update${EntityName}Command(id, request);
        var result = await _mediator.Send(command, ct);
        
        if (!result.IsSuccess)
        {
            if (result.Error.Code.Contains("NotFound"))
                return NotFound(new { error = result.Error.Message });
            return BadRequest(new { error = result.Error.Message });
        }
            
        return NoContent();
    }

    /// <summary>
    /// Exclui $DisplayName.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete($PkType id, CancellationToken ct)
    {
        _logger.LogInformation("Excluindo $DisplayName {Id}...", id);
        
        var command = new Delete${EntityName}Command(id);
        var result = await _mediator.Send(command, ct);
        
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error.Message });
            
        return NoContent();
    }

    /// <summary>
    /// Exclui múltiplos $DisplayName.
    /// </summary>
    [HttpDelete("batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteBatch([FromBody] List<$PkType> ids, CancellationToken ct)
    {
        _logger.LogInformation("Excluindo {Count} $DisplayName em lote...", ids.Count);
        
        var command = new Delete${PluralName}Command(ids);
        var result = await _mediator.Send(command, ct);
        
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error.Message });
            
        return Ok(result.Value);
    }
}
"@

# --- WEB CONTROLLER ---
$WebControllerTemplate = @"
// =============================================================================
// ARQUIVO GERADO POR Generate-Crud.ps1
// Entity: $EntityName
// =============================================================================
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.$PluralName;
using RhSensoERP.Web.Services.$PluralName;
using RhSensoERP.Web.Services.Permissions;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller Web MVC para $DisplayName.
/// </summary>
public class ${PluralName}Controller : BaseCrudController<
    ${EntityName}Dto,
    Create${EntityName}Dto,
    Update${EntityName}Dto,
    $PkType>
{
    private readonly IUserPermissionsCacheService _permissionsService;

    public ${PluralName}Controller(
        I${EntityName}ApiService apiService,
        IUserPermissionsCacheService permissionsService,
        ILogger<${PluralName}Controller> logger)
        : base(apiService, logger)
    {
        _permissionsService = permissionsService;
    }

    protected override string CdSistema => "$CdSistema";
    protected override string CdFuncao => "$CdFuncao";
    protected override string EntityDisplayName => "$DisplayName";
    protected override string ViewPath => "~/Views/$PluralName";

    protected override async Task<bool> CanViewAsync()
    {
        return await _permissionsService.HasPermissionAsync(CdSistema, CdFuncao, "C");
    }

    protected override async Task<bool> CanCreateAsync()
    {
        return await _permissionsService.HasPermissionAsync(CdSistema, CdFuncao, "I");
    }

    protected override async Task<bool> CanEditAsync()
    {
        return await _permissionsService.HasPermissionAsync(CdSistema, CdFuncao, "A");
    }

    protected override async Task<bool> CanDeleteAsync()
    {
        return await _permissionsService.HasPermissionAsync(CdSistema, CdFuncao, "E");
    }
}
"@

# --- WEB MODELS: DTO ---
$DtoTemplate = @"
// =============================================================================
// ARQUIVO GERADO POR Generate-Crud.ps1
// Entity: $EntityName
// =============================================================================
namespace RhSensoERP.Web.Models.$PluralName;

/// <summary>
/// DTO de leitura para $DisplayName.
/// </summary>
public class ${EntityName}Dto
{
    // TODO: Adicione as propriedades conforme a Entity
    public $PkType $PkProperty { get; set; } = default!;
}
"@

# --- WEB MODELS: CREATE DTO ---
$CreateDtoTemplate = @"
// =============================================================================
// ARQUIVO GERADO POR Generate-Crud.ps1
// Entity: $EntityName
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.$PluralName;

/// <summary>
/// DTO para criação de $DisplayName.
/// </summary>
public class Create${EntityName}Dto
{
    // TODO: Adicione as propriedades conforme a Entity (sem PK se for auto-gerada)
}
"@

# --- WEB MODELS: UPDATE DTO ---
$UpdateDtoTemplate = @"
// =============================================================================
// ARQUIVO GERADO POR Generate-Crud.ps1
// Entity: $EntityName
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.$PluralName;

/// <summary>
/// DTO para atualização de $DisplayName.
/// </summary>
public class Update${EntityName}Dto
{
    // TODO: Adicione as propriedades conforme a Entity
}
"@

# --- WEB MODELS: LIST VIEW MODEL ---
$ListViewModelTemplate = @"
// =============================================================================
// ARQUIVO GERADO POR Generate-Crud.ps1
// Entity: $EntityName
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.$PluralName;

/// <summary>
/// ViewModel para listagem de $DisplayName.
/// </summary>
public class ${PluralName}ListViewModel : BaseListViewModel
{
    public List<${EntityName}Dto> Items { get; set; } = new();
}
"@

# --- WEB SERVICE: INTERFACE ---
$ServiceInterfaceTemplate = @"
// =============================================================================
// ARQUIVO GERADO POR Generate-Crud.ps1
// Entity: $EntityName
// =============================================================================
using RhSensoERP.Web.Models.$PluralName;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.$PluralName;

/// <summary>
/// Interface do serviço de API para $DisplayName.
/// </summary>
public interface I${EntityName}ApiService 
    : IApiService<${EntityName}Dto, Create${EntityName}Dto, Update${EntityName}Dto, $PkType>,
      IBatchDeleteService<$PkType>
{
}
"@

# --- WEB SERVICE: IMPLEMENTATION ---
$ServiceImplementationTemplate = @"
// =============================================================================
// ARQUIVO GERADO POR Generate-Crud.ps1
// Entity: $EntityName
// =============================================================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using RhSensoERP.Web.Models.$PluralName;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.$PluralName;

/// <summary>
/// Implementação do serviço de API para $DisplayName.
/// </summary>
public class ${EntityName}ApiService : I${EntityName}ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<${EntityName}ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string ApiRoute = "api/$($Module.ToLower())/$PluralNameLower";

    public ${EntityName}ApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<${EntityName}ApiService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    private async Task SetAuthHeaderAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            var token = context.User.FindFirst("AccessToken")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    public async Task<ApiResponse<PagedResult<${EntityName}Dto>>> GetPagedAsync(
        int page, int pageSize, string? search = null, CancellationToken ct = default)
    {
        try
        {
            await SetAuthHeaderAsync();
            
            var request = new { page, pageSize, search };
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{ApiRoute}/list", content, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<PagedResult<${EntityName}Dto>>(responseContent, _jsonOptions);
                return ApiResponse<PagedResult<${EntityName}Dto>>.Success(result!);
            }
            
            _logger.LogWarning("[${EntityName.ToUpper()}] Erro ao listar: {Status} - {Content}", 
                response.StatusCode, responseContent);
            return ApiResponse<PagedResult<${EntityName}Dto>>.Failure($"Erro: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[${EntityName.ToUpper()}] Exceção ao listar");
            return ApiResponse<PagedResult<${EntityName}Dto>>.Failure(ex.Message);
        }
    }

    public async Task<ApiResponse<${EntityName}Dto>> GetByIdAsync($PkType id, CancellationToken ct = default)
    {
        try
        {
            await SetAuthHeaderAsync();
            
            var response = await _httpClient.GetAsync($"{ApiRoute}/{id}", ct);
            var content = await response.Content.ReadAsStringAsync(ct);
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<${EntityName}Dto>(content, _jsonOptions);
                return ApiResponse<${EntityName}Dto>.Success(result!);
            }
            
            return ApiResponse<${EntityName}Dto>.Failure($"Erro: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[${EntityName.ToUpper()}] Exceção ao buscar por ID");
            return ApiResponse<${EntityName}Dto>.Failure(ex.Message);
        }
    }

    public async Task<ApiResponse<$PkType>> CreateAsync(Create${EntityName}Dto dto, CancellationToken ct = default)
    {
        try
        {
            await SetAuthHeaderAsync();
            
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(ApiRoute, content, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<$PkType>(responseContent, _jsonOptions);
                return ApiResponse<$PkType>.Success(result!);
            }
            
            return ApiResponse<$PkType>.Failure($"Erro: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[${EntityName.ToUpper()}] Exceção ao criar");
            return ApiResponse<$PkType>.Failure(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> UpdateAsync($PkType id, Update${EntityName}Dto dto, CancellationToken ct = default)
    {
        try
        {
            await SetAuthHeaderAsync();
            
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"{ApiRoute}/{id}", content, ct);
            
            if (response.IsSuccessStatusCode)
                return ApiResponse<bool>.Success(true);
            
            var responseContent = await response.Content.ReadAsStringAsync(ct);
            return ApiResponse<bool>.Failure($"Erro: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[${EntityName.ToUpper()}] Exceção ao atualizar");
            return ApiResponse<bool>.Failure(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync($PkType id, CancellationToken ct = default)
    {
        try
        {
            await SetAuthHeaderAsync();
            
            var response = await _httpClient.DeleteAsync($"{ApiRoute}/{id}", ct);
            
            if (response.IsSuccessStatusCode)
                return ApiResponse<bool>.Success(true);
            
            return ApiResponse<bool>.Failure($"Erro: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[${EntityName.ToUpper()}] Exceção ao excluir");
            return ApiResponse<bool>.Failure(ex.Message);
        }
    }

    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(
        List<$PkType> ids, CancellationToken ct = default)
    {
        try
        {
            await SetAuthHeaderAsync();
            
            var json = JsonSerializer.Serialize(ids, _jsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{ApiRoute}/batch")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            
            var response = await _httpClient.SendAsync(request, ct);
            var content = await response.Content.ReadAsStringAsync(ct);
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<BatchDeleteResultDto>(content, _jsonOptions);
                return ApiResponse<BatchDeleteResultDto>.Success(result!);
            }
            
            return ApiResponse<BatchDeleteResultDto>.Failure($"Erro: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[${EntityName.ToUpper()}] Exceção ao excluir em lote");
            return ApiResponse<BatchDeleteResultDto>.Failure(ex.Message);
        }
    }
}
"@

# ============================================================================
# GERAÇÃO DOS ARQUIVOS
# ============================================================================

function Write-FileWithDirectory {
    param([string]$Path, [string]$Content)
    
    $dir = Split-Path $Path -Parent
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }
    
    Set-Content -Path $Path -Value $Content -Encoding UTF8
    Write-Host "  ✓ $Path" -ForegroundColor Green
}

# API Controller
if ($GenerateApi) {
    Write-Host "`n📁 Gerando API Controller..." -ForegroundColor Cyan
    $apiFile = Join-Path $ApiPath "${PluralName}Controller.cs"
    Write-FileWithDirectory -Path $apiFile -Content $ApiControllerTemplate
}

# Web Controller, Models e Services
if ($GenerateWeb) {
    Write-Host "`n📁 Gerando Web Controller..." -ForegroundColor Cyan
    $webControllerFile = Join-Path $WebControllersPath "${PluralName}Controller.cs"
    Write-FileWithDirectory -Path $webControllerFile -Content $WebControllerTemplate
    
    Write-Host "`n📁 Gerando Web Models..." -ForegroundColor Cyan
    Write-FileWithDirectory -Path (Join-Path $WebModelsPath "${EntityName}Dto.cs") -Content $DtoTemplate
    Write-FileWithDirectory -Path (Join-Path $WebModelsPath "Create${EntityName}Dto.cs") -Content $CreateDtoTemplate
    Write-FileWithDirectory -Path (Join-Path $WebModelsPath "Update${EntityName}Dto.cs") -Content $UpdateDtoTemplate
    Write-FileWithDirectory -Path (Join-Path $WebModelsPath "${PluralName}ListViewModel.cs") -Content $ListViewModelTemplate
    
    Write-Host "`n📁 Gerando Web Services..." -ForegroundColor Cyan
    Write-FileWithDirectory -Path (Join-Path $WebServicesPath "I${EntityName}ApiService.cs") -Content $ServiceInterfaceTemplate
    Write-FileWithDirectory -Path (Join-Path $WebServicesPath "${EntityName}ApiService.cs") -Content $ServiceImplementationTemplate
}

Write-Host "`n======================================" -ForegroundColor Green
Write-Host "✅ GERAÇÃO CONCLUÍDA!" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""
Write-Host "📝 Próximos passos:" -ForegroundColor Yellow
Write-Host "  1. Complete as propriedades nos DTOs gerados" -ForegroundColor White
Write-Host "  2. Registre o Service no DI (Program.cs ou Startup.cs):" -ForegroundColor White
Write-Host "     services.AddHttpClient<I${EntityName}ApiService, ${EntityName}ApiService>();" -ForegroundColor Gray
Write-Host "  3. Crie a View em Views/$PluralName/Index.cshtml" -ForegroundColor White
Write-Host "  4. Crie o JavaScript em wwwroot/js/pages/$PluralNameLower.js" -ForegroundColor White
Write-Host ""
