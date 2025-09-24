using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.RHU.Entities
{
    /// <summary>
    /// Tabela <b>const1</b> — contém as <b>constantes/configurações</b> parametrizáveis do sistema.
    /// </summary>
    public class ConstanteSistema : BaseEntity
    {
        public string CdConstante { get; set; } = string.Empty;  // varchar(60)
        public string DcConstante { get; set; } = string.Empty;  // varchar(255)
        public string? DcConteudo { get; set; }                  // varchar(200)
        public string TpCampo { get; set; } = string.Empty;      // char(1)
        public string? FlAlterar { get; set; }                   // char(1)
        public string? CdFuncao { get; set; }                    // varchar(30)
        public string? CdSistema { get; set; }                   // char(10)
        public string? TxDescricao { get; set; }                 // varchar(4000)
        public bool Config { get; set; }                         // bit default 0
        public string? Tipo { get; set; }                        // char(1)
    }
}
