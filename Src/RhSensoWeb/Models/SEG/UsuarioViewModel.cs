using System.ComponentModel.DataAnnotations;

namespace RhSensoWeb.Models.SEG;

/// <summary>
/// ViewModel para usuários do sistema
/// </summary>
public class UsuarioViewModel
{
    [Display(Name = "Código")]
    public string CdUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome do usuário é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    [Display(Name = "Nome")]
    public string DcUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres")]
    [Display(Name = "Email")]
    public string EmailUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string? Senha { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Senha")]
    [Compare("Senha", ErrorMessage = "As senhas não coincidem")]
    public string? ConfirmarSenha { get; set; }

    [Required(ErrorMessage = "O tipo de usuário é obrigatório")]
    [Display(Name = "Tipo de Usuário")]
    public string TpUsuario { get; set; } = string.Empty;

    [Display(Name = "Ativo")]
    public char FlAtivo { get; set; } = 'S';

    [Display(Name = "Empresa")]
    public string? CdEmpresa { get; set; }

    [Display(Name = "Filial")]
    public string? CdFilial { get; set; }

    [Display(Name = "ID SaaS")]
    public string? IdSaas { get; set; }

    [Display(Name = "Data de Criação")]
    public DateTime? DtCriacao { get; set; }

    [Display(Name = "Último Login")]
    public DateTime? DtUltimoLogin { get; set; }

    [Display(Name = "Grupos")]
    public List<string> GruposSelecionados { get; set; } = new();

    [Display(Name = "Observações")]
    [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }

    /// <summary>
    /// Indica se é uma edição (não criar nova senha)
    /// </summary>
    public bool IsEdicao => !string.IsNullOrEmpty(CdUsuario);

    /// <summary>
    /// Status formatado para exibição
    /// </summary>
    public string StatusFormatado => FlAtivo == 'S' ? "Ativo" : "Inativo";

    /// <summary>
    /// CSS class para status
    /// </summary>
    public string StatusCssClass => FlAtivo == 'S' ? "badge-success" : "badge-danger";
}

/// <summary>
/// DTO para listagem de usuários
/// </summary>
public class UsuarioListDto
{
    public string CdUsuario { get; set; } = string.Empty;
    public string DcUsuario { get; set; } = string.Empty;
    public string EmailUsuario { get; set; } = string.Empty;
    public string TpUsuario { get; set; } = string.Empty;
    public char FlAtivo { get; set; }
    public string? CdEmpresa { get; set; }
    public string? CdFilial { get; set; }
    public DateTime? DtUltimoLogin { get; set; }
    public List<string> Grupos { get; set; } = new();
}

/// <summary>
/// DTO para detalhes do usuário
/// </summary>
public class UsuarioDetalheDto : UsuarioListDto
{
    public string? IdSaas { get; set; }
    public DateTime? DtCriacao { get; set; }
    public string? Observacoes { get; set; }
    public List<PermissaoUsuarioDto> Permissoes { get; set; } = new();
}

/// <summary>
/// DTO para permissões do usuário
/// </summary>
public class PermissaoUsuarioDto
{
    public string CdSistema { get; set; } = string.Empty;
    public string DcSistema { get; set; } = string.Empty;
    public string CdFuncao { get; set; } = string.Empty;
    public string DcFuncao { get; set; } = string.Empty;
    public string CdBotao { get; set; } = string.Empty;
    public string DcBotao { get; set; } = string.Empty;
    public bool CanInclude { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanConsult { get; set; }
}

/// <summary>
/// Filtros para pesquisa de usuários
/// </summary>
public class UsuarioFiltroDto
{
    [Display(Name = "Nome")]
    public string? Nome { get; set; }

    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Tipo")]
    public string? Tipo { get; set; }

    [Display(Name = "Status")]
    public char? Status { get; set; }

    [Display(Name = "Empresa")]
    public string? Empresa { get; set; }

    [Display(Name = "Filial")]
    public string? Filial { get; set; }

    [Display(Name = "Grupo")]
    public string? Grupo { get; set; }
}
