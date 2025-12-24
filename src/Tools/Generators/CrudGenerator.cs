// =============================================================================
// RHSENSOERP CRUD TOOL - MAIN GENERATOR
// Versão: 2.0
// =============================================================================
using RhSensoERP.CrudTool.Models;
using RhSensoERP.CrudTool.Templates;
using Spectre.Console;

namespace RhSensoERP.CrudTool.Generators;

/// <summary>
/// Gerador principal de CRUD Frontend.
/// Gera arquivos compatíveis com a estrutura existente do projeto Web.
/// </summary>
public class CrudGenerator
{
    private readonly CrudConfig _config;
    private readonly string _solutionRoot;
    private readonly string _webPath;

    public CrudGenerator(CrudConfig config)
    {
        _config = config;
        _solutionRoot = Path.GetFullPath(config.SolutionRoot);
        _webPath = Path.Combine(_solutionRoot, config.WebProject);
    }

    /// <summary>
    /// Gera todos os arquivos para uma Entity.
    /// </summary>
    public async Task GenerateAsync(EntityConfig entity)
    {
        AnsiConsole.MarkupLine($"\n[bold cyan]Gerando arquivos para {entity.Name}...[/]");

        // Web Models (DTOs para o frontend)
        if (entity.Generate.WebModels)
        {
            AnsiConsole.MarkupLine("  [yellow]→[/] Gerando Models...");
            
            var modelsPath = Path.Combine(_webPath, "Models", entity.PluralName);
            
            // DTO de leitura
            await WriteFileAsync(
                Path.Combine(modelsPath, $"{entity.Name}Dto.cs"),
                WebModelsTemplate.GenerateDto(entity));
            
            // CreateRequest
            await WriteFileAsync(
                Path.Combine(modelsPath, $"Create{entity.Name}Request.cs"),
                WebModelsTemplate.GenerateCreateRequest(entity));
            
            // UpdateRequest
            await WriteFileAsync(
                Path.Combine(modelsPath, $"Update{entity.Name}Request.cs"),
                WebModelsTemplate.GenerateUpdateRequest(entity));
            
            // ListViewModel
            await WriteFileAsync(
                Path.Combine(modelsPath, $"{entity.PluralName}ListViewModel.cs"),
                WebModelsTemplate.GenerateListViewModel(entity));
        }

        // Web Services
        if (entity.Generate.WebServices)
        {
            AnsiConsole.MarkupLine("  [yellow]→[/] Gerando Services...");
            
            var servicesPath = Path.Combine(_webPath, "Services", entity.PluralName);
            
            // Interface
            await WriteFileAsync(
                Path.Combine(servicesPath, $"I{entity.Name}ApiService.cs"),
                WebServicesTemplate.GenerateInterface(entity));
            
            // Implementation
            await WriteFileAsync(
                Path.Combine(servicesPath, $"{entity.Name}ApiService.cs"),
                WebServicesTemplate.GenerateImplementation(entity));
        }

        // Web Controller
        if (entity.Generate.WebController)
        {
            AnsiConsole.MarkupLine("  [yellow]→[/] Gerando Controller...");
            
            var webControllerPath = Path.Combine(
                _webPath, 
                "Controllers", 
                $"{entity.PluralName}Controller.cs");
            
            var content = WebControllerTemplate.Generate(entity);
            await WriteFileAsync(webControllerPath, content);
        }

        // View
        if (entity.Generate.View)
        {
            AnsiConsole.MarkupLine("  [yellow]→[/] Gerando View...");
            
            var viewsPath = Path.Combine(_webPath, "Views", entity.PluralName);
            
            await WriteFileAsync(
                Path.Combine(viewsPath, "Index.cshtml"),
                ViewTemplate.GenerateIndex(entity));
        }

        // JavaScript
        if (entity.Generate.JavaScript)
        {
            AnsiConsole.MarkupLine("  [yellow]→[/] Gerando JavaScript...");
            
            var jsPath = Path.Combine(_webPath, "wwwroot", "js", entity.PluralNameLower);
            
            await WriteFileAsync(
                Path.Combine(jsPath, $"{entity.NameLower}.js"),
                JavaScriptTemplate.Generate(entity));
        }

        AnsiConsole.MarkupLine($"[bold green]✓ {entity.Name} gerado com sucesso![/]");
    }

    /// <summary>
    /// Gera todos os arquivos para todas as entities configuradas.
    /// </summary>
    public async Task GenerateAllAsync()
    {
        AnsiConsole.MarkupLine($"\n[bold blue]═══════════════════════════════════════════════════════════[/]");
        AnsiConsole.MarkupLine($"[bold blue]  RHSENSOERP CRUD TOOL - GERADOR DE CÓDIGO FRONTEND[/]");
        AnsiConsole.MarkupLine($"[bold blue]═══════════════════════════════════════════════════════════[/]");

        AnsiConsole.MarkupLine($"\n[bold]Configuração:[/]");
        AnsiConsole.MarkupLine($"  Solution Root: [cyan]{_solutionRoot}[/]");
        AnsiConsole.MarkupLine($"  Web Project: [cyan]{_webPath}[/]");
        AnsiConsole.MarkupLine($"  Entities: [cyan]{_config.Entities.Count}[/]");

        foreach (var entity in _config.Entities)
        {
            await GenerateAsync(entity);
        }

        AnsiConsole.MarkupLine($"\n[bold green]═══════════════════════════════════════════════════════════[/]");
        AnsiConsole.MarkupLine($"[bold green]  ✓ GERAÇÃO CONCLUÍDA COM SUCESSO![/]");
        AnsiConsole.MarkupLine($"[bold green]═══════════════════════════════════════════════════════════[/]\n");
    }

    /// <summary>
    /// Escreve arquivo criando diretórios se necessário.
    /// </summary>
    private static async Task WriteFileAsync(string path, string content)
    {
        var dir = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        await File.WriteAllTextAsync(path, content, System.Text.Encoding.UTF8);
        
        var relativePath = Path.GetRelativePath(Directory.GetCurrentDirectory(), path);
        AnsiConsole.MarkupLine($"    [green]✓[/] {relativePath}");
    }
}
