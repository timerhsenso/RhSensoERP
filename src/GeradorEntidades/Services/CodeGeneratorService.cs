// =============================================================================
// GERADOR DE ENTIDADES - CODE GENERATOR SERVICE v2.0
// Com suporte a Navigation Properties e Relacionamentos
// =============================================================================

using System.Text;
using System.Text.Json;
using GeradorEntidades.Models;

namespace GeradorEntidades.Services;

/// <summary>
/// Serviço de geração de código C# e JSON com suporte a navegação.
/// </summary>
public class CodeGeneratorService
{
    private readonly ILogger<CodeGeneratorService> _logger;

    public CodeGeneratorService(ILogger<CodeGeneratorService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gera o código completo (Entidade + JSON).
    /// </summary>
    public GeracaoResult Gerar(TabelaInfo tabela, GeracaoRequest request)
    {
        try
        {
            _logger.LogInformation("Gerando código para tabela {Tabela}", tabela.NomeTabela);

            var nomeEntidade = tabela.NomePascalCase;
            
            var displayName = !string.IsNullOrWhiteSpace(request.DisplayName) 
                ? request.DisplayName 
                : tabela.NomePascalCase;

            var navigationsGeradas = new List<string>();
            var codigoEntidade = GerarEntidade(tabela, request, nomeEntidade, navigationsGeradas);
            var jsonConfig = GerarJsonConfig(tabela, request, nomeEntidade, displayName);

            return new GeracaoResult
            {
                Success = true,
                NomeTabela = tabela.NomeTabela,
                NomeEntidade = nomeEntidade,
                CodigoEntidade = codigoEntidade,
                JsonConfig = jsonConfig,
                NavigationsGeradas = navigationsGeradas
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar código para {Tabela}", tabela.NomeTabela);
            return new GeracaoResult
            {
                Success = false,
                Error = ex.Message,
                NomeTabela = tabela.NomeTabela
            };
        }
    }

    /// <summary>
    /// Gera a entidade C# com Navigation Properties.
    /// </summary>
    private string GerarEntidade(TabelaInfo tabela, GeracaoRequest request, string nomeEntidade, List<string> navigationsGeradas)
    {
        var sb = new StringBuilder();
        var modulo = ModuloConfig.GetModulos()
            .FirstOrDefault(m => m.Nome.Equals(request.Modulo, StringComparison.OrdinalIgnoreCase))
            ?? ModuloConfig.GetModulos().First();

        // Usings
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
        sb.AppendLine("using RhSensoERP.Shared.Core.Attributes;");
        sb.AppendLine();

        // Namespace
        sb.AppendLine($"namespace {modulo.Namespace}.Core.Entities;");
        sb.AppendLine();

        // Documentação XML
        if (!string.IsNullOrWhiteSpace(tabela.Descricao))
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// {tabela.Descricao}");
            sb.AppendLine("/// </summary>");
        }

        // Atributo GenerateCrud
        sb.AppendLine("[GenerateCrud(");
        sb.AppendLine($"    TableName = \"{tabela.NomeTabela.ToLower()}\",");
        sb.AppendLine($"    DisplayName = \"{request.DisplayName ?? nomeEntidade}\",");
        sb.AppendLine($"    CdSistema = \"{request.CdSistema}\",");
        sb.AppendLine($"    CdFuncao = \"{request.CdFuncao}\",");
        sb.AppendLine($"    IsLegacyTable = {request.IsLegacyTable.ToString().ToLower()},");
        sb.AppendLine($"    GenerateApiController = {request.GerarApiController.ToString().ToLower()}");
        sb.AppendLine(")]");

        // Classe
        sb.AppendLine($"public class {nomeEntidade}");
        sb.AppendLine("{");

        // =========================================================================
        // SEÇÃO 1: PROPRIEDADES ESCALARES (colunas normais)
        // =========================================================================
        sb.AppendLine("    #region Propriedades");
        sb.AppendLine();
        
        foreach (var coluna in tabela.Colunas)
        {
            GerarPropriedade(sb, coluna, tabela.PrimaryKey);
        }
        
        sb.AppendLine("    #endregion");

        // =========================================================================
        // SEÇÃO 2: NAVIGATION PROPERTIES (relacionamentos)
        // =========================================================================
        if (request.GerarNavigation && tabela.ForeignKeys.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("    #region Navigation Properties");
            sb.AppendLine();
            
            // PASSO 1: Elimina FKs duplicadas (mesma coluna origem + mesma tabela destino)
            // Isso acontece quando o banco tem constraints duplicadas (FK_xxx e fk_xxx)
            var fksUnicas = tabela.ForeignKeys
                .GroupBy(fk => fk.ChaveUnica)
                .Select(g => g.First())
                .ToList();

            // PASSO 2: IGNORA FKs que fazem parte de FK composta
            // Uma FK composta (ex: cdbanrec + cdagerec -> tage1) não deve gerar
            // navegações individuais para cada coluna, pois EF Core não suporta
            var fksSimplesOuPrimeiraDaComposta = fksUnicas
                .Where(fk => !fk.IsParteDeFkComposta)
                .ToList();

            // PASSO 3: Garante que cada coluna só gere UMA navegação
            // Se cdbanrec aponta para tban1 E faz parte de FK composta para tage1,
            // só mantemos a FK simples para tban1
            var colunasJaProcessadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var fksFiltradas = new List<ForeignKeyInfo>();
            
            foreach (var fk in fksSimplesOuPrimeiraDaComposta)
            {
                if (!colunasJaProcessadas.Contains(fk.ColunaOrigem))
                {
                    fksFiltradas.Add(fk);
                    colunasJaProcessadas.Add(fk.ColunaOrigem);
                }
            }

            // PASSO 4: Ordenar para processar FKs por Guid PRIMEIRO
            // Assim elas "reservam" os nomes bons (Cargo, Situacao, etc)
            // e as FKs por código ficam com sufixo (CargoPorCodigo, SituacaoPorCodigo)
            var fksOrdenadas = fksFiltradas
                .OrderByDescending(fk => fk.IsFkByGuid)  // Guid primeiro
                .ThenBy(fk => fk.ColunaOrigem)           // Depois por nome
                .ToList();

            var navigationNamesUsados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            // Mapeia: TabelaDestino -> nome já usado para ela (para detectar duplicatas)
            var tabelaParaNavigation = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var fk in fksOrdenadas)
            {
                // Se só quer FKs por Guid, pula as por código
                if (request.ApenasNavigationPorGuid && !fk.IsFkByGuid)
                    continue;

                var navigationName = GerarNomeNavigation(fk, navigationNamesUsados, tabelaParaNavigation);
                
                navigationNamesUsados.Add(navigationName);
                
                // Registra que esta tabela destino já tem uma navegação
                if (!tabelaParaNavigation.ContainsKey(fk.TabelaDestino))
                    tabelaParaNavigation[fk.TabelaDestino] = navigationName;

                // Encontra a coluna correspondente
                var coluna = tabela.Colunas.FirstOrDefault(c => 
                    c.Nome.Equals(fk.ColunaOrigem, StringComparison.OrdinalIgnoreCase));
                
                var isNullable = coluna?.IsNullable ?? true;
                var nullableMarker = isNullable ? "?" : "";

                // Comentário XML
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// Navegação para {fk.EntidadeDestino} via {fk.ColunaOrigem}");
                sb.AppendLine($"    /// </summary>");
                
                // Atributo ForeignKey
                var colunaPascal = coluna?.NomePascalCase ?? TabelaInfo.ToPascalCase(fk.ColunaOrigem);
                sb.AppendLine($"    [ForeignKey(nameof({colunaPascal}))]");
                
                // Propriedade virtual
                sb.AppendLine($"    public virtual {fk.EntidadeDestino}{nullableMarker} {navigationName} {{ get; set; }}");
                sb.AppendLine();
                
                navigationsGeradas.Add($"{navigationName} -> {fk.EntidadeDestino}");
            }
            
            sb.AppendLine("    #endregion");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// Gera uma propriedade da entidade.
    /// </summary>
    private void GerarPropriedade(StringBuilder sb, ColunaInfo coluna, ColunaInfo? pk)
    {
        var isPk = pk != null && coluna.Nome.Equals(pk.Nome, StringComparison.OrdinalIgnoreCase);
        var nomeProp = coluna.NomePascalCase;

        // Documentação XML
        if (!string.IsNullOrWhiteSpace(coluna.Descricao))
        {
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// {coluna.Descricao}");
            sb.AppendLine($"    /// </summary>");
        }

        // [Key] para PK
        if (isPk)
        {
            sb.AppendLine("    [Key]");
        }

        // [Required] para não-nullable
        if (!coluna.IsNullable && !coluna.IsIdentity && !coluna.IsComputed)
        {
            sb.AppendLine("    [Required]");
        }

        // [Column] para mapear nome da coluna
        sb.AppendLine($"    [Column(\"{coluna.Nome.ToLower()}\")]");

        // [StringLength] para tipos texto
        if (coluna.IsTexto && coluna.Tamanho.HasValue && coluna.Tamanho.Value > 0 && coluna.Tamanho.Value < int.MaxValue)
        {
            sb.AppendLine($"    [StringLength({coluna.Tamanho.Value})]");
        }

        // [FieldDisplayName] para label amigável
        var displayName = !string.IsNullOrWhiteSpace(coluna.Descricao) 
            ? coluna.Descricao 
            : FormatDisplayName(nomeProp);
        sb.AppendLine($"    [FieldDisplayName(\"{displayName}\")]");

        // [DatabaseGenerated] para Identity
        if (coluna.IsIdentity)
        {
            sb.AppendLine("    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
        }
        else if (coluna.IsComputed)
        {
            sb.AppendLine("    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]");
        }

        // Propriedade
        var tipoCsharp = coluna.TipoCSharp;
        var defaultValue = tipoCsharp == "string" ? " = string.Empty;" : "";
        
        sb.AppendLine($"    public {tipoCsharp} {nomeProp} {{ get; set; }}{defaultValue}");
        sb.AppendLine();
    }

    /// <summary>
    /// Gera nome único para Navigation Property com lógica inteligente.
    /// FKs por Guid têm prioridade para nomes limpos.
    /// </summary>
    private static string GerarNomeNavigation(
        ForeignKeyInfo fk, 
        HashSet<string> usados, 
        Dictionary<string, string> tabelaParaNav)
    {
        var colunaLower = fk.ColunaOrigem.ToLower();
        var nomeBase = fk.NavigationPropertyName;
        
        // Se ainda não existe navegação com este nome, usa o nome base
        if (!usados.Contains(nomeBase))
            return nomeBase;

        // Já existe - precisa de sufixo contextual
        string nomeComSufixo;
        
        // Primeiro tenta sufixos contextuais baseados no nome da coluna
        if (colunaLower.Contains("naturalidade"))
            nomeComSufixo = nomeBase + "Naturalidade";
        else if (colunaLower.Contains("endereco"))
            nomeComSufixo = nomeBase + "Endereco";
        else if (colunaLower.Contains("recebimento"))
            nomeComSufixo = nomeBase + "Recebimento";
        else if (colunaLower.EndsWith("rec") && !colunaLower.Contains("recebimento"))
            nomeComSufixo = nomeBase + "Recebimento";
        else if (colunaLower.Contains("fgts"))
            nomeComSufixo = nomeBase + "Fgts";
        else if (fk.IsFkByCodigo)
            // FK por código legado recebe sufixo "PorCodigo"
            nomeComSufixo = nomeBase + "PorCodigo";
        else
            // Fallback: usa a coluna como sufixo
            nomeComSufixo = nomeBase + "_" + TabelaInfo.ToPascalCase(fk.ColunaOrigem);
        
        // Garante unicidade final
        var nomeFinal = nomeComSufixo;
        var seq = 2;
        while (usados.Contains(nomeFinal))
        {
            nomeFinal = $"{nomeComSufixo}{seq++}";
        }
        
        return nomeFinal;
    }

    /// <summary>
    /// Gera o JSON de configuração do CrudTool com informações de navegação.
    /// </summary>
    private string GerarJsonConfig(TabelaInfo tabela, GeracaoRequest request, string nomeEntidade, string displayName)
    {
        var pk = tabela.PrimaryKey ?? tabela.Colunas.First();
        var modulo = ModuloConfig.GetModulos()
            .FirstOrDefault(m => m.Nome.Equals(request.Modulo, StringComparison.OrdinalIgnoreCase))
            ?? ModuloConfig.GetModulos().First();

        var nomePlural = tabela.NomePlural;
        var order = 0;

        var colunasListagem = request.ColunasListagem.Count > 0
            ? tabela.Colunas.Where(c => request.ColunasListagem.Contains(c.Nome)).ToList()
            : tabela.Colunas.Take(5).ToList();

        var colunasFormulario = request.ColunasFormulario.Count > 0
            ? tabela.Colunas.Where(c => request.ColunasFormulario.Contains(c.Nome)).ToList()
            : tabela.Colunas.Where(c => !c.IsComputed && !c.IsIdentity).ToList();

        // Cria dicionário de navigation names para evitar duplicatas
        var navigationNames = GerarNavigationNames(tabela);

        var config = new
        {
            schema = "./crud-schema.json",
            solutionRoot = ".",
            apiProject = "src/RhSensoERP.API",
            webProject = "src/Web",
            entities = new[]
            {
                new
                {
                    name = nomeEntidade,
                    displayName = displayName,
                    pluralName = nomePlural,
                    module = modulo.Nome,
                    moduleRoute = modulo.Rota,
                    backendNamespace = $"{modulo.Namespace}.Application.DTOs.{nomePlural}",
                    tableName = tabela.NomeTabela.ToLower(),
                    cdSistema = request.CdSistema,
                    cdFuncao = request.CdFuncao,
                    
                    menu = new
                    {
                        icon = request.Icone,
                        order = request.MenuOrder,
                        hidden = !request.GerarMenuItem
                    },
                    
                    primaryKey = new
                    {
                        property = pk.NomePascalCase,
                        type = pk.TipoJson,
                        column = pk.Nome.ToLower()
                    },
                    
                    // Pré-calcula conjunto de colunas que fazem parte de FK válida (não composta)
                    properties = tabela.Colunas.Select(c => 
                    {
                        order++;
                        var isPk = c.Nome.Equals(pk.Nome, StringComparison.OrdinalIgnoreCase);
                        var showInList = colunasListagem.Any(cl => cl.Nome == c.Nome);
                        var showInForm = colunasFormulario.Any(cf => cf.Nome == c.Nome);
                        
                        // Informações de FK e navegação
                        // IMPORTANTE: Ignora FKs que fazem parte de FK composta
                        object? foreignKeyInfo = null;
                        if (c.ForeignKey != null && !c.ForeignKey.IsParteDeFkComposta)
                        {
                            var navName = navigationNames.GetValueOrDefault(c.Nome.ToLower(), c.ForeignKey.NavigationPropertyName);
                            foreignKeyInfo = new
                            {
                                table = c.ForeignKey.TabelaDestino,
                                column = c.ForeignKey.ColunaDestino,
                                // NOVOS CAMPOS para navegação
                                navigationProperty = navName,
                                relatedEntity = c.ForeignKey.EntidadeDestino,
                                displayColumn = c.ForeignKey.ColunaDisplay ?? InferirColunaDisplay(c.ForeignKey.TabelaDestino),
                                isByGuid = c.ForeignKey.IsFkByGuid,
                                isByCodigo = c.ForeignKey.IsFkByCodigo
                            };
                        }
                        
                        return new
                        {
                            name = c.NomePascalCase,
                            type = c.TipoJson,
                            column = c.Nome.ToLower(),
                            displayName = !string.IsNullOrWhiteSpace(c.Descricao) 
                                ? c.Descricao 
                                : FormatDisplayName(c.NomePascalCase),
                            maxLength = c.Tamanho,
                            required = !c.IsNullable && !c.IsIdentity,
                            isPrimaryKey = isPk,
                            isIdentity = c.IsIdentity,
                            isComputed = c.IsComputed,
                            isGuid = c.IsGuid,
                            foreignKey = foreignKeyInfo,
                            list = new
                            {
                                show = showInList,
                                order = order,
                                sortable = true,
                                align = c.IsNumerico ? "right" : "left",
                                format = GetColumnFormat(c),
                                width = isPk ? "100px" : (c.IsData ? "150px" : (object?)null)
                            },
                            form = new
                            {
                                show = showInForm && !c.IsComputed,
                                showOnCreate = showInForm && !c.IsIdentity && !c.IsComputed,
                                showOnEdit = showInForm && !isPk && !c.IsIdentity && !c.IsComputed,
                                order = order,
                                inputType = GetInputType(c),
                                placeholder = GetPlaceholder(c),
                                helpText = c.Descricao ?? $"Informe {FormatDisplayName(c.NomePascalCase).ToLower()}",
                                colSize = isPk ? 3 : (c.IsTexto && c.Tamanho > 100 ? 12 : 6)
                            }
                        };
                    }).ToArray(),
                    
                    // SEÇÃO: Relacionamentos/Includes para queries
                    // Usa mesma lógica de filtro que a geração de entidade
                    relationships = FiltrarFksParaJson(tabela)
                        .Select(fk => 
                        {
                            var navName = navigationNames.GetValueOrDefault(fk.ColunaOrigem.ToLower(), fk.NavigationPropertyName);
                            return new
                            {
                                property = navName,
                                relatedEntity = fk.EntidadeDestino,
                                foreignKeyColumn = fk.ColunaOrigem,
                                referencedTable = fk.TabelaDestino,
                                referencedColumn = fk.ColunaDestino,
                                displayColumn = fk.ColunaDisplay ?? InferirColunaDisplay(fk.TabelaDestino),
                                isByGuid = fk.IsFkByGuid
                            };
                        }).ToArray(),
                    
                    generate = new
                    {
                        apiController = request.GerarApiController,
                        webController = request.GerarWebController,
                        webModels = true,
                        webServices = true,
                        view = true,
                        javascript = true
                    }
                }
            }
        };

        return JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
    }

    /// <summary>
    /// Filtra FKs para uso no JSON, usando mesma lógica da geração de entidade.
    /// Remove FKs duplicadas, FKs compostas e garante uma navegação por coluna.
    /// </summary>
    private static List<ForeignKeyInfo> FiltrarFksParaJson(TabelaInfo tabela)
    {
        // PASSO 1: Elimina FKs duplicadas (mesma coluna + mesma tabela destino)
        var fksUnicas = tabela.ForeignKeys
            .GroupBy(fk => fk.ChaveUnica)
            .Select(g => g.First())
            .ToList();

        // PASSO 2: Ignora FKs compostas
        var fksSimplesOuPrimeiraDaComposta = fksUnicas
            .Where(fk => !fk.IsParteDeFkComposta)
            .ToList();

        // PASSO 3: Garante uma navegação por coluna
        var colunasJaProcessadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var fksFiltradas = new List<ForeignKeyInfo>();
        
        foreach (var fk in fksSimplesOuPrimeiraDaComposta)
        {
            if (!colunasJaProcessadas.Contains(fk.ColunaOrigem))
            {
                fksFiltradas.Add(fk);
                colunasJaProcessadas.Add(fk.ColunaOrigem);
            }
        }

        // PASSO 4: Ordenar FKs por Guid primeiro
        return fksFiltradas
            .OrderByDescending(fk => fk.IsFkByGuid)
            .ThenBy(fk => fk.ColunaOrigem)
            .ToList();
    }

    /// <summary>
    /// Gera dicionário de nomes de navegação únicos, eliminando FKs duplicadas.
    /// Usa mesma lógica que a geração de entidade para consistência.
    /// </summary>
    private Dictionary<string, string> GerarNavigationNames(TabelaInfo tabela)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var usados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var tabelaParaNav = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Usa mesmo filtro que o JSON e a entidade
        var fksFiltradas = FiltrarFksParaJson(tabela);

        foreach (var fk in fksFiltradas)
        {
            var navName = GerarNomeNavigation(fk, usados, tabelaParaNav);
            
            usados.Add(navName);
            if (!tabelaParaNav.ContainsKey(fk.TabelaDestino))
                tabelaParaNav[fk.TabelaDestino] = navName;
                
            result[fk.ColunaOrigem.ToLower()] = navName;
        }

        return result;
    }

    /// <summary>
    /// Infere a coluna de display baseado em padrões comuns.
    /// </summary>
    private static string InferirColunaDisplay(string nomeTabela)
    {
        var tabela = nomeTabela.ToLower();
        
        // Padrões comuns de colunas de descrição
        // Prioridade: dc*, nm*, descricao, nome, dc + nometabela
        
        // Remove prefixos da tabela para montar nome da coluna
        var nomeBase = tabela.Replace("tbl_", "").Replace("tb_", "").Replace("t_", "");
        
        // Padrões mais comuns
        var patterns = new[]
        {
            $"dc{nomeBase}",      // dccargo, dcsituacao
            $"nm{nomeBase}",      // nmcolab
            "descricao",
            "nome",
            "dc_descricao",
            "nm_nome"
        };
        
        // Retorna o primeiro padrão (o mais provável)
        return patterns[0];
    }

    #region Helpers

    private static string FormatDisplayName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var sb = new StringBuilder();
        
        for (int i = 0; i < name.Length; i++)
        {
            var c = name[i];
            
            if (i > 0 && char.IsUpper(c) && !char.IsUpper(name[i - 1]))
            {
                sb.Append(' ');
            }
            
            sb.Append(c);
        }

        var result = sb.ToString()
            .Replace("Cd ", "Código ")
            .Replace("Dc ", "Descrição ")
            .Replace("Dt ", "Data ")
            .Replace("Nr ", "Número ")
            .Replace("Nm ", "Nome ")
            .Replace("Fl ", "Flag ")
            .Replace("Vl ", "Valor ")
            .Replace("Qt ", "Quantidade ")
            .Replace("Sg ", "Sigla ")
            .Replace("No ", "Número ")
            .Replace("Id ", "ID ");

        return result.Trim();
    }

    private static string GetColumnFormat(ColunaInfo coluna)
    {
        if (coluna.IsData) return "date";
        if (coluna.Tipo.ToLower() is "decimal" or "money" or "numeric") return "currency";
        if (coluna.Tipo.ToLower() is "float" or "real") return "decimal";
        if (coluna.Tipo.ToLower() == "bit") return "boolean";
        return "text";
    }

    private static string GetInputType(ColunaInfo coluna)
    {
        if (coluna.IsData) return coluna.Tipo.ToLower() == "date" ? "date" : "datetime-local";
        if (coluna.Tipo.ToLower() == "bit") return "checkbox";
        if (coluna.IsNumerico) return "number";
        if (coluna.IsTexto && coluna.Tamanho > 255) return "textarea";
        if (coluna.ForeignKey != null) return "select";
        if (coluna.IsGuid && coluna.ForeignKey != null) return "select";
        return "text";
    }

    private static string GetPlaceholder(ColunaInfo coluna)
    {
        var displayName = FormatDisplayName(coluna.NomePascalCase);
        
        if (coluna.IsData) return "dd/mm/aaaa";
        if (coluna.IsNumerico) return "0";
        if (coluna.ForeignKey != null) return "Selecione...";
        
        return $"Digite {displayName.ToLower()}";
    }

    #endregion
}
