using RhSensoERP.Core.Abstractions.Entities;
using System;

public class Sistema : BaseEntity
{
    public string CdSistema { get; set; } = string.Empty;        // PK
    public string DcSistema { get; set; } = string.Empty;        // Descrição
    public bool Ativo { get; set; } = true;                     // Status

    public virtual ICollection<Funcao> Funcoes { get; set; } = new List<Funcao>();
}