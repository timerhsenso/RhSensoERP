namespace RhSensoERP.Generators.Models;

/// <summary>
/// Informações para geração de métodos Select2 (Lookup).
/// </summary>
public class Select2LookupInfo
{
    public string PropertyName { get; set; } = string.Empty; // Ex: IdFornecedor
    public string DisplayName { get; set; } = string.Empty; // Ex: Fornecedor
    public string EntityName { get; set; } = string.Empty; // Ex: CapFornecedores
    public string DtoName { get; set; } = string.Empty; // Ex: CapFornecedoresDto
    public string MethodName { get; set; } = string.Empty; // Ex: GetFornecedorSelect2Async
    public string ApiRoute { get; set; } = string.Empty; // Ex: /api/gestaoterceirosprestadores/capfornecedores
}
