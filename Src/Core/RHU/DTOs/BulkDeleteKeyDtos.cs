namespace RhSensoERP.Core.RHU.DTOs
{
    // DTOs de chaves para bulk-delete (um por entidade) - nomes únicos evitam colisão no Swagger
    public class EmpresaKeyDto { public int CdEmpresa { get; set; } }
    public class FilialKeyDto { public int CdEmpresa { get; set; } public int CdFilial { get; set; } }
    public class SituacaoKeyDto { public string CdSituacao { get; set; } = string.Empty; }
    public class MotivoAfastamentoKeyDto { public string CdMotAfas { get; set; } = string.Empty; public string CdSituacao { get; set; } = string.Empty; }
    public class CargoKeyDto { public string CdCargo { get; set; } = string.Empty; }
    public class MunicipioKeyDto { public string CdMunicip { get; set; } = string.Empty; }
    public class CalendarioMunicipalKeyDto { public string CdMunicip { get; set; } = string.Empty; public System.DateTime DtCalend { get; set; } }
    public class AfastamentoKeyDto { public string NoMatric { get; set; } = string.Empty; public int CdEmpresa { get; set; } public int CdFilial { get; set; } public System.DateTime DtAfast { get; set; } }
    public class FeriasProgramacaoKeyDto { public string NoMatric { get; set; } = string.Empty; public int CdEmpresa { get; set; } public int CdFilial { get; set; } public System.DateTime DtIniPa { get; set; } public int NoSequenc { get; set; } }
    public class FichaFinanceiraKeyDto { public string NoMatric { get; set; } = string.Empty; public int CdEmpresa { get; set; } public int CdFilial { get; set; } public string CdConta { get; set; } = string.Empty; public System.DateTime DtConta { get; set; } }
    public class LancamentoCalculadoKeyDto { public string NoMatric { get; set; } = string.Empty; public int CdEmpresa { get; set; } public int CdFilial { get; set; } public string NoProcesso { get; set; } = string.Empty; public string CdConta { get; set; } = string.Empty; }
    public class VerbaKeyDto { public string CdConta { get; set; } = string.Empty; }
    public class CentroCustoKeyDto { public string CdCcusto { get; set; } = string.Empty; }
}
