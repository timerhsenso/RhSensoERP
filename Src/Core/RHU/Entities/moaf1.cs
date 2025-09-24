using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>moaf1</b> — contém os <b>motivos de afastamento</b> e sua situação padrão.
    /// PK composta: CdMotAfas + CdSituacao.
    /// </summary>
    public class MotivoAfastamento : BaseEntity
    {
        public string CdMotAfas { get; set; } = string.Empty;  // char(2)
        public string CdSituacao { get; set; } = string.Empty; // char(2)
        public string? DcMotAfas { get; set; }
        public System.Guid? IdSituacao { get; set; }

        public virtual Situacao Situacao { get; set; } = null!;
    }
}
