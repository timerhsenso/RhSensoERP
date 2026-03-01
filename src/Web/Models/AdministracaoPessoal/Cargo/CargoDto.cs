// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Cargo
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:19:49
// =============================================================================

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Cargo;

/// <summary>
/// DTO de leitura para Cargos.
/// Compatível com backend: RhSensoERP.Modules.AdministracaoPessoal.Application.DTOs.CargoDto
/// </summary>
public class CargoDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    public string Cdcargo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Dccargo { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    public int Flativo { get; set; }

    /// <summary>
    /// Data Início Validade
    /// </summary>
    public DateTime? Dtinival { get; set; }

    /// <summary>
    /// Data Fim Validade
    /// </summary>
    public DateTime? Dtfimval { get; set; }

    /// <summary>
    /// Código Tabela Salarial
    /// </summary>
    public string Cdtabela { get; set; } = string.Empty;

    /// <summary>
    /// Nível Inicial
    /// </summary>
    public string Cdniveini { get; set; } = string.Empty;

    /// <summary>
    /// Nível Final
    /// </summary>
    public string Cdnivefim { get; set; } = string.Empty;

    /// <summary>
    /// Código Instrução
    /// </summary>
    public string CdinStruc { get; set; } = string.Empty;

    /// <summary>
    /// Código CBO (5 dígitos)
    /// </summary>
    public string Cdcbo { get; set; } = string.Empty;

    /// <summary>
    /// Código CBO (6 dígitos)
    /// </summary>
    public string Cdcbo6 { get; set; } = string.Empty;

    /// <summary>
    /// Grupo Profissional
    /// </summary>
    public string Cdgrprof { get; set; } = string.Empty;

    /// <summary>
    /// Tenant
    /// </summary>
    public int Tenant { get; set; }

    /// <summary>
    /// Idcbo
    /// </summary>
    public Guid? Idcbo { get; set; }

    /// <summary>
    /// Idgraudeinstrucao
    /// </summary>
    public Guid? IdgraudeinStrucao { get; set; }

    /// <summary>
    /// Idtabelasalarial
    /// </summary>
    public Guid? Idtabelasalarial { get; set; }
}
