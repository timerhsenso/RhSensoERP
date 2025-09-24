using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>FREQ4</b> — contém o <b>detalhamento</b> das marcações/intervalos do dia por colaborador.
    /// </summary>
    public class FrequenciaDetalhe : BaseEntity
    {
        public string NoMatric { get; set; } = string.Empty;
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public System.DateTime Data { get; set; }
        public System.DateTime Inicio { get; set; }
        public System.DateTime? Fim { get; set; }
        public System.DateTime? InicioIntervalo { get; set; }
        public System.DateTime? FimIntervalo { get; set; }
        public string? FlIntervalo { get; set; }
    }
}
