using Microsoft.OpenApi.Models;

namespace RhSensoERP.API.Configuration;

/// <summary>
/// Extens�es para configura��o do Swagger/OpenAPI
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Configura Swagger com documenta��o API e autentica��o Bearer JWT
    /// </summary>
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            // Documenta��o por m�dulo
            c.SwaggerDoc("RHU", new OpenApiInfo
            {
                Title = "Recursos Humanos (RHU)",
                Version = "v1",
                Description = "M�dulo de Recursos Humanos"
            });

            c.SwaggerDoc("SEG", new OpenApiInfo
            {
                Title = "Seguran�a (SEG)",
                Version = "v1",
                Description = "M�dulo de Seguran�a e Autentica��o"
            });

            c.SwaggerDoc("FRE", new OpenApiInfo
            {
                Title = "Frequ�ncia (FRE)",
                Version = "v1",
                Description = "M�dulo de Controle de Frequ�ncia"
            });

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "RhSensoERP API - Geral",
                Version = "v1",
                Description = "API completa do sistema RhSensoERP",
                Contact = new OpenApiContact
                {
                    Name = "Equipe RhSensoERP",
                    Email = "dev@rhsensoerp.com"
                }
            });

            // Configura��o de autentica��o Bearer JWT
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header usando Bearer scheme. Digite apenas o token (sem 'Bearer'). Exemplo: eyJhbGciOiJIUzI1NiIsInR5..."
            };

            c.AddSecurityDefinition("Bearer", securityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Incluir coment�rios XML se dispon�veis
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