// src/Web/Models/Common/BatchDeleteResultDto.cs
namespace RhSensoERP.Web.Models.Common;

/// <summary>
/// Resultado de operacao de exclusao em lote.
/// </summary>
public class BatchDeleteResultDto
{
    /// <summary>
    /// Quantidade de registros excluidos com sucesso.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Quantidade de registros que falharam.
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Total de registros processados.
    /// </summary>
    public int TotalCount => SuccessCount + FailureCount;

    /// <summary>
    /// Indica se todas as exclusoes foram bem-sucedidas.
    /// </summary>
    public bool AllSucceeded => FailureCount == 0;

    /// <summary>
    /// Lista de erros por codigo.
    /// </summary>
    public List<BatchDeleteErrorDto> Errors { get; set; } = new();
}

/// <summary>
/// Erro de exclusao individual.
/// </summary>
public class BatchDeleteErrorDto
{
    /// <summary>
    /// Codigo do registro que falhou.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Mensagem de erro.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}