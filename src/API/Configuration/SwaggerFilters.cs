// ============================================================================
// src/API/Configuration/SwaggerFilters.cs
// ============================================================================
// Filtros customizados para o Swagger/OpenAPI.
// ============================================================================
#nullable enable
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

namespace RhSensoERP.API.Configuration;

/// <summary>
/// Filtro para adicionar valores padrão aos parâmetros do Swagger.
/// Útil para exibir valores default de parâmetros opcionais.
/// </summary>
public class SwaggerDefaultValuesFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        if (operation.Parameters == null)
            return;

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions
                .FirstOrDefault(p => p.Name == parameter.Name);

            if (description == null)
                continue;

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null &&
                description.DefaultValue != null &&
                description.DefaultValue.ToString() != "" &&
                description.ModelMetadata != null)
            {
                try
                {
                    var json = JsonSerializer.Serialize(description.DefaultValue, description.ModelMetadata.ModelType);
                    parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
                }
                catch
                {
                    // Ignora erros de serialização
                }
            }

            parameter.Required |= description.IsRequired;
        }
    }
}

/// <summary>
/// Filtro para converter paths do Swagger para lowercase.
/// Padroniza URLs como /api/usuarios ao invés de /api/Usuarios.
/// </summary>
public class LowercaseDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = new OpenApiPaths();

        foreach (var path in swaggerDoc.Paths)
        {
            paths.Add(path.Key.ToLowerInvariant(), path.Value);
        }

        swaggerDoc.Paths = paths;
    }
}

/// <summary>
/// Extensões auxiliares para ApiDescription.
/// </summary>
internal static class ApiDescriptionExtensions
{
    /// <summary>
    /// Verifica se um endpoint está marcado como obsoleto.
    /// </summary>
    public static bool IsDeprecated(this ApiDescription apiDescription)
    {
        return apiDescription.CustomAttributes()
            .Any(a => a.GetType() == typeof(ObsoleteAttribute));
    }
}