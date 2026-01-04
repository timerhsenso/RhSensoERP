// =============================================================================
// RHSENSOERP - NAVIGATION DISPLAY ATTRIBUTE
// =============================================================================
// Controla como propriedades de navegação aparecem no DTO, Grid e Metadados
// =============================================================================
using System;

namespace RhSensoERP.Shared.Core.Attributes;

/// <summary>
/// Configura como uma propriedade de navegação deve ser exibida no sistema.
/// Use em propriedades virtuais de navegação (ex: public virtual Fornecedor? Fornecedor).
/// 
/// COMPORTAMENTO AUTOMÁTICO:
/// - Gera propriedade no DTO (ex: FornecedorRazaoSocial)
/// - Gera mapeamento no AutoMapper
/// - Gera Include() nas queries
/// - Gera metadados para frontend
/// - Gera coluna no grid (se GridColumn = true)
/// - Gera lookup no formulário (sempre)
/// 
/// EXEMPLO 1 - MESMO MÓDULO:
/// [NavigationDisplay(Property = "RazaoSocial", GridColumn = true)]
/// public virtual CapFornecedores? Fornecedor { get; set; }
/// 
/// EXEMPLO 2 - CROSS-MODULE:
/// [NavigationDisplay(
///     Property = "Descricao",
///     Module = "AdministracaoPessoal",
///     EntityRoute = "tiposanguineo",
///     GridColumn = true,
///     GridHeader = "Tipo Sanguíneo"
/// )]
/// public virtual BasTiposSanguineo? TipoSanguineo { get; set; }
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class NavigationDisplayAttribute : Attribute
{
	/// <summary>
	/// Nome da propriedade a exibir na entidade relacionada.
	/// OBRIGATÓRIO.
	/// 
	/// Exemplos:
	/// - "RazaoSocial" (para Fornecedor)
	/// - "Nome" (para Funcionario)
	/// - "Descricao" (para TipoSanguineo)
	/// - "Sigla" (para UF)
	/// </summary>
	public string Property { get; set; } = "";

	/// <summary>
	/// Módulo da entidade relacionada na API.
	/// OPCIONAL - Se não especificado, usa o módulo da entidade atual.
	/// 
	/// Use apenas para FKs cross-module.
	/// 
	/// Exemplos:
	/// - "AdministracaoPessoal"
	/// - "GestaoTerceirosPrestadores"
	/// - "Financeiro"
	/// </summary>
	public string? Module { get; set; }

	/// <summary>
	/// Route da entidade relacionada na API.
	/// OPCIONAL - Se não especificado, gera automaticamente do nome da propriedade.
	/// 
	/// Exemplos:
	/// - "fornecedores"
	/// - "tiposanguineo"
	/// - "ufs"
	/// 
	/// Formato gerado automaticamente:
	/// - IdFornecedor → "fornecedores"
	/// - IdTipoSanguineo → "tiposanguineo"
	/// </summary>
	public string? EntityRoute { get; set; }

	/// <summary>
	/// Se true, gera coluna no grid com o valor desta navegação.
	/// Se false, mostra apenas no formulário como Select2.
	/// 
	/// DEFAULT: false
	/// 
	/// EFEITO:
	/// - GridColumn = true  → Grid mostra "Tipo Sanguíneo: O+", Form mostra Select2
	/// - GridColumn = false → Grid NÃO mostra, Form mostra Select2
	/// </summary>
	public bool GridColumn { get; set; } = false;

	/// <summary>
	/// Header da coluna no grid (só usado se GridColumn = true).
	/// OPCIONAL - Se não especificado, usa DisplayName da navegação.
	/// 
	/// Exemplos:
	/// - "Fornecedor"
	/// - "Tipo Sanguíneo"
	/// - "UF"
	/// </summary>
	public string? GridHeader { get; set; }

	/// <summary>
	/// Nome da propriedade gerada no DTO.
	/// OPCIONAL - Se não especificado, gera automaticamente.
	/// 
	/// Formato automático: {NavigationName}{PropertyName}
	/// 
	/// Exemplos gerados automaticamente:
	/// - Fornecedor + RazaoSocial → FornecedorRazaoSocial
	/// - TipoSanguineo + Descricao → TipoSanguineoDescricao
	/// - Uf + Sigla → UfSigla
	/// </summary>
	public string? DtoPropertyName { get; set; }

	/// <summary>
	/// Order da coluna no grid (só usado se GridColumn = true).
	/// OPCIONAL - Se não especificado, usa ordem da propriedade.
	/// </summary>
	public int GridOrder { get; set; } = 0;
}