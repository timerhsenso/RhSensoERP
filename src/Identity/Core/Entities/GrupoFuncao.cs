
namespace RhSensoERP.Identity.Core.Entities;

/// <summary>
/// Entidade que representa permissões de grupos nas funções (tabela hbrh1)
/// Define quais ações cada grupo pode executar em cada função
/// Estrutura baseada no script SQL do banco bd_rhu_copenor
/// </summary>
public class GrupoFuncao 
{
    /// <summary>
    /// Código do grupo - varchar(30) - referência ao grupo
    /// </summary>
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Código da função - varchar(30) - referência à função
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Código das ações permitidas - char(20) - ex: "IAEC" (Incluir/Alterar/Excluir/Consultar)
    /// </summary>
    public string CdAcoes { get; set; } = string.Empty;

    /// <summary>
    /// Código de restrição - char(1) - S/N ou outro controle
    /// </summary>
    public char CdRestric { get; set; }

    /// <summary>
    /// Código do sistema - char(10) nullable - referência ao sistema
    /// </summary>
    public string? CdSistema { get; set; }

    /// <summary>
    /// ID do grupo de usuário - uniqueidentifier nullable - FK para gurh1.id
    /// </summary>
    public Guid? IdGrupoDeUsuario { get; set; }

    // Relacionamentos
    public virtual Funcao? Funcao { get; set; }
    public virtual GrupoDeUsuario? GrupoDeUsuario { get; set; }
}