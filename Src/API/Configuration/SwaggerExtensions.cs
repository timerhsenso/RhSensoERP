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
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            // Documentação por módulo
            c.SwaggerDoc("RHU", new OpenApiInfo
            {
                Title = "Recursos Humanos (RHU)",
                Version = "v1",
                Description = "Módulo de Recursos Humanos"
            });

            c.SwaggerDoc("SEG", new OpenApiInfo
            {
                Title = "Segurança (SEG)",
                Version = "v1",
                Description = "Módulo de Segurança e Autenticação"
            });

            c.SwaggerDoc("FRE", new OpenApiInfo
            {
                Title = "Frequência (FRE)",
                Version = "v1",
                Description = "Módulo de Controle de Frequência"
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

            // Configuração de autenticação Bearer JWT
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

            // Incluir comentários XML se disponíveis
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