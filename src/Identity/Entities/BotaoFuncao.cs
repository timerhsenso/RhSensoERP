using RhSensoERP.Shared.Core.Entities;

namespace RhSensoERP.Identity.Entities;

/// <summary>
/// Entidade que representa botões/ações de uma função (tabela btfuncao)
/// Estrutura baseada no script SQL do banco bd_rhu_copenor
/// </summary>
public class BotaoFuncao : BaseEntity
{
    /// <summary>
    /// Código da função - varchar(30) - parte da PK composta
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Código do sistema - char(10) - parte da PK composta
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Nome do botão - varchar(30) - parte da PK composta
    /// </summary>
    public string NmBotao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do botão - varchar(60) - obrigatório
    /// </summary>
    public string DcBotao { get; set; } = string.Empty;

    /// <summary>
    /// Código da ação - char(1) - I/A/E/C (Incluir/Alterar/Excluir/Consultar)
    /// </summary>
    public char CdAcao { get; set; }

    // Relacionamentos
    public virtual Funcao Funcao { get; set; } = null!;
}