using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.FRE.Entities
{
    /// <summary>
    /// Tabela <b>jtpa1</b> — contém a <b>quantidade de horas por mês</b> por tipo de jornada/turma.
    /// </summary>
    public class JornadaTipoAno : BaseEntity
    {
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string TpJornada { get; set; } = string.Empty;
        public string AaJornada { get; set; } = string.Empty; // char(4)
        public double? Janeiro { get; set; }
        public double? Fevereiro { get; set; }
        public double? Marco { get; set; }
        public double? Abril { get; set; }
        public double? Maio { get; set; }
        public double? Junho { get; set; }
        public double? Julho { get; set; }
        public double? Agosto { get; set; }
        public double? Setembro { get; set; }
        public double? Outubro { get; set; }
        public double? Novembro { get; set; }
        public double? Dezembro { get; set; }
        public System.Guid? IdFilial { get; set; }
    }
}
