using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Colaborador/Funcionário
/// Tabela: func1
/// PK Composta no banco: (nomatric, cdempresa, cdfilial)
/// PK Alternativa: id (GUID) - usada pelo EF Core
/// </summary>
[GenerateCrud(
    TableName = "func1",
    DisplayName = "Colaborador",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_COLABORADOR",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("func1")]
public class Colaborador
{
    // ═══════════════════════════════════════════════════════════════════
    // CHAVE PRIMÁRIA (GUID)
    // ═══════════════════════════════════════════════════════════════════

    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // CHAVE COMPOSTA LEGADA (para compatibilidade)
    // ═══════════════════════════════════════════════════════════════════

    [Column("nomatric")]
    [StringLength(8)]
    [Display(Name = "Matrícula")]
    public string Nomatric { get; set; } = string.Empty;

    [Column("cdempresa")]
    [Display(Name = "Código Empresa")]
    public int Cdempresa { get; set; }

    [Column("cdfilial")]
    [Display(Name = "Código Filial")]
    public int Cdfilial { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // DADOS PESSOAIS
    // ═══════════════════════════════════════════════════════════════════

    [Column("nmcolab")]
    [StringLength(60)]
    [Display(Name = "Nome Completo")]
    public string Nmcolab { get; set; } = string.Empty;

    [Column("nmguerra")]
    [StringLength(15)]
    [Display(Name = "Nome de Guerra")]
    public string? Nmguerra { get; set; }

    [Column("dtnasc")]
    [Display(Name = "Data de Nascimento")]
    public DateTime? Dtnasc { get; set; }

    [Column("cdsexo")]
    [StringLength(1)]
    [Display(Name = "Sexo")]
    public string? Cdsexo { get; set; }

    [Column("cdestcivil")]
    [StringLength(1)]
    [Display(Name = "Estado Civil")]
    public string? Cdestcivil { get; set; }

    [Column("nmpaicolab")]
    [StringLength(60)]
    [Display(Name = "Nome do Pai")]
    public string? Nmpaicolab { get; set; }

    [Column("nmmaecolab")]
    [StringLength(60)]
    [Display(Name = "Nome da Mãe")]
    public string? Nmmaecolab { get; set; }

    [Column("cdtpsangue")]
    [StringLength(2)]
    [Display(Name = "Tipo Sanguíneo")]
    public string? Cdtpsangue { get; set; }

    [Column("cod_raca")]
    [Display(Name = "Raça/Cor")]
    public int? CodRaca { get; set; }

    [Column("cod_deficiente")]
    [StringLength(1)]
    [Display(Name = "Deficiência")]
    public string? CodDeficiente { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // DOCUMENTOS
    // ═══════════════════════════════════════════════════════════════════

    [Column("nocpf")]
    [StringLength(11)]
    [Display(Name = "CPF")]
    public string? Nocpf { get; set; }

    [Column("norg")]
    [StringLength(15)]
    [Display(Name = "RG")]
    public string? Norg { get; set; }

    [Column("sgorgrg")]
    [StringLength(20)]
    [Display(Name = "Órgão Emissor RG")]
    public string? Sgorgrg { get; set; }

    [Column("sgestrg")]
    [StringLength(2)]
    [Display(Name = "UF RG")]
    public string? Sgestrg { get; set; }

    [Column("dtrg")]
    [Display(Name = "Data Emissão RG")]
    public DateTime? Dtrg { get; set; }

    [Column("nopis")]
    [StringLength(11)]
    [Display(Name = "PIS")]
    public string? Nopis { get; set; }

    [Column("dtinscpis")]
    [Display(Name = "Data Inscrição PIS")]
    public DateTime? Dtinscpis { get; set; }

    [Column("nocartprof")]
    [StringLength(10)]
    [Display(Name = "CTPS Número")]
    public string? Nocartprof { get; set; }

    [Column("noserie")]
    [StringLength(5)]
    [Display(Name = "CTPS Série")]
    public string? Noserie { get; set; }

    [Column("sgestcart")]
    [StringLength(2)]
    [Display(Name = "CTPS UF")]
    public string? Sgestcart { get; set; }

    [Column("dtcartprof")]
    [Display(Name = "CTPS Data Emissão")]
    public DateTime? Dtcartprof { get; set; }

    [Column("notitelei")]
    [StringLength(12)]
    [Display(Name = "Título Eleitor")]
    public string? Notitelei { get; set; }

    [Column("nosecaotit")]
    [StringLength(4)]
    [Display(Name = "Seção Eleitoral")]
    public string? Nosecaotit { get; set; }

    [Column("nozonatit")]
    [StringLength(3)]
    [Display(Name = "Zona Eleitoral")]
    public string? Nozonatit { get; set; }

    [Column("sgesttit")]
    [StringLength(2)]
    [Display(Name = "UF Título")]
    public string? Sgesttit { get; set; }

    [Column("dttitelei")]
    [Display(Name = "Data Emissão Título")]
    public DateTime? Dttitelei { get; set; }

    [Column("nocartaosus")]
    [StringLength(20)]
    [Display(Name = "Cartão SUS")]
    public string? Nocartaosus { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // ENDEREÇO
    // ═══════════════════════════════════════════════════════════════════

    [Column("dcendereco")]
    [StringLength(60)]
    [Display(Name = "Endereço")]
    public string? Dcendereco { get; set; }

    [Column("end_numero")]
    [StringLength(10)]
    [Display(Name = "Número")]
    public string? EndNumero { get; set; }

    [Column("end_comp")]
    [StringLength(60)]
    [Display(Name = "Complemento")]
    public string? EndComp { get; set; }

    [Column("dcbairro")]
    [StringLength(40)]
    [Display(Name = "Bairro")]
    public string? Dcbairro { get; set; }

    [Column("nocep")]
    [StringLength(8)]
    [Display(Name = "CEP")]
    public string? Nocep { get; set; }

    [Column("sgestado")]
    [StringLength(2)]
    [Display(Name = "UF")]
    public string? Sgestado { get; set; }

    [Column("dcptreferencia")]
    [StringLength(100)]
    [Display(Name = "Ponto de Referência")]
    public string? Dcptreferencia { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // CONTATO
    // ═══════════════════════════════════════════════════════════════════

    [Column("noddd")]
    [StringLength(3)]
    [Display(Name = "DDD")]
    public string? Noddd { get; set; }

    [Column("notelefone")]
    [StringLength(30)]
    [Display(Name = "Telefone")]
    public string? Notelefone { get; set; }

    [Column("noddd2")]
    [StringLength(3)]
    [Display(Name = "DDD 2")]
    public string? Noddd2 { get; set; }

    [Column("notelefone2")]
    [StringLength(15)]
    [Display(Name = "Telefone 2")]
    public string? Notelefone2 { get; set; }

    [Column("noddd3")]
    [StringLength(2)]
    [Display(Name = "DDD 3")]
    public string? Noddd3 { get; set; }

    [Column("notelefone3")]
    [StringLength(30)]
    [Display(Name = "Telefone 3")]
    public string? Notelefone3 { get; set; }

    [Column("dcemail")]
    [StringLength(80)]
    [Display(Name = "E-mail")]
    public string? Dcemail { get; set; }

    [Column("emailalternativo")]
    [StringLength(60)]
    [Display(Name = "E-mail Alternativo")]
    public string? Emailalternativo { get; set; }

    [Column("dcramal")]
    [StringLength(9)]
    [Display(Name = "Ramal")]
    public string? Dcramal { get; set; }

    [Column("contato2")]
    [StringLength(60)]
    [Display(Name = "Contato 2")]
    public string? Contato2 { get; set; }

    [Column("contato3")]
    [StringLength(60)]
    [Display(Name = "Contato 3")]
    public string? Contato3 { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // DADOS FUNCIONAIS
    // ═══════════════════════════════════════════════════════════════════

    [Column("dtadmissao")]
    [Display(Name = "Data Admissão")]
    public DateTime Dtadmissao { get; set; }

    [Column("dtdemissao")]
    [Display(Name = "Data Demissão")]
    public DateTime? Dtdemissao { get; set; }

    [Column("dtaviso")]
    [Display(Name = "Data Aviso")]
    public DateTime? Dtaviso { get; set; }

    [Column("dthomologacao")]
    [Display(Name = "Data Homologação")]
    public DateTime? Dthomologacao { get; set; }

    [Column("tpcolab")]
    [Display(Name = "Tipo Colaborador")]
    public int? Tpcolab { get; set; }

    [Column("nohssem")]
    [Display(Name = "Horas Semanais")]
    public int? Nohssem { get; set; }

    [Column("nohsmes")]
    [Display(Name = "Horas Mensais")]
    public int? Nohsmes { get; set; }

    [Column("flfreq")]
    [Display(Name = "Controla Frequência")]
    public int? Flfreq { get; set; }

    [Column("flsindicat")]
    [Display(Name = "Sindicalizado")]
    public int? Flsindicat { get; set; }

    [Column("cdnivel")]
    [StringLength(5)]
    [Display(Name = "Nível Salarial")]
    public string? Cdnivel { get; set; }

    [Column("cdturma")]
    [StringLength(2)]
    [Display(Name = "Turma")]
    public string? Cdturma { get; set; }

    [Column("cdcolab")]
    [StringLength(6)]
    [Display(Name = "Código Colaborador")]
    public string? Cdcolab { get; set; }

    [Column("matsap")]
    [StringLength(30)]
    [Display(Name = "Matrícula SAP")]
    public string? Matsap { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // DADOS BANCÁRIOS - RECEBIMENTO
    // ═══════════════════════════════════════════════════════════════════

    [Column("noctarec")]
    [StringLength(13)]
    [Display(Name = "Conta Recebimento")]
    public string? Noctarec { get; set; }

    [Column("dvctarec")]
    [StringLength(2)]
    [Display(Name = "DV Conta")]
    public string? Dvctarec { get; set; }

    [Column("tpctarec")]
    [StringLength(1)]
    [Display(Name = "Tipo Conta")]
    public string? Tpctarec { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // DADOS BANCÁRIOS - FGTS
    // ═══════════════════════════════════════════════════════════════════

    [Column("cdbanfgts")]
    [StringLength(3)]
    [Display(Name = "Banco FGTS")]
    public string? Cdbanfgts { get; set; }

    [Column("cdagefgts")]
    [StringLength(4)]
    [Display(Name = "Agência FGTS")]
    public string? Cdagefgts { get; set; }

    [Column("noctafgts")]
    [StringLength(13)]
    [Display(Name = "Conta FGTS")]
    public string? Noctafgts { get; set; }

    [Column("dvagefgts")]
    [StringLength(1)]
    [Display(Name = "DV Agência FGTS")]
    public string? Dvagefgts { get; set; }

    [Column("cdopcfgts")]
    [StringLength(1)]
    [Display(Name = "Opção FGTS")]
    public string? Cdopcfgts { get; set; }

    [Column("dtopcfgts")]
    [Display(Name = "Data Opção FGTS")]
    public DateTime? Dtopcfgts { get; set; }

    [Column("vlsaldofgts")]
    [Display(Name = "Saldo FGTS")]
    public double? Vlsaldofgts { get; set; }

    [Column("flrecfgts")]
    [Display(Name = "Recolhe FGTS")]
    public int? Flrecfgts { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // CÓDIGOS LEGADOS (para compatibilidade)
    // ═══════════════════════════════════════════════════════════════════

    [Column("cdccusto")]
    [StringLength(5)]
    [Display(Name = "Código Centro Custo")]
    public string? Cdccusto { get; set; }

    [Column("cdcargo")]
    [StringLength(5)]
    [Display(Name = "Código Cargo")]
    public string? Cdcargo { get; set; }

    [Column("cdmunicip")]
    [StringLength(5)]
    [Display(Name = "Código Município")]
    public string? Cdmunicip { get; set; }

    [Column("cdmuninasc")]
    [StringLength(5)]
    [Display(Name = "Código Município Nasc.")]
    public string? Cdmuninasc { get; set; }

    [Column("dcmuninasc")]
    [StringLength(20)]
    [Display(Name = "Município Nascimento")]
    public string? Dcmuninasc { get; set; }

    [Column("sgestadonasc")]
    [StringLength(2)]
    [Display(Name = "UF Nascimento")]
    public string? Sgestadonasc { get; set; }

    [Column("cdinstruc")]
    [StringLength(2)]
    [Display(Name = "Código Instrução")]
    public string? Cdinstruc { get; set; }

    [Column("cdsituacao")]
    [StringLength(2)]
    [Display(Name = "Código Situação")]
    public string? Cdsituacao { get; set; }

    [Column("cdvincul")]
    [StringLength(2)]
    [Display(Name = "Código Vínculo")]
    public string? Cdvincul { get; set; }

    [Column("cdsindicat")]
    [StringLength(2)]
    [Display(Name = "Código Sindicato")]
    public string? Cdsindicat { get; set; }

    [Column("cdbanrec")]
    [StringLength(3)]
    [Display(Name = "Código Banco Receb.")]
    public string? Cdbanrec { get; set; }

    [Column("cdagerec")]
    [StringLength(4)]
    [Display(Name = "Código Agência Receb.")]
    public string? Cdagerec { get; set; }

    [Column("dvagerec")]
    [StringLength(1)]
    [Display(Name = "DV Agência")]
    public string? Dvagerec { get; set; }

    [Column("cdnacion")]
    [StringLength(2)]
    [Display(Name = "Nacionalidade")]
    public string? Cdnacion { get; set; }

    [Column("aachegpais")]
    [StringLength(4)]
    [Display(Name = "Ano Chegada País")]
    public string? Aachegpais { get; set; }

    [Column("cdcategori")]
    [StringLength(2)]
    [Display(Name = "Categoria")]
    public string? Cdcategori { get; set; }

    [Column("cdcausres")]
    [StringLength(2)]
    [Display(Name = "Causa Rescisão")]
    public string? Cdcausres { get; set; }

    [Column("cdtpvisto")]
    [StringLength(10)]
    [Display(Name = "Tipo Visto")]
    public string? Cdtpvisto { get; set; }

    [Column("dtvcapfext")]
    [Display(Name = "Validade Passaporte")]
    public DateTime? Dtvcapfext { get; set; }

    [Column("dtvrgext")]
    [Display(Name = "Validade RNE")]
    public DateTime? Dtvrgext { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // OUTROS CAMPOS
    // ═══════════════════════════════════════════════════════════════════

    [Column("cdempreg")]
    [StringLength(2)]
    [Display(Name = "Código Empregador")]
    public string? Cdempreg { get; set; }

    [Column("dttransf")]
    [Display(Name = "Data Transferência")]
    public DateTime? Dttransf { get; set; }

    [Column("nomatant")]
    [StringLength(8)]
    [Display(Name = "Matrícula Anterior")]
    public string? Nomatant { get; set; }

    [Column("flcontsin")]
    [StringLength(1)]
    [Display(Name = "Contribuição Sindical")]
    public string? Flcontsin { get; set; }

    [Column("nopessoa")]
    [StringLength(6)]
    [Display(Name = "Número Pessoa")]
    public string? Nopessoa { get; set; }

    [Column("dtultexper")]
    [Display(Name = "Data Últ. Experiência")]
    public DateTime? Dtultexper { get; set; }

    [Column("cdocorr")]
    [StringLength(2)]
    [Display(Name = "Código Ocorrência")]
    public string? Cdocorr { get; set; }

    [Column("dtultmov")]
    [Display(Name = "Data Última Movimentação")]
    public DateTime? Dtultmov { get; set; }

    [Column("cdusuario")]
    [StringLength(20)]
    [Display(Name = "Usuário")]
    public string? Cdusuario { get; set; }

    [Column("flcexperie")]
    [Display(Name = "Contrato Experiência")]
    public int? Flcexperie { get; set; }

    [Column("dtcexperie")]
    [Display(Name = "Data Experiência")]
    public DateTime? Dtcexperie { get; set; }

    [Column("flprevcomp")]
    [Display(Name = "Previdência Complementar")]
    public int? Flprevcomp { get; set; }

    [Column("cdramal_transp")]
    [StringLength(5)]
    [Display(Name = "Ramal Transporte")]
    public string? CdramalTransp { get; set; }

    [Column("cdgrupo_ppp")]
    [Display(Name = "Grupo PPP")]
    public int? CdgrupoPpp { get; set; }

    [Column("dtlimite_acesso")]
    [Display(Name = "Limite Acesso")]
    public DateTime? DtlimiteAcesso { get; set; }

    [Column("cdcarreira")]
    [Display(Name = "Carreira")]
    public int? Cdcarreira { get; set; }

    [Column("dtadmhis")]
    [Display(Name = "Data Admissão Histórica")]
    public DateTime? Dtadmhis { get; set; }

    [Column("dtvenccontr2")]
    [Display(Name = "Vencimento Contrato 2")]
    public DateTime? Dtvenccontr2 { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // FOREIGN KEYS (GUIDs) - Baseado nas FKs reais do banco SQL
    // ═══════════════════════════════════════════════════════════════════

    [Column("idcentrodecusto")]
    public Guid? Idcentrodecusto { get; set; }

    [Column("idcargo")]
    public Guid? Idcargo { get; set; }

    [Column("idmunicipionaturalidade")]
    public Guid? Idmunicipionaturalidade { get; set; }

    [Column("idmunicipioendereco")]
    public Guid? Idmunicipioendereco { get; set; }

    [Column("idgraudeinstrucao")]
    public Guid? Idgraudeinstrucao { get; set; }

    [Column("idsituacao")]
    public Guid? Idsituacao { get; set; }

    [Column("idvinculoempregaticio")]
    public Guid? Idvinculoempregaticio { get; set; }

    [Column("idsindicato")]
    public Guid? Idsindicato { get; set; }

    [Column("idbancorecebimento")]
    public Guid? Idbancorecebimento { get; set; }

    [Column("idagenciarecebimento")]
    public Guid? Idagenciarecebimento { get; set; }

    [Column("idfilial")]
    public Guid? Idfilial { get; set; }

    [Column("idmotivorescisao")]
    public Guid? Idmotivorescisao { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // NAVIGATION PROPERTIES - APENAS 1 POR FK!
    // Cada navegação tem [ForeignKey] apontando para a FK correspondente
    // ═══════════════════════════════════════════════════════════════════

    [ForeignKey(nameof(Idcentrodecusto))]
    public virtual CentroDeCusto? CentroDeCusto { get; set; }

    [ForeignKey(nameof(Idcargo))]
    public virtual Cargo? Cargo { get; set; }

    [ForeignKey(nameof(Idmunicipionaturalidade))]
    public virtual Municipio? MunicipioNaturalidade { get; set; }

    [ForeignKey(nameof(Idmunicipioendereco))]
    public virtual Municipio? MunicipioEndereco { get; set; }

    [ForeignKey(nameof(Idgraudeinstrucao))]
    public virtual GrauInstrucao? GrauInstrucao { get; set; }

    [ForeignKey(nameof(Idsituacao))]
    public virtual SituacaoColaborador? Situacao { get; set; }

    [ForeignKey(nameof(Idvinculoempregaticio))]
    public virtual VinculoEmpregaticio? VinculoEmpregaticio { get; set; }

    [ForeignKey(nameof(Idsindicato))]
    public virtual Sindicato? Sindicato { get; set; }

   // [ForeignKey(nameof(Idbancorecebimento))]
    //public virtual Banco? BancoRecebimento { get; set; }

    [ForeignKey(nameof(Idagenciarecebimento))]
    public virtual AgenciaBancaria? AgenciaRecebimento { get; set; }

 //   [ForeignKey(nameof(Idfilial))]
 //   public virtual Filial? Filial { get; set; }

    [ForeignKey(nameof(Idmotivorescisao))]
    public virtual MotivoRescisao? MotivoRescisao { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // NOTA IMPORTANTE: Não há FK idempresa (GUID) no banco!
    // A relação com Empresa é apenas por cdempresa (código legado)
    // Se precisar navegar para Empresa, configure manualmente no DbContext
    // ou use um serviço para buscar por código
    // ═══════════════════════════════════════════════════════════════════
}
