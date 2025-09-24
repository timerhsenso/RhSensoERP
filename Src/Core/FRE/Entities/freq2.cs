using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>freq2</b> — contém os <b>registros compactos/resumo</b> de frequência por colaborador/dia.
    /// </summary>
    public class Frequencia2 : BaseEntity
    {
        public int Id { get; set; }
        public string NoMatric { get; set; } = string.Empty;
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public System.DateTime Data { get; set; }
        public System.DateTime Inicio { get; set; }
        public System.DateTime? Fim { get; set; }
        public System.DateTime? DtFrequen { get; set; }
        public int Importado { get; set; }
        public int? Erro { get; set; }
        public int? Erro2 { get; set; }
        public int QtMinDescFds { get; set; }
    }
}
