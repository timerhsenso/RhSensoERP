using RhSensoERP.Shared.Core.Entities;

namespace RhSensoERP.Identity.Entities;

/// <summary>
/// Entidade que representa um sistema no ERP (tabela tsistema)
/// Estrutura baseada no script SQL do banco bd_rhu_copenor
/// </summary>
public class Sistema : BaseEntity
{
    /// <summary>
    /// Código do sistema (PK) - char(10) - ex: "SEG", "RHU", "FIN"
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do sistema - varchar(60) - ex: "Segurança", "Recursos Humanos"
    /// </summary>
    public string DcSistema { get; set; } = string.Empty;

    /// <summary>
    /// Sistema ativo - bit - default 1 (true)
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Relacionamentos
    public virtual ICollection<Funcao> Funcoes { get; set; } = new List<Funcao>();
}