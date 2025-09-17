using Microsoft.OpenApi.Models;

namespace RhSensoERP.API.Configuration;

/// <summary>
/// Extensões para configuração do Swagger/OpenAPI
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Configura Swagger com documentação API e autenticação Bearer JWT
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <returns>Coleção de serviços para encadeamento</returns>
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "RhSensoERP API",
                Version = "v1",
                Description = "API para gerenciamento do sistema RhSensoERP com autenticação JWT e controle de permissões",
                Contact = new OpenApiContact
                {
                    Name = "Equipe RhSensoERP",
                    Email = "dev@rhsensoerp.com"
                }
            });

            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Exemplo: \"Authorization: Bearer {token}\""
            };
            c.AddSecurityDefinition("Bearer", scheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, new List<string>() } });

            // Incluir comentários XML
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });
        return services;
    }
}