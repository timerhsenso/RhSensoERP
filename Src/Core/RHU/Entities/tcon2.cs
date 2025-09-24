using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>tcon2</b> — contém as <b>verbas/contas</b> utilizadas nos cálculos de folha.
    /// </summary>
    public class Verba : BaseEntity
    {
        public string CdConta { get; set; } = string.Empty;      // char(4) - PK
        public string DcConta { get; set; } = string.Empty;      // varchar(40)
        public string? SgConta { get; set; }
        public string? UfConta { get; set; }
        public string? CdFormula { get; set; }
        public int? NoOrdem { get; set; }
        public double? VlConta { get; set; }
        public string? ChHis { get; set; }
        public string? ChFol { get; set; }
        public string? ChDem { get; set; }
        public string? ChInfor { get; set; }
        public string? ChLanc { get; set; }
        public double? PeConta { get; set; }
        public string? CdTabela { get; set; }
        public string? CdContab { get; set; }
        public int? RefConta { get; set; }
        public string? ChSemPreFolha { get; set; }
        public string? CdTrct { get; set; }
        public string? ChTrct { get; set; }
        public int? NaTrubrica { get; set; }
        public System.Guid? IdConfiguracaoContabilizacao { get; set; }
        public bool Esocial { get; set; }
        public System.Guid? IdTabelaCalculoPorFaixa { get; set; }
        public string? CdContaBc { get; set; }
    }
}
