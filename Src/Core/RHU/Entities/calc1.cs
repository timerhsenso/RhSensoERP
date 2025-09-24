using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>calc1</b> — contém os <b>lançamentos calculados</b> por processo de folha.
    /// PK composta: NoMatric, CdEmpresa, CdFilial, NoProcesso, CdConta.
    /// </summary>
    public class LancamentoCalculado : BaseEntity
    {
        public string NoMatric { get; set; } = string.Empty;
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string CdConta { get; set; } = string.Empty;    // char(4)
        public string NoProcesso { get; set; } = string.Empty; // char(6)
        public string? CdOrigem { get; set; }
        public double? VlConta { get; set; }
        public double? QtConta { get; set; }
        public System.DateTime? DtRef { get; set; }
        public string? CdUsuario { get; set; }
        public bool? Rc { get; set; }
    }
}
