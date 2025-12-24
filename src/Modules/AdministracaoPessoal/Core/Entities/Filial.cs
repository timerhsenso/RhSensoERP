using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Filial/Estabelecimento da empresa
/// Tabela: test1
/// </summary>
[GenerateCrud(
    TableName = "test1",
    DisplayName = "Filial",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_FILIAL",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("test1")]
public class Filial
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdempresa")]
    [Display(Name = "Código Empresa")]
    public int CdEmpresa { get; set; }

    [Column("cdfilial")]
    [Display(Name = "Código Filial")]
    public int CdFilial { get; set; }

    [Column("nmfantasia")]
    [StringLength(30)]
    [Display(Name = "Nome Fantasia")]
    public string NmFantasia { get; set; } = string.Empty;

    [Column("dcestab")]
    [StringLength(60)]
    [Display(Name = "Descrição Estabelecimento")]
    public string? DcEstab { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados de Endereço
    // ═══════════════════════════════════════════════════════════════════

    [Column("dcendereco")]
    [StringLength(60)]
    [Display(Name = "Endereço")]
    public string? DcEndereco { get; set; }

    [Column("numero")]
    [StringLength(6)]
    [Display(Name = "Número")]
    public string? Numero { get; set; }

    [Column("DCEND_COMP")]
    [StringLength(60)]
    [Display(Name = "Complemento")]
    public string? DcEndComp { get; set; }

    [Column("dcbairro")]
    [StringLength(60)]
    [Display(Name = "Bairro")]
    public string? DcBairro { get; set; }

    [Column("sgestado")]
    [StringLength(2)]
    [Display(Name = "UF")]
    public string? SgEstado { get; set; }

    [Column("nocep")]
    [StringLength(9)]
    [Display(Name = "CEP")]
    public string? NoCep { get; set; }

    [Column("cdmunicip")]
    [StringLength(5)]
    [Display(Name = "Código Município")]
    public string? CdMunicip { get; set; }

    [Column("cdmunirais")]
    [StringLength(7)]
    [Display(Name = "Código Município RAIS")]
    public string? CdMuniRais { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Contato
    // ═══════════════════════════════════════════════════════════════════

    [Column("noddd")]
    [StringLength(3)]
    [Display(Name = "DDD")]
    public string? NoDdd { get; set; }

    [Column("notelefone")]
    [StringLength(15)]
    [Display(Name = "Telefone")]
    public string? NoTelefone { get; set; }

    [Column("nofax")]
    [StringLength(10)]
    [Display(Name = "Fax")]
    public string? NoFax { get; set; }

    [Column("EMAIL")]
    [StringLength(30)]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Documentos e Inscrições
    // ═══════════════════════════════════════════════════════════════════

    [Column("cdcgc")]
    [StringLength(15)]
    [Display(Name = "CNPJ")]
    public string? CdCgc { get; set; }

    [Column("nomatinps")]
    [StringLength(15)]
    [Display(Name = "Matrícula INSS")]
    public string? NoMatInps { get; set; }

    [Column("noinscriest")]
    [StringLength(15)]
    [Display(Name = "Inscrição Estadual")]
    public string? NoInscriEst { get; set; }

    [Column("noinscricei")]
    [StringLength(15)]
    [Display(Name = "Inscrição CEI")]
    public string? NoInscriCei { get; set; }

    [Column("noinscrimun")]
    [StringLength(15)]
    [Display(Name = "Inscrição Municipal")]
    public string? NoInscriMun { get; set; }

    [Column("cdtpinscri")]
    [Display(Name = "Tipo de Inscrição")]
    public int? CdTpInscri { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Classificações e Atividades
    // ═══════════════════════════════════════════════════════════════════

    [Column("cdatvinps")]
    [StringLength(7)]
    [Display(Name = "Código Atividade INSS")]
    public string? CdAtvInps { get; set; }

    [Column("cdativibge")]
    [StringLength(5)]
    [Display(Name = "Código Atividade IBGE")]
    public string? CdAtivIbge { get; set; }

    [Column("cdnatjus")]
    [StringLength(4)]
    [Display(Name = "Natureza Jurídica")]
    public string? CdNatJus { get; set; }

    [Column("cdactivir")]
    [StringLength(4)]
    [Display(Name = "Código Atividade IR")]
    public string? CdAtivIr { get; set; }

    [Column("noproprie")]
    [StringLength(2)]
    [Display(Name = "Propriedade")]
    public string? NoProprie { get; set; }

    [Column("flcnae")]
    [StringLength(1)]
    [Display(Name = "Flag CNAE")]
    public string? FlCnae { get; set; }

    [Column("cdactrab")]
    [StringLength(7)]
    [Display(Name = "Código AC Trabalho")]
    public string? CdAcTrab { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados Tributários e Previdenciários
    // ═══════════════════════════════════════════════════════════════════

    [Column("cdfpas")]
    [StringLength(3)]
    [Display(Name = "Código FPAS")]
    public string? CdFpas { get; set; }

    [Column("cdterc")]
    [StringLength(4)]
    [Display(Name = "Código Terceiros")]
    public string? CdTerc { get; set; }

    [Column("cdgps")]
    [StringLength(4)]
    [Display(Name = "Código GPS")]
    public string? CdGps { get; set; }

    [Column("cdsimples")]
    [StringLength(2)]
    [Display(Name = "Código Simples")]
    public string? CdSimples { get; set; }

    [Column("pcconv")]
    [Display(Name = "% Convênio")]
    public decimal PcConv { get; set; }

    [Column("pcsat")]
    [Display(Name = "% SAT")]
    public decimal PcSat { get; set; }

    [Column("pcterc")]
    [Display(Name = "% Terceiros")]
    public decimal PcTerc { get; set; }

    [Column("pcemp")]
    [Display(Name = "% Empresa")]
    public decimal PcEmp { get; set; }

    [Column("pc_fap")]
    [Display(Name = "% FAP")]
    public decimal? PcFap { get; set; }

    [Column("pcfilantropia")]
    [Display(Name = "% Filantropia")]
    public decimal? PcFilantropia { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados FGTS
    // ═══════════════════════════════════════════════════════════════════

    [Column("cdbcofgts")]
    [StringLength(3)]
    [Display(Name = "Banco FGTS")]
    public string? CdBcoFgts { get; set; }

    [Column("cdagefgts")]
    [StringLength(4)]
    [Display(Name = "Agência FGTS")]
    public string? CdAgeFgts { get; set; }

    [Column("cdidempcef")]
    [StringLength(13)]
    [Display(Name = "ID Empresa CEF")]
    public string? CdIdEmpCef { get; set; }

    [Column("flrecfgts")]
    [StringLength(2)]
    [Display(Name = "Flag Recolhimento FGTS")]
    public string? FlRecFgts { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Configurações de Frequência e Ponto
    // ═══════════════════════════════════════════════════════════════════

    [Column("FLADTNOT")]
    [Display(Name = "Flag Adicional Noturno")]
    public int FlAdtNot { get; set; }

    [Column("INIADTNOT")]
    [StringLength(5)]
    [Display(Name = "Início Adicional Noturno")]
    public string? IniAdtNot { get; set; }

    [Column("FIMADTNOT")]
    [StringLength(5)]
    [Display(Name = "Fim Adicional Noturno")]
    public string? FimAdtNot { get; set; }

    [Column("FLLIMTROCA")]
    [Display(Name = "Flag Limite Troca")]
    public int FlLimTroca { get; set; }

    [Column("LIMTROCA")]
    [Display(Name = "Limite Troca")]
    public int? LimTroca { get; set; }

    [Column("VALORHORAADN")]
    [Display(Name = "Valor Hora Adicional Noturno")]
    public decimal? ValorHoraAdn { get; set; }

    [Column("FLDESCONTAALMOCO")]
    [Display(Name = "Flag Desconta Almoço")]
    public int? FlDescontaAlmoco { get; set; }

    [Column("FLMINHE")]
    [Display(Name = "Flag Mínimo HE")]
    public int FlMinHe { get; set; }

    [Column("VLMINHE")]
    [Display(Name = "Valor Mínimo HE")]
    public int? VlMinHe { get; set; }

    [Column("CQTHORAMAX")]
    [StringLength(30)]
    [Display(Name = "Carga Horária Máxima")]
    public string? CQtHoraMax { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Outros
    // ═══════════════════════════════════════════════════════════════════

    [Column("cdsindicatres")]
    [StringLength(2)]
    [Display(Name = "Código Sindicato")]
    public string? CdSindicatRes { get; set; }

    [Column("flativofilial")]
    [Display(Name = "Filial Ativa")]
    public int? FlAtivoFilial { get; set; }

    [Column("dtinivalidade")]
    [Display(Name = "Data Início Validade")]
    public DateTime? DtIniValidade { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // eSocial
    // ═══════════════════════════════════════════════════════════════════

    [Column("indsubstpatrobra")]
    [Display(Name = "Indicador Substituição Patronal Obra")]
    public short? IndSubstPatrObra { get; set; }

    [Column("numeroprocessoapcd")]
    [StringLength(20)]
    [Display(Name = "Número Processo APCD")]
    public string? NumeroProcessoApcd { get; set; }

    [Column("numeroprocessoaprendiz")]
    [StringLength(20)]
    [Display(Name = "Número Processo Aprendiz")]
    public string? NumeroProcessoAprendiz { get; set; }

    [Column("tpcaepf")]
    [Display(Name = "Tipo CAEPF")]
    public short? TpCaepf { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // FKs para entidades do MESMO MÓDULO (GestaoDePessoas) ✅
    // ═══════════════════════════════════════════════════════════════════

    [Column("idempresa")]
    [Display(Name = "Empresa")]
    public Guid? IdEmpresa { get; set; }

    [ForeignKey(nameof(IdEmpresa))]
    public virtual Empresa? Empresa { get; set; }

    [Column("idmunicipioendereco")]
    [Display(Name = "Município")]
    public Guid? IdMunicipioEndereco { get; set; }

    [ForeignKey(nameof(IdMunicipioEndereco))]
    public virtual Municipio? Municipio { get; set; }

    [Column("idsindicato")]
    [Display(Name = "Sindicato")]
    public Guid? IdSindicato { get; set; }

    [ForeignKey(nameof(IdSindicato))]
    public virtual Sindicato? Sindicato { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // FKs para entidades de OUTRO MÓDULO (Esocial) ⚠️
    // APENAS o ID - SEM navigation property
    // ═══════════════════════════════════════════════════════════════════

    [Column("idlotacaotributaria")]
    [Display(Name = "Lotação Tributária")]
    public Guid? IdLotacaoTributaria { get; set; }

    [Column("tpocorrHE")]
    [Display(Name = "Tipo Ocorrência HE")]
    public int? TpOcorrHE { get; set; }

    [Column("cdmotocHE")]
    [StringLength(4)]
    [Display(Name = "Código Motivo HE")]
    public string? CdMotocHE { get; set; }

    [Column("tpocorrFALTA")]
    [Display(Name = "Tipo Ocorrência Falta")]
    public int? TpOcorrFalta { get; set; }

    [Column("cdmotocFALTA")]
    [StringLength(4)]
    [Display(Name = "Código Motivo Falta")]
    public string? CdMotocFalta { get; set; }

    [Column("TPOCORRATRAZO")]
    [Display(Name = "Tipo Ocorrência Atraso")]
    public int? TpOcorrAtraso { get; set; }

    [Column("CDMOTOCATRAZO")]
    [StringLength(4)]
    [Display(Name = "Código Motivo Atraso")]
    public string? CdMotocAtraso { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections do MESMO MÓDULO (GestaoDePessoas) ✅
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Colaboradores vinculados a esta filial
    /// </summary>
    [InverseProperty(nameof(Colaborador.Filial))]
    public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
