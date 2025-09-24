using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>ficha1</b> — contém os <b>lançamentos</b> da ficha financeira do colaborador.
    /// </summary>
    public class FichaFinanceira : BaseEntity
    {
        public string NoMatric { get; set; } = string.Empty;
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string CdConta { get; set; } = string.Empty;   // char(4) - FK tcon2
        public System.DateTime DtConta { get; set; }
        public float? VlConta { get; set; }
        public float? QtConta { get; set; }
        public string? NoProcesso { get; set; }
        public int? FlFechamen { get; set; }

        public System.Guid? IdFuncionario { get; set; }
        public System.Guid? IdVerba { get; set; }          // tcon2
        public System.Guid? IdProcessamento { get; set; }
    }
}
