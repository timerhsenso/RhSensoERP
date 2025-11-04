using RhSensoERP.Shared.Core.Entities;

namespace RhSensoERP.Identity.Entities;

public class User : AuditableEntity
{
    // Campos da tabela tuse1
    public string CdUsuario { get; set; } = string.Empty;
    public string DcUsuario { get; set; } = string.Empty;
    public string? SenhaUser { get; set; }
    public string? NmImpCche { get; set; }
    public char TpUsuario { get; set; }
    public string? NoMatric { get; set; }
    public int? CdEmpresa { get; set; }
    public int? CdFilial { get; set; }
    public int NoUser { get; set; }
    public string? EmailUsuario { get; set; }
    public char FlAtivo { get; set; } = 'S';
    public string? NormalizedUsername { get; set; }
    public Guid? IdFuncionario { get; set; }
    public char? FlNaoRecebeEmail { get; set; }

    // Propriedades de conveniência
    public string Username => CdUsuario;
    public string DisplayName => DcUsuario;
    public string Email => EmailUsuario ?? "";
    public bool Active => FlAtivo == 'S';
    public string PasswordHash => SenhaUser ?? "";

    // Relacionamentos
    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}