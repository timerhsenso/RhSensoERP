// Src/Core/MSO/Entities/MsoEmpresa.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_empresa")]
public class MsoEmpresa
{
    [Key]
    [Column("cod_empresa")]
    public int CodEmpresa { get; set; }

    [Column("rzsocial_empresa")]
    [StringLength(60)]
    [Required]
    public string RzsocialEmpresa { get; set; } = string.Empty;

    [Column("nfantasia_empresa")]
    [StringLength(30)]
    [Required]
    public string NfantasiaEmpresa { get; set; } = string.Empty;

    [Column("cnpj_cpf")]
    [StringLength(14)]
    public string? CnpjCpf { get; set; }

    [Column("desc_endereco")]
    [StringLength(60)]
    public string? DescEndereco { get; set; }

    [Column("desc_complemento")]
    [StringLength(60)]
    public string? DescComplemento { get; set; }

    [Column("cod_municipio")]
    [StringLength(5)]
    public string? CodMunicipio { get; set; }

    [Column("sigla_uf")]
    [StringLength(2)]
    public string? SiglaUf { get; set; }

    [Column("numero_cep")]
    [StringLength(8)]
    public string? NumeroCep { get; set; }

    [Column("nome_responsavel")]
    [StringLength(60)]
    public string? NomeResponsavel { get; set; }

    [Column("endereco_eletronico")]
    [StringLength(80)]
    public string? EnderecoEletronico { get; set; }

    [Column("ch_terceiro")]
    [StringLength(1)]
    [Required]
    public string ChTerceiro { get; set; } = "N";

    [Column("ch_ativo")]
    [StringLength(1)]
    [Required]
    public string ChAtivo { get; set; } = "S";

    [Column("cod_empreiteira")]
    public int? CodEmpreiteira { get; set; }

    [Column("dirlogotipo")]
    [StringLength(255)]
    public string? DirLogotipo { get; set; }

    [Column("telefone")]
    [StringLength(20)]
    public string? Telefone { get; set; }

    [Column("fax")]
    [StringLength(20)]
    public string? Fax { get; set; }

    [Column("site")]
    [StringLength(100)]
    public string? Site { get; set; }

    [Column("inscricao_estadual")]
    [StringLength(20)]
    public string? InscricaoEstadual { get; set; }

    [Column("inscricao_municipal")]
    [StringLength(20)]
    public string? InscricaoMunicipal { get; set; }

    [Column("data_criacao")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [Column("data_atualizacao")]
    public DateTime? DataAtualizacao { get; set; }

    [Column("usuario_criacao")]
    [StringLength(50)]
    public string? UsuarioCriacao { get; set; }

    [Column("usuario_atualizacao")]
    [StringLength(50)]
    public string? UsuarioAtualizacao { get; set; }

    // Relacionamentos
    public virtual ICollection<MsoEmpregado> Empregados { get; set; } = new List<MsoEmpregado>();

    // Propriedades calculadas
    [NotMapped]
    public bool EstaAtiva => ChAtivo == "S";

    [NotMapped]
    public bool EhTerceira => ChTerceiro == "S";

    [NotMapped]
    public string EnderecoCompleto
    {
        get
        {
            var endereco = new List<string>();

            if (!string.IsNullOrWhiteSpace(DescEndereco))
                endereco.Add(DescEndereco);

            if (!string.IsNullOrWhiteSpace(DescComplemento))
                endereco.Add(DescComplemento);

            var cidadeUf = new List<string>();
            if (!string.IsNullOrWhiteSpace(CodMunicipio))
                cidadeUf.Add(CodMunicipio);
            if (!string.IsNullOrWhiteSpace(SiglaUf))
                cidadeUf.Add(SiglaUf);

            if (cidadeUf.Any())
                endereco.Add(string.Join("/", cidadeUf));

            if (!string.IsNullOrWhiteSpace(NumeroCep))
                endereco.Add($"CEP: {NumeroCep}");

            return string.Join(", ", endereco);
        }
    }

    [NotMapped]
    public string TipoEmpresa => EhTerceira ? "Terceira" : "Própria";

    [NotMapped]
    public string StatusDescricao => EstaAtiva ? "Ativa" : "Inativa";

    [NotMapped]
    public bool TemLogotipo => !string.IsNullOrWhiteSpace(DirLogotipo);

    [NotMapped]
    public int TotalEmpregados => Empregados?.Count(e => e.EstaAtivo) ?? 0;

    // Métodos de negócio
    public void AtivarEmpresa(string usuario)
    {
        ChAtivo = "S";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void InativarEmpresa(string usuario)
    {
        ChAtivo = "N";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void DefinirComoTerceira(string usuario)
    {
        ChTerceiro = "S";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void DefinirComoPropria(string usuario)
    {
        ChTerceiro = "N";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarContato(string telefone, string email, string site, string usuario)
    {
        Telefone = telefone;
        EnderecoEletronico = email;
        Site = site;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarEndereco(string endereco, string complemento, string municipio,
                                 string uf, string cep, string usuario)
    {
        DescEndereco = endereco;
        DescComplemento = complemento;
        CodMunicipio = municipio;
        SiglaUf = uf;
        NumeroCep = cep;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarLogotipo(string caminhoLogotipo, string usuario)
    {
        DirLogotipo = caminhoLogotipo;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    // Validações
    public bool IsCnpjValido()
    {
        if (string.IsNullOrWhiteSpace(CnpjCpf)) return true;
        return CnpjCpf.Length == 14 && CnpjCpf.All(char.IsDigit);
    }

    public bool IsEmailValido()
    {
        if (string.IsNullOrWhiteSpace(EnderecoEletronico)) return true;
        return EnderecoEletronico.Contains("@") && EnderecoEletronico.Contains(".");
    }

    public bool TemDadosObrigatorios()
    {
        return !string.IsNullOrWhiteSpace(RzsocialEmpresa) &&
               !string.IsNullOrWhiteSpace(NfantasiaEmpresa);
    }

    public override string ToString()
    {
        return $"{CodEmpresa} - {NfantasiaEmpresa} ({StatusDescricao})";
    }
}