// =============================================================================
// RHSENSOERP CRUD TOOL - CLI ENTRY POINT
// Versão: 2.0
// =============================================================================
using System.Text.Json;
using RhSensoERP.CrudTool.Generators;
using RhSensoERP.CrudTool.Models;
using Spectre.Console;

namespace RhSensoERP.CrudTool;

class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        AnsiConsole.Write(new FigletText("CRUD Tool").Color(Color.Cyan1));
        AnsiConsole.MarkupLine("[grey]RhSensoERP CRUD Generator v2.0 - Frontend Generator[/]");
        AnsiConsole.MarkupLine("[grey]Compatível com Backend Source Generator[/]");
        AnsiConsole.WriteLine();

        try
        {
            // Determina o arquivo de configuração
            var configFile = args.Length > 0 ? args[0] : "crud-config.json";

            if (!File.Exists(configFile))
            {
                AnsiConsole.MarkupLine($"[red]✗ Arquivo de configuração não encontrado: {configFile}[/]");
                AnsiConsole.MarkupLine("[yellow]Crie um arquivo crud-config.json ou especifique o caminho como argumento.[/]");
                return 1;
            }

            AnsiConsole.MarkupLine($"[blue]📄 Lendo configuração: {configFile}[/]");

            // Lê e parseia o JSON
            var json = await File.ReadAllTextAsync(configFile);
            var config = JsonSerializer.Deserialize<CrudConfig>(json, JsonOptions);

            if (config == null || config.Entities.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]✗ Configuração inválida ou sem entities.[/]");
                return 1;
            }

            // Valida paths
            if (!Directory.Exists(config.SolutionRoot))
            {
                AnsiConsole.MarkupLine($"[red]✗ Diretório da solution não encontrado: {config.SolutionRoot}[/]");
                return 1;
            }

            AnsiConsole.MarkupLine($"[green]✓ Configuração válida - {config.Entities.Count} entity(s)[/]");
            AnsiConsole.WriteLine();

            // Lista entities a serem geradas
            var table = new Table();
            table.AddColumn("Entity");
            table.AddColumn("Module");
            table.AddColumn("Controller");
            table.AddColumn("Models");
            table.AddColumn("Services");
            table.AddColumn("View");
            table.AddColumn("JS");

            foreach (var entity in config.Entities)
            {
                table.AddRow(
                    entity.Name,
                    entity.Module,
                    entity.Generate.WebController ? "[green]✓[/]" : "[grey]✗[/]",
                    entity.Generate.WebModels ? "[green]✓[/]" : "[grey]✗[/]",
                    entity.Generate.WebServices ? "[green]✓[/]" : "[grey]✗[/]",
                    entity.Generate.View ? "[green]✓[/]" : "[grey]✗[/]",
                    entity.Generate.JavaScript ? "[green]✓[/]" : "[grey]✗[/]"
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            // Confirmação
            if (!AnsiConsole.Confirm("Gerar arquivos?"))
            {
                AnsiConsole.MarkupLine("[yellow]Operação cancelada.[/]");
                return 0;
            }

            // Gera os arquivos
            var generator = new CrudGenerator(config);

            await AnsiConsole.Progress()
                .StartAsync(async ctx =>
                {
                    var task = ctx.AddTask("[green]Gerando arquivos...[/]");
                    task.MaxValue = config.Entities.Count;

                    foreach (var entity in config.Entities)
                    {
                        task.Description = $"[green]Gerando {entity.Name}...[/]";
                        await generator.GenerateAsync(entity);
                        task.Increment(1);
                    }
                });

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]✓ Geração concluída com sucesso![/]");
            AnsiConsole.WriteLine();

            // Mostra próximos passos
            var panel = new Panel(
                "[yellow]Próximos passos:[/]\n" +
                "1. Registre o Service no DI (Program.cs ou ServiceCollectionExtensions.cs)\n" +
                "2. Adicione a rota no menu de navegação\n" +
                "3. Teste a funcionalidade"
            );
            panel.Header = new PanelHeader("[blue]📋 TODO[/]");
            AnsiConsole.Write(panel);

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }
}
