// =============================================================================
// GERADORENTIDADES - MANIFEST SERVICE v3.2
// =============================================================================
// Arquivo: GeradorEntidades/Services/ManifestService.cs
// Descrição: Consome o manifesto de entidades do backend RhSensoERP
// v3.2 - Passa ApiRoute do manifesto para o FullStackRequest
// =============================================================================

using GeradorEntidades.Models;
using System.Text.Json;

namespace GeradorEntidades.Services;

/// <summary>
/// Serviço que consome o manifesto de entidades do backend RhSensoERP.
/// </summary>
public class ManifestService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ManifestService> _logger;
    private readonly IConfiguration _configuration;

    // Cache
    private ManifestData? _cachedManifest;
    private DateTime _cacheTime;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public ManifestService(
        HttpClient httpClient,
        ILogger<ManifestService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Obtém o manifesto completo de entidades.
    /// </summary>
    public async Task<ManifestData?> GetManifestAsync(bool forceRefresh = false)
    {
        // Cache
        if (!forceRefresh && _cachedManifest != null && DateTime.Now - _cacheTime < _cacheDuration)
        {
            _logger.LogDebug("Retornando manifesto do cache");
            return _cachedManifest;
        }

        try
        {
            _logger.LogInformation("Buscando manifesto da API: {Url}", _httpClient.BaseAddress);

            var response = await _httpClient.GetAsync("/api/manifest");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao buscar manifesto: {Status}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            _cachedManifest = JsonSerializer.Deserialize<ManifestData>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            _cacheTime = DateTime.Now;

            _logger.LogInformation("Manifesto carregado: {Count} entidades",
                _cachedManifest?.Entities.Count ?? 0);

            return _cachedManifest;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar manifesto da API");
            return null;
        }
    }

    /// <summary>
    /// Obtém lista de módulos disponíveis.
    /// </summary>
    public async Task<List<ModuleInfo>> GetModulesAsync()
    {
        var manifest = await GetManifestAsync();

        if (manifest == null)
            return new List<ModuleInfo>();

        return manifest.Entities
            .GroupBy(e => e.ModuleName)
            .Select(g => new ModuleInfo
            {
                Name = g.Key,
                DisplayName = g.First().ModuleDisplayName,
                EntityCount = g.Count(),
                Entities = g.Select(e => e.EntityName).ToList()
            })
            .OrderBy(m => m.DisplayName)
            .ToList();
    }

    /// <summary>
    /// Obtém entidades de um módulo específico.
    /// </summary>
    public async Task<List<EntityManifestItem>> GetEntitiesByModuleAsync(string moduleName)
    {
        var manifest = await GetManifestAsync();

        return manifest?.Entities
            .Where(e => e.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
            .ToList() ?? new List<EntityManifestItem>();
    }

    /// <summary>
    /// Obtém uma entidade específica.
    /// </summary>
    public async Task<EntityManifestItem?> GetEntityAsync(string entityName)
    {
        var manifest = await GetManifestAsync();

        return manifest?.Entities
            .FirstOrDefault(e => e.EntityName.Equals(entityName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Obtém o JSON de metadados diretamente do endpoint da entidade.
    /// Ex: /api/modulo/entidade/metadata
    /// </summary>
    public async Task<string?> GetMetadataAsync(string route)
    {
        try
        {
            if (string.IsNullOrEmpty(route)) return null;

            // Remove barra inicial se houver, pois BaseAddress já pode ter ou o setup do HttpClient cuida disso
            var cleanRoute = route.TrimStart('/');
            var url = $"{cleanRoute}/metadata";

            _logger.LogInformation("📡 Buscando metadados reais em: {Url}", url);

            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠️ Falha ao buscar metadata em {Url}. Status: {Status}", url, response.StatusCode);
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao buscar metadata da rota {Route}", route);
            return null;
        }
    }

    /// <summary>
    /// Obtém a entidade completa, tentando buscar metadados detalhados da API
    /// caso a propriedade Route esteja disponível.
    /// </summary>
    /// <summary>
    /// Obtém a entidade completa, tentando buscar metadados detalhados da API
    /// caso a propriedade Route esteja disponível.
    /// v3.3: Implementa Merge Seguro (Prioriza Tipos do Manifesto se Metadata vier como string genérica)
    /// </summary>
    public async Task<EntityManifestItem?> GetEntityWithMetadataAsync(string entityName)
    {
        // 1. Busca do manifesto (cache) - FONTE CONFIÁVEL
        var entity = await GetEntityAsync(entityName);
        if (entity == null) return null;

        // 2. Se tiver rota, tenta buscar metadados completos
        if (!string.IsNullOrEmpty(entity.Route))
        {
            var json = await GetMetadataAsync(entity.Route);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    // Deserializa o metadata (que pode estar bugado com tudo "string")
                    var metadataEntity = JsonSerializer.Deserialize<EntityManifestItem>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                    if (metadataEntity != null)
                    {
                        // =====================================================================
                        // ⭐ MERGE SEGURO: Preserva a integridade dos tipos do Manifesto
                        // =====================================================================
                        
                        // 1. Preserva dados do Módulo (que não vêm no metadata)
                        metadataEntity.ModuleName = !string.IsNullOrEmpty(metadataEntity.ModuleName) 
                            ? metadataEntity.ModuleName 
                            : entity.ModuleName;
                            
                        metadataEntity.ModuleDisplayName = !string.IsNullOrEmpty(metadataEntity.ModuleDisplayName) 
                            ? metadataEntity.ModuleDisplayName 
                            : entity.ModuleDisplayName;

                        // 2. CORREÇÃO DE TIPOS: Se o manifesto tem tipos ricos (int, bool, DateTime)
                        // e o metadata trouxe tudo como "string", PRESERVA O MANIFESTO.
                        foreach (var metaProp in metadataEntity.Properties)
                        {
                            var originalProp = entity.Properties.FirstOrDefault(p => 
                                p.Name.Equals(metaProp.Name, StringComparison.OrdinalIgnoreCase));

                            if (originalProp != null)
                            {
                                // Se metadata diz "string", mas manifesto diz algo mais específico (int, date, bool)...
                                // ... OU se metadata perdeu info de Nullable
                                if ((metaProp.Type == "string" && originalProp.Type != "string") ||
                                    (metaProp.Type == "string" && originalProp.IsNullable))
                                {
                                    _logger.LogWarning("⚠️ Corrigindo tipo da propriedade '{Prop}': Metadata='{bad}', Manifesto='{good}'", 
                                        metaProp.Name, metaProp.Type, originalProp.Type);
                                    
                                    metaProp.Type = originalProp.Type;
                                    metaProp.IsNullable = originalProp.IsNullable;
                                    metaProp.IsPrimaryKey = originalProp.IsPrimaryKey;
                                    metaProp.MaxLength = originalProp.MaxLength;
                                }
                            }
                        }

                        // 3. Garante que Navigations do manifesto sejam preservadas se o metadata não as tiver
                        if (metadataEntity.Navigations.Count == 0 && entity.Navigations.Count > 0)
                        {
                            metadataEntity.Navigations = entity.Navigations;
                        }

                        // Sucesso: Retorna o metadata enriquecido/corrigido
                        return metadataEntity;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao deserializar metadata detalhado para {Entity}. Usando manifesto original.", entityName);
                }
            }
        }

        // Fallback: Retorna manifesto original
        return entity;
    }

    // =========================================================================
    // CONVERSÕES - v3.2 Com suporte a ApiRoute do manifesto
    // =========================================================================

    /// <summary>
    /// Converte EntityManifestItem para TabelaInfo (compatível com o gerador).
    /// </summary>
    public TabelaInfo ConvertToTabelaInfo(EntityManifestItem entity)
    {
        var tabela = new TabelaInfo
        {
            Schema = entity.Schema ?? "dbo",
            NomeTabela = entity.TableName,
            Descricao = entity.DisplayName
        };

        // Converte propriedades para colunas
        int ordinal = 0;
        foreach (var prop in entity.Properties)
        {
            var coluna = new ColunaInfo
            {
                Nome = prop.ColumnName ?? prop.Name,
                Tipo = MapCSharpTypeToSql(prop.Type),
                IsNullable = prop.IsNullable,
                IsPrimaryKey = prop.IsPrimaryKey,
                IsIdentity = prop.IsIdentity,
                Tamanho = prop.MaxLength,
                Descricao = prop.DisplayName,
                OrdinalPosition = ordinal++
            };

            // Vincula FK se houver navegação correspondente
            var navigation = entity.Navigations
                .FirstOrDefault(n => n.ForeignKeyProperty?.Equals(prop.Name, StringComparison.OrdinalIgnoreCase) == true
                                  || n.ForeignKeyProperty?.Equals(prop.ColumnName, StringComparison.OrdinalIgnoreCase) == true);

            if (navigation != null && navigation.RelationType == "ManyToOne")
            {
                coluna.ForeignKey = new ForeignKeyInfo
                {
                    Nome = $"FK_{entity.TableName}_{navigation.TargetEntity}",
                    ColunaOrigem = prop.ColumnName ?? prop.Name,
                    TabelaDestino = navigation.TargetEntity,
                    ColunaDestino = "Id"
                };
            }

            tabela.Colunas.Add(coluna);
        }

        // Adiciona FKs da lista de navegações (para FKs sem propriedade correspondente)
        foreach (var nav in entity.Navigations.Where(n => n.RelationType == "ManyToOne"))
        {
            // Verifica se já não foi adicionada via coluna
            var jaExiste = tabela.ForeignKeys.Any(fk =>
                fk.ColunaOrigem.Equals(nav.ForeignKeyProperty, StringComparison.OrdinalIgnoreCase));

            if (!jaExiste && !string.IsNullOrEmpty(nav.ForeignKeyProperty))
            {
                tabela.ForeignKeys.Add(new ForeignKeyInfo
                {
                    Nome = $"FK_{entity.TableName}_{nav.TargetEntity}",
                    ColunaOrigem = nav.ForeignKeyProperty,
                    TabelaDestino = nav.TargetEntity,
                    ColunaDestino = "Id"
                });
            }
        }

        return tabela;
    }

    /// <summary>
    /// Converte EntityManifestItem para FullStackRequest.
    /// v3.2: Agora passa a ApiRoute do manifesto corretamente.
    /// </summary>
    public FullStackRequest ConvertToFullStackRequest(EntityManifestItem entity)
    {
        // Extrai módulo da rota
        var moduleRoute = ExtractModuleRoute(entity.Route);

        // Determina CdSistema baseado no módulo se não estiver definido
        var cdSistema = !string.IsNullOrEmpty(entity.CdSistema)
            ? entity.CdSistema
            : GetCdSistemaFromModule(entity.ModuleName);

        var request = new FullStackRequest
        {
            // Identificação
            NomeTabela = entity.TableName,
            DisplayName = entity.DisplayName,
            Descricao = entity.DisplayName,
            CdFuncao = entity.CdFuncao,
            CdSistema = cdSistema,

            // =========================================================================
            // v3.2 - ROTA API DO MANIFESTO
            // =========================================================================
            ApiRoute = entity.Route,

            // Módulo e Navegação
            Modulo = entity.ModuleName,
            ModuloRota = moduleRoute,
            Icone = entity.Icon ?? "fas fa-table",
            MenuOrder = 10,

            // Opções de geração (apenas frontend por default)
            GerarEntidade = false,
            GerarApiController = false,
            GerarWebController = true,
            GerarWebModels = true,
            GerarWebServices = true,
            GerarView = true,
            GerarJavaScript = true,

            // Navegação
            GerarNavigation = true,
            ApenasNavigationPorGuid = false,
            IsLegacyTable = true
        };

        // =========================================================================
        // Configurações de Colunas
        // =========================================================================
        int order = 0;
        foreach (var prop in entity.Properties)
        {
            // Listagem
            request.ColunasListagem.Add(new ColumnListConfig
            {
                Nome = prop.ColumnName ?? prop.Name,
                Visible = !prop.IsPrimaryKey && prop.Type != "Guid",
                Order = order,
                Title = FormatDisplayName(prop.DisplayName ?? prop.Name),
                Format = DetermineListFormat(prop),
                Sortable = true,
                Align = DetermineAlign(prop)
            });

            // Formulário
            request.ColunasFormulario.Add(new ColumnFormConfig
            {
                Nome = prop.ColumnName ?? prop.Name,
                Visible = !prop.IsPrimaryKey && !prop.IsIdentity,
                Order = order++,
                Label = FormatDisplayName(prop.DisplayName ?? prop.Name),
                InputType = DetermineInputType(prop),
                ColSize = DetermineColSize(prop),
                Required = prop.IsRequired && !prop.IsNullable,
                Placeholder = $"Digite {(prop.DisplayName ?? prop.Name).ToLowerInvariant()}..."
            });
        }

        // =========================================================================
        // Configurações de FK/Navegação
        // =========================================================================
        foreach (var nav in entity.Navigations.Where(n => n.RelationType == "ManyToOne"))
        {
            if (request.ConfiguracoesFk.Any(x => x.ColunaOrigem == (nav.ForeignKeyProperty ?? "")))
                continue;

            request.ConfiguracoesFk.Add(new FkNavigationConfig
            {
                ColunaOrigem = nav.ForeignKeyProperty ?? "",
                TabelaDestino = nav.TargetEntity,
                NavigationName = nav.Name,
                DisplayColumn = "Nome", // Assume padrão
                Ignorar = false
            });
        }

        // =========================================================================
        // v4.3 - Inferir FKs a partir de Lookups (se não houver Navigation explícita)
        // =========================================================================
        foreach (var prop in entity.Properties.Where(p => p.Lookup != null))
        {
            if (request.ConfiguracoesFk.Any(x => x.ColunaOrigem == prop.Name))
                continue;

            // Tenta inferir o nome da entidade a partir da rota ou módulo
            // Ex: "capfornecedores" -> "CapFornecedores"
            var targetEntity = "Unknown";
            if (!string.IsNullOrEmpty(prop.Lookup.Route))
            {
                targetEntity = prop.Lookup.Route.Replace("api/", "").Split('/').LastOrDefault() ?? "Unknown";
                targetEntity = char.ToUpper(targetEntity[0]) + targetEntity[1..]; // PascalCase-ish
            }

            // Remove sufixo Id para NavigationName
            var navName = prop.Name;
            if (navName.StartsWith("Id") && navName.Length > 2) navName = navName[2..];
            else if (navName.EndsWith("Id") && navName.Length > 2) navName = navName[..^2];

            request.ConfiguracoesFk.Add(new FkNavigationConfig
            {
                ColunaOrigem = prop.Name,
                TabelaDestino = targetEntity, // Nome aproximado, pode não ser exato
                NavigationName = navName,
                DisplayColumn = prop.Lookup.TextField,
                Ignorar = false
            });
        }

        return request;
    }

    #region Helpers

    private static string ExtractModuleRoute(string route)
    {
        // "api/gestaodeterceiros/contrato" -> "gestaodeterceiros"
        if (string.IsNullOrEmpty(route)) return "";

        var parts = route.Split('/');
        if (parts.Length >= 2)
        {
            // Pula "api/" e pega o próximo segmento
            var moduleIndex = parts[0].Equals("api", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            return parts.Length > moduleIndex ? parts[moduleIndex] : "";
        }
        return route;
    }

    private static string GetCdSistemaFromModule(string moduleName)
    {
        return moduleName?.ToUpperInvariant() switch
        {
            "IDENTITY" => "SEG",
            "GESTAODEPESSOAS" => "RHU",
            "GESTAODETERCEIROS" => "GTC",
            "CONTROLEACESSOPORTARIA" => "CAP",
            "CONTROLEDEPONTO" => "CPO",
            "SAUDEOCUPACIONAL" => "MSO",
            "TREINAMENTOS" or "TREINAMENTO" => "TRE",
            "INTEGRACOES" => "INT",
            "AVALIACOES" or "AVALIACAO" => "AVA",
            "ESOCIAL" => "ESO",
            "GESTAODEEPI" => "EPI",
            _ => "RHU"
        };
    }

    private static string MapCSharpTypeToSql(string csharpType)
    {
        var baseType = csharpType.TrimEnd('?');
        return baseType switch
        {
            "int" => "int",
            "long" => "bigint",
            "short" => "smallint",
            "byte" => "tinyint",
            "decimal" => "decimal",
            "double" => "float",
            "float" => "real",
            "bool" => "bit",
            "string" => "nvarchar",
            "DateTime" => "datetime2",
            "DateOnly" => "date",
            "TimeOnly" => "time",
            "Guid" => "uniqueidentifier",
            "byte[]" => "varbinary",
            _ => "nvarchar"
        };
    }

    private static string FormatDisplayName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        // Remove prefixos comuns
        var prefixes = new[] { "Cd", "Dc", "Dt", "Nr", "Nm", "Fl", "Vl", "Qt", "Sg", "No", "Id", "Tp", "St" };
        var workingName = name;

        foreach (var prefix in prefixes)
        {
            if (workingName.StartsWith(prefix) && workingName.Length > prefix.Length &&
                char.IsUpper(workingName[prefix.Length]))
            {
                workingName = workingName[prefix.Length..];
                break;
            }
        }

        // Adiciona espaços antes de maiúsculas
        var result = new System.Text.StringBuilder();
        for (int i = 0; i < workingName.Length; i++)
        {
            if (i > 0 && char.IsUpper(workingName[i]) && !char.IsUpper(workingName[i - 1]))
                result.Append(' ');
            result.Append(workingName[i]);
        }

        return result.ToString().Trim();
    }

    private static string DetermineListFormat(PropertyManifestItem prop)
    {
        if (prop.Type.Contains("DateTime")) return "datetime";
        if (prop.Type.Contains("DateOnly")) return "date";
        if (prop.Type is "decimal" or "decimal?") return "currency";
        if (prop.Type is "bool" or "bool?") return "boolean";
        return "text";
    }

    private static string DetermineInputType(PropertyManifestItem prop)
    {
        if (prop.Type.Contains("DateTime")) return "datetime-local";
        if (prop.Type.Contains("DateOnly")) return "date";
        if (prop.Type.Contains("TimeOnly")) return "time";
        if (prop.Type is "bool" or "bool?") return "checkbox";
        if (prop.Type is "int" or "int?" or "long" or "long?" or "short" or "short?") return "number";
        if (prop.Type is "decimal" or "decimal?" or "double" or "double?" or "float" or "float?") return "number";
        if (prop.MaxLength.HasValue && prop.MaxLength > 255) return "textarea";
        return "text";
    }

    private static string DetermineAlign(PropertyManifestItem prop)
    {
        if (prop.Type is "int" or "int?" or "long" or "long?" or "decimal" or "decimal?")
            return "right";
        if (prop.Type is "bool" or "bool?")
            return "center";
        return "left";
    }

    private static int DetermineColSize(PropertyManifestItem prop)
    {
        if (prop.Type is "bool" or "bool?") return 2;
        if (prop.Type.Contains("Date")) return 4;
        if (prop.Type is "int" or "int?" or "long" or "long?") return 3;
        if (prop.MaxLength.HasValue && prop.MaxLength < 20) return 3;
        if (prop.MaxLength.HasValue && prop.MaxLength > 255) return 12;
        return 6;
    }

    #endregion
}


