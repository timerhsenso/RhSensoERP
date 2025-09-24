using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>test1</b> — contém as <b>filiais/estabelecimentos</b> de cada empresa.
    /// </summary>
    public class Filial : BaseEntity
    {
        public int CdEmpresa { get; set; }  // PK composta (parte)
        public int CdFilial { get; set; }   // PK composta (parte)

        public string? NmFantasia { get; set; }
        public string? DcEstab { get; set; }
        public string? DcEndereco { get; set; }
        public string? DcBairro { get; set; }
        public string? SgEstado { get; set; }
        public string? NoCep { get; set; }
        public string? NoTelefone { get; set; }
        public string? NoFax { get; set; }

        public virtual Empresa Empresa { get; set; } = null!;
    }
}
