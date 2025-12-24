// =============================================================================
// RHSENSOERP WEB - MENU ITEM ATTRIBUTE
// =============================================================================
// Arquivo: src/Web/Attributes/MenuItemAttribute.cs
// Descrição: Atributo para configurar aparição automática no menu
// =============================================================================

namespace RhSensoERP.Web.Attributes;

/// <summary>
/// Marca um Controller para aparecer automaticamente no menu.
/// Usado pelo MenuDiscoveryService para montar o menu dinâmico.
/// </summary>
/// <example>
/// <code>
/// [MenuItem(
///     Module = MenuModule.GestaoDePessoas,
///     DisplayName = "Tipos de Tabela",
///     Icon = "fas fa-table",
///     Order = 10,
///     CdFuncao = "RHU_FM_TAUX1",
///     Area = "RHU"
/// )]
/// public class Tabtaux1sController : BaseCrudController { }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class MenuItemAttribute : Attribute
{
    /// <summary>
    /// Módulo ao qual este item pertence.
    /// Determina em qual dropdown do menu o item aparecerá.
    /// </summary>
    public MenuModule Module { get; set; } = MenuModule.eSocial;

    /// <summary>
    /// Nome de exibição no menu.
    /// Se não informado, usa o nome do Controller sem o sufixo "Controller".
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Ícone FontAwesome (ex: "fas fa-users", "fas fa-table").
    /// </summary>
    public string Icon { get; set; } = "fas fa-circle";

    /// <summary>
    /// Ordem de exibição dentro do módulo (menor = primeiro).
    /// </summary>
    public int Order { get; set; } = 100;

    /// <summary>
    /// Código da função no sistema de permissões (ex: "RHU_FM_TAUX1").
    /// Usado para verificar se o usuário tem acesso.
    /// Se não informado, o item aparece para todos os usuários autenticados.
    /// </summary>
    public string? CdFuncao { get; set; }

    /// <summary>
    /// Código do sistema (ex: "RHU", "SEG").
    /// </summary>
    public string? CdSistema { get; set; }

    /// <summary>
    /// Nome da área MVC (ex: "SEG", "RHU").
    /// Se não informado aqui, o sistema tenta descobrir via [Area] no Controller.
    /// </summary>
    public string? Area { get; set; }

    /// <summary>
    /// Se true, o item NÃO aparece no menu (útil para telas auxiliares).
    /// </summary>
    public bool Hidden { get; set; } = false;

    /// <summary>
    /// Se true, mostra badge "Em breve" e desabilita o link.
    /// </summary>
    public bool ComingSoon { get; set; } = false;

    /// <summary>
    /// Texto do badge (ex: "Novo", "Beta", "Em breve").
    /// </summary>
    public string? Badge { get; set; }

    /// <summary>
    /// Cor do badge (ex: "success", "warning", "info", "danger").
    /// </summary>
    public string BadgeColor { get; set; } = "info";

    /// <summary>
    /// Descrição/tooltip do item.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Action padrão (default: "Index").
    /// </summary>
    public string Action { get; set; } = "Index";

    /// <summary>
    /// Parâmetros de rota adicionais (string livre, ex: "id=1&tipo=X").
    /// </summary>
    public string? RouteValues { get; set; }

    /// <summary>
    /// Permite fixar uma chave estável manualmente, se precisar.
    /// </summary>
    public string? ScreenKey { get; set; }
}

/// <summary>
/// Módulos disponíveis no sistema.
/// Cada módulo corresponde a um dropdown no menu.
/// </summary>
public enum MenuModule
{
    /*
    /// <summary>
    /// Módulo de Segurança (SEG) - Usuários, Permissões, etc.
    /// </summary>
    [MenuModuleInfo("Segurança", "fas fa-shield-alt", 1, "SEG")]
    Seguranca,

    /// <summary>
    /// Módulo de Gestão de Pessoas (RHU) - Funcionários, Tabelas, etc.
    /// </summary>
    [MenuModuleInfo("Gestão de Pessoas", "fas fa-users", 2, "RHU")]
    GestaoDePessoas,

    /// <summary>
    /// Módulo de Controle de Ponto (CPO) - Marcações, Escalas, etc.
    /// </summary>
    [MenuModuleInfo("Controle de Ponto", "fas fa-clock", 3, "CPO")]
    ControleDePonto,

    /// <summary>
    /// Módulo de Cadastros Gerais.
    /// </summary>
    [MenuModuleInfo("Cadastros", "fas fa-folder", 4, "CAD")]
    Cadastros,

    /// <summary>
    /// Módulo de Relatórios.
    /// </summary>
    [MenuModuleInfo("Relatórios", "fas fa-chart-bar", 5, "REL")]
    Relatorios,

    /// <summary>
    /// Módulo de Configurações.
    /// </summary>
    [MenuModuleInfo("Configurações", "fas fa-cog", 6, "CFG")]
    Configuracoes,

    /// <summary>
    /// Módulo de Configurações.
    /// </summary>
    [MenuModuleInfo("Configurações", "fas fa-cog", 6, "CFG")]
    GestaoDeTerceiros,

    /// <summary>
    /// Módulo de Configurações.
    /// </summary>
    [MenuModuleInfo("Configurações", "fas fa-cog", 6, "CFG")]
    ControleAcessoPortaria,

    /// <summary>
    /// Outros itens não categorizados.
    /// </summary>
    [MenuModuleInfo("Outros", "fas fa-ellipsis-h", 99, "OUT")]
    Outros
        */


    [MenuModuleInfo("Gestão de Pessoas", "fas fa-cog", 6, "RHU")]
    GestaoDePessoas,

    [MenuModuleInfo("Gestão de Terceiros", "fas fa-cog", 6, "GTC")]
    GestaooDeTerceiros,
    
    [MenuModuleInfo("Controle Acesso Portaria", "fas fa-cog", 6, "CAP")]
    ControleAcessoPortaria,

    [MenuModuleInfo("Controle de Frequencia", "fas fa-cog", 6, "CPO")]
    ControleDePonto,

    [MenuModuleInfo("Treinamento", "fas fa-cog", 6, "TRE")]
    Treinamento,

    [MenuModuleInfo("Saúde Ocupacional", "fas fa-cog", 6, "MSO")]
    SaudeOcupacional,

    [MenuModuleInfo("Segurança", "fas fa-cog", 6, "SEG")]
    Seguranca,

    [MenuModuleInfo("Avaliação360", "fas fa-cog", 6, "AVA")]
    Avaliacao,

    [MenuModuleInfo("eSocial", "fas fa-cog", 6, "ESO")]
    eSocial,

    [MenuModuleInfo("Gestão de EPI", "fas fa-cog", 6, "EPI")]
    GestaodeEPI

}

/// <summary>
/// Atributo para configurar informações do módulo.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class MenuModuleInfoAttribute : Attribute
{
    public string DisplayName { get; }
    public string Icon { get; }
    public int Order { get; }
    public string CdSistema { get; }

    public MenuModuleInfoAttribute(string displayName, string icon, int order, string cdSistema)
    {
        DisplayName = displayName;
        Icon = icon;
        Order = order;
        CdSistema = cdSistema;
    }
}