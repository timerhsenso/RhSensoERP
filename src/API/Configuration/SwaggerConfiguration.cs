// ============================================================================
// src/API/Configuration/SwaggerConfiguration.cs
// ============================================================================
// Configuração centralizada do Swagger/OpenAPI para documentação da API.
// Organiza os endpoints por módulos e permite filtrar no dropdown.
//
// ✅ SUPORTA:
// - Controllers tradicionais
// - Controllers gerados via SourceGenerator
// - Agrupamento automático por módulo via ModuleGroupConvention
// ============================================================================
#nullable enable
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Serilog;

namespace RhSensoERP.API.Configuration;

public static class SwaggerConfiguration
{
    /// <summary>
    /// Definição dos módulos que aparecerão no dropdown do Swagger.
    /// Key = GroupName (deve corresponder ao retornado por ModuleGroupConvention)
    /// Title = Nome exibido no dropdown
    /// </summary>
    private static readonly (string Key, string Title)[] ModuleDocs =
    [
        // ===== Módulos Core =====
        ("Identity",              "🔐 Identity - Autenticação"),
        ("Diagnostics",           "🔧 Diagnostics - Monitoramento"),
        
        // ===== Módulos de Negócio =====
        ("GestaoDePessoas",       "👥 Gestão de Pessoas"),
        ("ControleDePonto",       "⏰ Controle de Ponto"),
        ("Avaliacoes",            "📊 Avaliações"),
        ("Esocial",               "📋 eSocial"),
        ("SaudeOcupacional",      "🏥 Saúde Ocupacional"),
        ("Treinamentos",          "📚 Treinamentos"),
        ("AuditoriaCompliance",   "📝 Auditoria & Compliance"),
        ("ControleAcessoPortaria","🚪 Controle de Acesso"),
        ("GestaoDeTerceiros",     "🤝 Gestão de Terceiros"),
        ("GestaoDeEPI",          "🦺 Gestão de EPI"),
        ("Integracoes",          "🔌 Integrações")
    ];

    /// <summary>
    /// Adiciona a configuração do Swagger aos serviços.
    /// </summary>
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        Log.Information("📘 Configurando Swagger com {Count} módulos", ModuleDocs.Length);

        services.AddSwaggerGen(c =>
        {
            // ===== DOCUMENTO GERAL (v1) - Contém TODOS os endpoints =====
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "RhSensoERP API - Todos os Módulos",
                Version = "v1",
                Description = "Documentação completa com todos os endpoints da API.\n\n" +
                              "Selecione um módulo específico no dropdown acima para filtrar os endpoints.",
                Contact = new OpenApiContact
                {
                    Name = "Equipe RhSenso",
                    Email = "suporte@rhsenso.com.br"
                }
            });

            // ===== DOCUMENTOS POR MÓDULO =====
            foreach (var (key, title) in ModuleDocs)
            {
                c.SwaggerDoc(key, new OpenApiInfo
                {
                    Title = title,
                    Version = "v1",
                    Description = $"Endpoints do módulo {title}."
                });

                Log.Debug("  📄 Módulo registrado: {Key} → {Title}", key, title);
            }

            // ===== CONFIGURAÇÃO DE SEGURANÇA JWT =====
            var jwtScheme = new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Insira o token JWT.\n\nExemplo: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            c.AddSecurityDefinition("Bearer", jwtScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [jwtScheme] = Array.Empty<string>()
            });

            // ===== CONFIGURAÇÕES GERAIS =====
            c.SupportNonNullableReferenceTypes();
            c.DescribeAllParametersInCamelCase();
            c.EnableAnnotations();
            c.UseInlineDefinitionsForEnums();
            c.CustomSchemaIds(t => t.FullName!.Replace("+", "."));

            // ===== XML COMMENTS (documentação dos controllers) =====
            // Inclui comentários XML de todos os assemblies carregados
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic))
            {
                try
                {
                    var xml = Path.ChangeExtension(asm.Location, ".xml");
                    if (File.Exists(xml))
                    {
                        c.IncludeXmlComments(xml, includeControllerXmlComments: true);
                        Log.Debug("  📝 XML Comments carregado: {Assembly}", asm.GetName().Name);
                    }
                }
                catch
                {
                    // Ignora assemblies que não podem ter XML comments
                }
            }

            // ===== FILTROS CUSTOMIZADOS =====
            c.OperationFilter<SwaggerDefaultValuesFilter>();
            c.DocumentFilter<LowercaseDocumentFilter>();

            // =====================================================================
            // ✅ CRÍTICO: TagActionsBy determina as SUBTAGS (nome do controller)
            // =====================================================================
            // Dentro de cada módulo, os endpoints são agrupados pelo nome do controller.
            // Ex: Módulo "GestaoDePessoas" → Tags: "Municipios", "Bancos", "Colaboradores"
            // =====================================================================
            c.TagActionsBy(api =>
            {
                // Usa o nome do controller como tag
                var controllerName = api.ActionDescriptor.RouteValues["controller"];

                if (!string.IsNullOrWhiteSpace(controllerName))
                {
                    return new[] { controllerName };
                }

                return new[] { "API" };
            });

            // =====================================================================
            // ✅ CRÍTICO: DocInclusionPredicate determina qual DOCUMENTO (dropdown)
            // =====================================================================
            // O GroupName é definido pelo ModuleGroupConvention baseado no namespace.
            // Ex: RhSensoERP.Modules.GestaoDePessoas.API.Controllers → "GestaoDePessoas"
            // =====================================================================
            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                // "v1" inclui TODOS os endpoints
                if (docName == "v1")
                    return true;

                // Outros documentos: filtra pelo GroupName do controller
                var groupName = apiDesc.GroupName;

                if (!string.IsNullOrWhiteSpace(groupName))
                {
                    var matches = string.Equals(groupName, docName, StringComparison.OrdinalIgnoreCase);

                    if (matches)
                    {
                        Log.Debug("  ✅ Incluindo {Action} no documento {Doc}",
                            apiDesc.ActionDescriptor.DisplayName, docName);
                    }

                    return matches;
                }

                // Se não tem GroupName, não inclui em documentos específicos
                return false;
            });
        });

        Log.Information("✅ Swagger configurado com {Total} documentos (v1 + {Modules} módulos)",
            ModuleDocs.Length + 1, ModuleDocs.Length);

        return services;
    }

    /// <summary>
    /// Configura o Swagger UI no pipeline da aplicação.
    /// </summary>
    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(ui =>
        {
            // ===== DOCUMENTO GERAL =====
            ui.SwaggerEndpoint("/swagger/v1/swagger.json", "📚 Todos os Módulos");

            // ===== DOCUMENTOS POR MÓDULO =====
            foreach (var (key, title) in ModuleDocs)
            {
                ui.SwaggerEndpoint($"/swagger/{key}/swagger.json", title);
            }

            // ===== CONFIGURAÇÕES DA UI =====
            ui.RoutePrefix = "swagger";
            ui.DocumentTitle = "RhSensoERP API Documentation";
            ui.DocExpansion(DocExpansion.List);      // Tags colapsadas por padrão
            ui.DefaultModelsExpandDepth(-1);         // Esconde schemas por padrão
            ui.EnableDeepLinking();                  // Permite links diretos para endpoints
            ui.EnableFilter();                       // Campo de busca
            ui.DisplayOperationId();                 // Mostra OperationId
            ui.DisplayRequestDuration();             // Mostra tempo de resposta
            ui.EnableTryItOutByDefault();            // Habilita "Try it out" por padrão
        });

        Log.Information("✅ Swagger UI configurada em /swagger");
        return app;
    }
}
