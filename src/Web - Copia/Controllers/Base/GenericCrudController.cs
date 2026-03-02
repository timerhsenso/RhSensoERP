// =============================================================================
// RHSENSOERP.WEB - GENERIC CRUD CONTROLLER
// =============================================================================
// Arquivo: src/RhSensoERP.Web/Controllers/Base/GenericCrudController.cs
// Descrição: Controller base genérico para operações CRUD usando metadados
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services;

namespace RhSensoERP.Web.Controllers.Base;

/// <summary>
/// Controller base genérico para operações CRUD usando metadados.
/// Fornece a View Index que renderiza a listagem dinâmica.
/// </summary>
[Authorize]
public abstract class GenericCrudController : Controller
{
    private readonly IMetadataService _metadataService;
    private readonly ILogger _logger;

    /// <summary>
    /// Nome da entidade (ex: "Banco", "Sistema").
    /// </summary>
    protected abstract string EntityName { get; }

    /// <summary>
    /// Nome do módulo (ex: "GestaoDePessoas", "Identity").
    /// </summary>
    protected virtual string? ModuleName => null;

    /// <summary>
    /// Exibir filtros avançados na listagem.
    /// </summary>
    protected virtual bool ShowAdvancedFilters => false;

    /// <summary>
    /// Construtor.
    /// </summary>
    protected GenericCrudController(
        IMetadataService metadataService,
        ILogger logger)
    {
        _metadataService = metadataService;
        _logger = logger;
    }

    /// <summary>
    /// Action principal - Renderiza a listagem genérica.
    /// </summary>
    [HttpGet]
    public virtual async Task<IActionResult> Index()
    {
        try
        {
            // Carrega metadados da entidade
            var metadata = await _metadataService.GetByNameAsync(EntityName);

            if (metadata == null)
            {
                _logger.LogWarning(
                    "[GenericCrud] Metadados não encontrados para entidade {EntityName}",
                    EntityName);

                TempData["Error"] = $"Metadados não encontrados para a entidade '{EntityName}'.";
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new GenericListViewModel
            {
                EntityName = EntityName,
                Metadata = metadata,
                ShowAdvancedFilters = ShowAdvancedFilters
            };

            // Permite customização do ViewModel antes de renderizar
            await CustomizeViewModelAsync(viewModel);

            return View("~/Views/Shared/_GenericList.cshtml", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "[GenericCrud] Erro ao carregar Index para entidade {EntityName}",
                EntityName);

            TempData["Error"] = "Erro ao carregar a página. Tente novamente.";
            return RedirectToAction("Index", "Home");
        }
    }

    /// <summary>
    /// Permite que controllers derivados customizem o ViewModel antes da renderização.
    /// </summary>
    protected virtual Task CustomizeViewModelAsync(GenericListViewModel viewModel)
    {
        return Task.CompletedTask;
    }
}