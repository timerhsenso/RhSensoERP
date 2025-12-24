using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Identity.Core.Entities;

/// <summary>
/// Entidade que representa botões/ações de uma função (tabela dbo.btfuncao).
/// Tabela legada sem colunas de auditoria.
/// PK composta: (CdSistema, CdFuncao, NmBotao)
/// </summary>
public class BotaoFuncao
{
    /// <summary>
    /// Código do sistema - char(10) - parte da PK composta.
    /// </summary>
    [Required, StringLength(10)]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código da função - varchar(30) - parte da PK composta.
    /// </summary>
    [Required, StringLength(30)]
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Nome do botão - varchar(30) - parte da PK composta.
    /// </summary>
    [Required, StringLength(30)]
    public string NmBotao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do botão - varchar(60) - obrigatório.
    /// </summary>
    [Required, StringLength(60)]
    public string DcBotao { get; set; } = string.Empty;

    /// <summary>
    /// Código da ação - char(1) NOT NULL - I/A/E/C (Incluir/Alterar/Excluir/Consultar)
    /// </summary>
    public char CdAcao { get; set; }

    // Relacionamentos
    public virtual Funcao Funcao { get; set; } = null!;
}
