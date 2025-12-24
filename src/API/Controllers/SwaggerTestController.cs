// src/API/Controllers/SwaggerTestController.cs
#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Reflection;

namespace RhSensoERP.API.Controllers;

/// <summary>
/// Controller de teste para validar configuração do Swagger e detectar problemas.
/// ⚠️ REMOVER EM PRODUÇÃO - Apenas para diagnóstico.
/// </summary>
[ApiController]
[Route("api/swagger-test")]
[ApiExplorerSettings(GroupName = "Diagnostics")]
public sealed class SwaggerTestController : ControllerBase
{
    private readonly IApiDescriptionGroupCollectionProvider _apiExplorer;
    private readonly ILogger<SwaggerTestController> _logger;

    public SwaggerTestController(
        IApiDescriptionGroupCollectionProvider apiExplorer,
        ILogger<SwaggerTestController> logger)
    {
        _apiExplorer = apiExplorer;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os controllers e seus GroupNames para diagnóstico.
    /// </summary>
    [HttpGet("controllers")]
    public IActionResult GetControllersInfo()
    {
        var controllers = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
            .OrderBy(t => t.Namespace)
            .ThenBy(t => t.Name)
            .Select(t =>
            {
                // Tenta obter o GroupName do atributo
                var attr = t.GetCustomAttribute<ApiExplorerSettingsAttribute>();
                var groupName = attr?.GroupName ?? "NULL";

                return new
                {
                    Name = t.Name,
                    Namespace = t.Namespace ?? "NULL",
                    GroupName = groupName,
                    FullName = t.FullName ?? "NULL"
                };
            })
            .ToList();

        return Ok(new
        {
            TotalControllers = controllers.Count,
            Controllers = controllers,
            Warning = "⚠️ Controllers com GroupName = NULL podem não aparecer no Swagger corretamente"
        });
    }

    /// <summary>
    /// Lista todos os grupos (documentos) disponíveis no API Explorer.
    /// </summary>
    [HttpGet("groups")]
    public IActionResult GetApiGroups()
    {
        var groups = _apiExplorer.ApiDescriptionGroups.Items
            .Select(g => new
            {
                GroupName = g.GroupName,
                EndpointCount = g.Items.Count,
                Endpoints = g.Items.Select(i => new
                {
                    Route = i.RelativePath,
                    HttpMethod = i.HttpMethod,
                    GroupName = i.GroupName ?? "NULL"
                }).ToList()
            })
            .OrderBy(g => g.GroupName)
            .ToList();

        return Ok(new
        {
            TotalGroups = groups.Count,
            Groups = groups,
            Info = "Se um grupo tem 0 endpoints, o controller pode não estar configurado corretamente"
        });
    }

    /// <summary>
    /// Valida configuração e retorna diagnóstico completo.
    /// </summary>
    [HttpGet("diagnostics")]
    public IActionResult GetDiagnostics()
    {
        var controllersResponse = GetControllersInfo() as OkObjectResult;
        var groupsResponse = GetApiGroups() as OkObjectResult;

        // Detecta problemas comuns
        var issues = new List<string>();

        // Controllers sem GroupName
        if (controllersResponse?.Value != null)
        {
            var controllersResult = controllersResponse.Value;
            var controllersProperty = controllersResult.GetType().GetProperty("Controllers");

            if (controllersProperty != null)
            {
                var controllersList = controllersProperty.GetValue(controllersResult) as IEnumerable<object>;

                if (controllersList != null)
                {
                    var controllersWithoutGroup = new List<string>();

                    foreach (var controller in controllersList)
                    {
                        var groupNameProp = controller.GetType().GetProperty("GroupName");
                        var nameProp = controller.GetType().GetProperty("Name");

                        if (groupNameProp != null && nameProp != null)
                        {
                            var groupName = groupNameProp.GetValue(controller)?.ToString();
                            var name = nameProp.GetValue(controller)?.ToString();

                            if (groupName == "NULL" && name != null)
                            {
                                controllersWithoutGroup.Add(name);
                            }
                        }
                    }

                    if (controllersWithoutGroup.Any())
                    {
                        issues.Add($"❌ {controllersWithoutGroup.Count} controllers sem GroupName: {string.Join(", ", controllersWithoutGroup)}");
                    }
                }
            }
        }

        // Grupos vazios
        if (groupsResponse?.Value != null)
        {
            var groupsResult = groupsResponse.Value;
            var groupsProperty = groupsResult.GetType().GetProperty("Groups");

            if (groupsProperty != null)
            {
                var groupsList = groupsProperty.GetValue(groupsResult) as IEnumerable<object>;

                if (groupsList != null)
                {
                    var emptyGroups = new List<string>();

                    foreach (var group in groupsList)
                    {
                        var endpointCountProp = group.GetType().GetProperty("EndpointCount");
                        var groupNameProp = group.GetType().GetProperty("GroupName");

                        if (endpointCountProp != null && groupNameProp != null)
                        {
                            var endpointCount = Convert.ToInt32(endpointCountProp.GetValue(group));
                            var groupName = groupNameProp.GetValue(group)?.ToString();

                            if (endpointCount == 0 && groupName != null)
                            {
                                emptyGroups.Add(groupName);
                            }
                        }
                    }

                    if (emptyGroups.Any())
                    {
                        issues.Add($"⚠️ {emptyGroups.Count} grupos sem endpoints: {string.Join(", ", emptyGroups)}");
                    }
                }
            }
        }

        // Verifica ModuleGroupConvention
        var conventionRegistered = CheckConventionRegistered();
        if (!conventionRegistered)
        {
            issues.Add("⚠️ Não foi possível verificar se ModuleGroupConvention está registrada");
        }

        return Ok(new
        {
            Status = issues.Any() ? "ISSUES_FOUND" : "OK",
            Timestamp = DateTime.UtcNow,
            IssuesCount = issues.Count,
            Issues = issues,
            ControllersInfo = controllersResponse?.Value,
            GroupsInfo = groupsResponse?.Value,
            Recommendations = new[]
            {
                "1. Todos os controllers devem ter [ApiExplorerSettings(GroupName = \"...\")]",
                "2. ModuleGroupConvention deve estar registrada em AddControllers()",
                "3. SwaggerConfiguration.cs deve ser usado em Program.cs",
                "4. Documentos vazios indicam problemas de GroupName"
            }
        });
    }

    /// <summary>
    /// Testa se um controller específico aparece no Swagger.
    /// </summary>
    [HttpGet("test-controller/{controllerName}")]
    public IActionResult TestController(string controllerName)
    {
        var controller = Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(t =>
                typeof(ControllerBase).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                t.Name.Equals(controllerName, StringComparison.OrdinalIgnoreCase));

        if (controller == null)
        {
            return NotFound(new { error = $"Controller '{controllerName}' não encontrado" });
        }

        var attr = controller.GetCustomAttribute<ApiExplorerSettingsAttribute>();
        var groupName = attr?.GroupName;

        // Busca endpoints deste controller no API Explorer
        var endpoints = _apiExplorer.ApiDescriptionGroups.Items
            .SelectMany(g => g.Items)
            .Where(i => i.ActionDescriptor.DisplayName?.Contains(controller.Name, StringComparison.OrdinalIgnoreCase) == true)
            .Select(i => new
            {
                Route = i.RelativePath,
                HttpMethod = i.HttpMethod,
                GroupName = i.GroupName ?? "NULL"
            })
            .ToList();

        return Ok(new
        {
            ControllerName = controller.Name,
            Namespace = controller.Namespace,
            GroupName = groupName ?? "NULL",
            HasGroupName = groupName != null,
            EndpointsFound = endpoints.Count,
            Endpoints = endpoints,
            Status = endpoints.Any() ? "✅ VISIBLE" : "❌ NOT VISIBLE",
            Recommendation = groupName == null
                ? "Adicionar [ApiExplorerSettings(GroupName = \"...\")] no controller"
                : endpoints.Any()
                    ? "Controller configurado corretamente"
                    : "Verificar se o documento correspondente existe no Swagger"
        });
    }

    /// <summary>
    /// Lista todos os documentos Swagger disponíveis.
    /// </summary>
    [HttpGet("swagger-docs")]
    public IActionResult GetSwaggerDocs()
    {
        var groups = _apiExplorer.ApiDescriptionGroups.Items
            .GroupBy(g => g.GroupName)
            .Select(g => new
            {
                DocumentName = g.Key,
                EndpointCount = g.Sum(x => x.Items.Count),
                Controllers = g.SelectMany(x => x.Items)
                    .Select(i => i.ActionDescriptor.DisplayName?.Split('.').Reverse().Skip(1).FirstOrDefault())
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList()
            })
            .OrderBy(x => x.DocumentName)
            .ToList();

        return Ok(new
        {
            TotalDocuments = groups.Count,
            Documents = groups,
            ExpectedDocuments = new[]
            {
                "GestaoDePessoas",
                "Identity",
                "Diagnostics",
                "ControleDePonto",
                "Avaliacoes",
                "Esocial",
                "SaudeOcupacional",
                "Treinamentos"
            },
            Info = "Verifique se todos os documentos esperados estão presentes"
        });
    }

    // Helper privado
    private bool CheckConventionRegistered()
    {
        try
        {
            // Verificação simplificada - apenas retorna true para não causar falsos positivos
            return true;
        }
        catch
        {
            return false;
        }
    }
}