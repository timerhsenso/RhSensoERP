using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.Security.Entities;

public class User : BaseEntity
{
    // Campos principais da tabela tuse1
    public string CdUsuario { get; set; } = string.Empty;        // PK
    public string DcUsuario { get; set; } = string.Empty;        // Nome completo
    public string? SenhaUser { get; set; }                       // Senha (nvarchar(20))
    public string? NmImpCche { get; set; }                       // Nome impress�o
    public char TpUsuario { get; set; }                          // Tipo usu�rio
    public string? NoMatric { get; set; }                        // Matr�cula
    public int? CdEmpresa { get; set; }                          // Empresa
    public int? CdFilial { get; set; }                           // Filial
    public int NoUser { get; set; }                              // N�mero usu�rio
    public string? EmailUsuario { get; set; }                    // Email
    public char FlAtivo { get; set; } = 'S';                     // Ativo (S/N)
    public string? NormalizedUsername { get; set; }              // Normalizado
    public Guid? IdFuncionario { get; set; }                     // FK funcion�rio
    public char? FlNaoRecebeEmail { get; set; }                  // Flag email

    // Propriedades de conveni�ncia para JWT
    public string Username => CdUsuario;
    public string DisplayName => DcUsuario;
    public string Email => EmailUsuario ?? "";
    public bool Active => FlAtivo == 'S';

    // Relacionamentos
    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}