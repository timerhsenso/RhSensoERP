using RhSensoERP.Core.Abstractions.Entities;
using System.Collections.Generic;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>tsitu1</b> — contém as <b>situações</b> utilizadas em RH (vários contextos).
    /// </summary>
    public class Situacao : BaseEntity
    {
        public string CdSituacao { get; set; } = string.Empty; // char(2) - PK
        public string? DcSituacao { get; set; }
        public virtual ICollection<MotivoAfastamento> MotivosAfastamento { get; set; } = new List<MotivoAfastamento>();
    }
}
