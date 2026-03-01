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
    public MenuModule Module { get; set; } = MenuModule.AdministracaoPessoal;

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


    // Exemplo: enum de módulos do menu (1 item por projeto em /Modules)

    [MenuModuleInfo("Administração de Pessoal", "fas fa-users-cog", 6, "ADP")]
    AdministracaoPessoal,

    [MenuModuleInfo("Cargos, Salários e Remuneração", "fas fa-money-check-alt", 6, "CSR")]
    CargosSalariosRemuneracao,

    [MenuModuleInfo("Compliance Trabalhista / Jurídico", "fas fa-balance-scale", 6, "CTJ")]
    ComplianceTrabalhistaJuridico,

    [MenuModuleInfo("Folha, Pagamentos e Encargos", "fas fa-file-invoice-dollar", 6, "FOL")]
    FolhaPagamentoEncargos,

    [MenuModuleInfo("Gestão de Benefícios", "fas fa-gift", 6, "BEN")]
    GestaoBeneficios,

    [MenuModuleInfo("Gestão Documental", "fas fa-folder-open", 6, "DOC")]
    GestaoDocumental,

    [MenuModuleInfo("Jornada e Ponto", "fas fa-user-clock", 6, "CPO")]
    GestaoJornadaPonto,

    [MenuModuleInfo("Portaria e Controle de Acesso", "fas fa-id-card", 6, "CAP")]
    GestaoPortariaAcesso,

    [MenuModuleInfo("Talentos e Desempenho", "fas fa-chart-line", 6, "TDE")]
    GestaoTalentosDesempenho,

    [MenuModuleInfo("Terceiros e Prestadores", "fas fa-people-carry", 6, "GTP")]
    GestaoTerceirosPrestadores,

    [MenuModuleInfo("Integrações e Mensageria", "fas fa-exchange-alt", 6, "IMS")]
    IntegracoesMensageria,

    [MenuModuleInfo("Multi-Tenant (SaaS)", "fas fa-sitemap", 6, "SAS")]
    MultiTenant,

    [MenuModuleInfo("People Analytics / BI", "fas fa-chart-pie", 6, "PBI")]
    PeopleAnalyticsBI,

    [MenuModuleInfo("Portal do Colaborador", "fas fa-user", 6, "PCO")]
    PortalColaborador,

    [MenuModuleInfo("Recrutamento e Seleção", "fas fa-user-plus", 6, "RSE")]
    RecrutamentoSelecao,

    [MenuModuleInfo("Saúde e Segurança do Trabalho", "fas fa-hard-hat", 6, "SST")]
    SaudeSegurancaTrabalho,

    [MenuModuleInfo("Segurança", "fas fa-shield-alt", 6, "SEG")]
    Seguranca,

    [MenuModuleInfo("Tabelas Compartilhadas", "fas fa-database", 6, "TBL")]
    TabelasCompartilhadas,

    [MenuModuleInfo("Treinamento e Desenvolvimento", "fas fa-chalkboard-teacher", 6, "TRE")]
    TreinamentoDesenvolvimento,

    [MenuModuleInfo("Viagens e Despesas", "fas fa-plane", 6, "VDP")]
    ViagensDespesas

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