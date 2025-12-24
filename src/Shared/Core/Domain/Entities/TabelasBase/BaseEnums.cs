namespace RhSensoERP.Shared.Core.Domain.Entities.TabelasBase;

/// <summary>
/// Tipos de feriado.
/// CHECK: Tipo IN ('NACIONAL', 'ESTADUAL', 'MUNICIPAL', 'PONTO_FACULTATIVO')
/// </summary>
public enum TipoFeriado
{
    NACIONAL,
    ESTADUAL,
    MUNICIPAL,
    PONTO_FACULTATIVO
}

/// <summary>
/// Tipos de numeração sequencial.
/// CHECK: TipoNumeracao IN ('PROTOCOLO_ACESSO', 'AUTORIZACAO', 'AGENDAMENTO', 
///        'OCORRENCIA', 'ORDEM_SERVICO', 'NUMERO_CRACHA', 'TICKET_PESAGEM')
/// </summary>
public enum TipoNumeracao
{
    PROTOCOLO_ACESSO,
    AUTORIZACAO,
    AGENDAMENTO,
    OCORRENCIA,
    ORDEM_SERVICO,
    NUMERO_CRACHA,
    TICKET_PESAGEM
}

/// <summary>
/// Tipos de operação para log de auditoria.
/// CHECK: TipoOperacao IN ('I', 'U', 'D')
/// </summary>
public enum TipoOperacaoAuditoria
{
    /// <summary>Insert</summary>
    I,
    /// <summary>Update</summary>
    U,
    /// <summary>Delete</summary>
    D
}

/// <summary>
/// Contextos de validação para configuração de obrigatoriedade.
/// CHECK: Contexto IN ('ACESSO_TERCEIRO', 'ACESSO_VISITANTE', 'ACESSO_MOTORISTA',
///        'CARGA_RECEBIMENTO', 'CARGA_PERIGOSA', 'VEICULO', 'EPI')
/// </summary>
public enum ContextoObrigatoriedade
{
    ACESSO_TERCEIRO,
    ACESSO_VISITANTE,
    ACESSO_MOTORISTA,
    CARGA_RECEBIMENTO,
    CARGA_PERIGOSA,
    VEICULO,
    EPI
}
