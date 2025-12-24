using RhSensoERP.Shared.Contracts.Esocial.DTOs;

namespace RhSensoERP.Shared.Contracts.Esocial.Interfaces;

/// <summary>
/// Serviço de lookup para dados do módulo eSocial
/// Permite que outros módulos consultem dados sem criar acoplamento direto
/// </summary>
public interface IEsocialLookupService
{
    // Tabela 4 - FPAS
    Task<IReadOnlyList<Tabela4LookupDto>> GetTabela4Async(CancellationToken ct = default);
    Task<Tabela4LookupDto?> GetTabela4ByCodigoAsync(string codigo, CancellationToken ct = default);

    // Tabela 8 - Classificação Tributária
    Task<IReadOnlyList<Tabela8LookupDto>> GetTabela8Async(CancellationToken ct = default);
    Task<Tabela8LookupDto?> GetTabela8ByCodigoAsync(string codigo, CancellationToken ct = default);

    // Tabela 10 - Tipos de Lotação Tributária
    Task<IReadOnlyList<Tabela10LookupDto>> GetTabela10Async(CancellationToken ct = default);
    Task<Tabela10LookupDto?> GetTabela10ByCodigoAsync(string codigo, CancellationToken ct = default);

    // Tabela 21 - Natureza Jurídica
    Task<IReadOnlyList<Tabela21LookupDto>> GetTabela21Async(CancellationToken ct = default);
    Task<Tabela21LookupDto?> GetTabela21ByCodigoAsync(string codigo, CancellationToken ct = default);

    // Lotação Tributária
    Task<IReadOnlyList<LotacaoTributariaLookupDto>> GetLotacoesTributariasAsync(CancellationToken ct = default);
    Task<LotacaoTributariaLookupDto?> GetLotacaoTributariaByIdAsync(Guid id, CancellationToken ct = default);
    Task<LotacaoTributariaLookupDto?> GetLotacaoTributariaByCodLotacaoAsync(string codLotacao, CancellationToken ct = default);

    // Motivo de Ocorrência
    Task<IReadOnlyList<MotivoOcorrenciaLookupDto>> GetMotivosOcorrenciaAsync(int? tipoOcorrencia = null, CancellationToken ct = default);
    Task<MotivoOcorrenciaLookupDto?> GetMotivoOcorrenciaByIdAsync(Guid id, CancellationToken ct = default);
    Task<MotivoOcorrenciaLookupDto?> GetMotivoOcorrenciaByCodigoAsync(string cdMotoc, int tpOcorr, CancellationToken ct = default);
    Task<IReadOnlyList<MotivoOcorrenciaLookupDto>> GetMotivosHoraExtraAsync(CancellationToken ct = default);
    Task<IReadOnlyList<MotivoOcorrenciaLookupDto>> GetMotivosFaltaAsync(CancellationToken ct = default);
    Task<IReadOnlyList<MotivoOcorrenciaLookupDto>> GetMotivosAtrasoAsync(CancellationToken ct = default);
}