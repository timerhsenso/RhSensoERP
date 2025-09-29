using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Application.Auth;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Core.Shared;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers.SEG;

/// <summary>
/// Controller para gerenciamento de usuários do sistema
/// Implementa controle de permissões granular por ação (IAEC)
/// </summary>
[ApiController]
[Route("api/v1/seg/usuarios")]
[Authorize] // ✅ Requer autenticação JWT
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "SEG")]

public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(AppDbContext context, ILogger<UsuariosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ========================================
    // CONSULTAR (C) - Listar usuários
    // ========================================

    /// <summary>
    /// Lista todos os usuários ativos do sistema
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.USUARIOS.C (Consultar)
    /// </remarks>
    [HttpGet]
    [HasPermission("SEG.SEG_USUARIOS.C")]
    [ProducesResponseType(typeof(ApiResponse<List<UsuarioDto>>), 200)]
    [ProducesResponseType(403)] // Forbidden se não tiver permissão
    public async Task<ActionResult<ApiResponse<List<UsuarioDto>>>> ListarUsuarios(
        [FromQuery] bool incluirInativos = false)
    {
        var query = _context.Set<User>().AsNoTracking();

        if (!incluirInativos)
            query = query.Where(u => u.FlAtivo == 'S');

        var usuarios = await query
            .OrderBy(u => u.DcUsuario)
            .Select(u => new UsuarioDto
            {
                CdUsuario = u.CdUsuario,
                DcUsuario = u.DcUsuario,
                EmailUsuario = u.EmailUsuario,
                TpUsuario = u.TpUsuario,
                FlAtivo = u.FlAtivo,
                CdEmpresa = u.CdEmpresa,
                CdFilial = u.CdFilial
            })
            .ToListAsync();

        _logger.LogInformation("Listagem de usuários acessada por {User}. Total: {Count}",
            User.Identity?.Name, usuarios.Count);

        return Ok(ApiResponse<List<UsuarioDto>>.Ok(usuarios));
    }

    // ========================================
    // CONSULTAR (C) - Obter usuário por ID
    // ========================================

    /// <summary>
    /// Obtém detalhes de um usuário específico
    /// </summary>
    /// <param name="cdusuario">Código do usuário</param>
    /// <remarks>
    /// Requer permissão: SEG.USUARIOS.C (Consultar)
    /// </remarks>
    [HttpGet("{cdusuario}")]
    [HasPermission("SEG.SEG_USUARIOS.C")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDetalheDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<UsuarioDetalheDto>>> ObterUsuario(string cdusuario)
    {
        var usuario = await _context.Set<User>()
            .AsNoTracking()
            .Where(u => u.CdUsuario == cdusuario)
            .Select(u => new UsuarioDetalheDto
            {
                CdUsuario = u.CdUsuario,
                DcUsuario = u.DcUsuario,
                EmailUsuario = u.EmailUsuario,
                TpUsuario = u.TpUsuario,
                FlAtivo = u.FlAtivo,
                CdEmpresa = u.CdEmpresa,
                CdFilial = u.CdFilial,
                NoMatric = u.NoMatric,
                NmImpCche = u.NmImpCche
            })
            .FirstOrDefaultAsync();

        if (usuario == null)
        {
            _logger.LogWarning("Usuário {CdUsuario} não encontrado", cdusuario);
            return NotFound(ApiResponse<UsuarioDetalheDto>.Fail("Usuário não encontrado"));
        }

        return Ok(ApiResponse<UsuarioDetalheDto>.Ok(usuario));
    }

    // ========================================
    // INCLUIR (I) - Criar novo usuário
    // ========================================

    /// <summary>
    /// Cria um novo usuário no sistema
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.USUARIOS.I (Incluir)
    /// </remarks>
    [HttpPost]
    [HasPermission("SEG.SEG_USUARIOS.I")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> CriarUsuario(
        [FromBody] CriarUsuarioRequest request)
    {
        // Validações de negócio
        if (await _context.Set<User>().AnyAsync(u => u.CdUsuario == request.CdUsuario))
        {
            return BadRequest(ApiResponse<object>.Fail("Código de usuário já existe"));
        }

        if (!string.IsNullOrEmpty(request.EmailUsuario) &&
            await _context.Set<User>().AnyAsync(u => u.EmailUsuario == request.EmailUsuario))
        {
            return BadRequest(ApiResponse<object>.Fail("Email já cadastrado"));
        }

        // Criar novo usuário
        var novoUsuario = new User
        {
            Id = Guid.NewGuid(),
            CdUsuario = request.CdUsuario,
            DcUsuario = request.DcUsuario,
            SenhaUser = request.Senha, // ⚠️ TODO: Hash em produção
            EmailUsuario = request.EmailUsuario,
            TpUsuario = request.TpUsuario,
            FlAtivo = 'S',
            CdEmpresa = request.CdEmpresa,
            CdFilial = request.CdFilial,
            NoUser = await ObterProximoNoUser(),
            NormalizedUsername = request.CdUsuario.ToUpperInvariant()
        };

        _context.Set<User>().Add(novoUsuario);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuário {CdUsuario} criado por {User}",
            novoUsuario.CdUsuario, User.Identity?.Name);

        var resultado = new UsuarioDto
        {
            CdUsuario = novoUsuario.CdUsuario,
            DcUsuario = novoUsuario.DcUsuario,
            EmailUsuario = novoUsuario.EmailUsuario,
            TpUsuario = novoUsuario.TpUsuario,
            FlAtivo = novoUsuario.FlAtivo,
            CdEmpresa = novoUsuario.CdEmpresa,
            CdFilial = novoUsuario.CdFilial
        };

        return CreatedAtAction(
            nameof(ObterUsuario),
            new { cdusuario = novoUsuario.CdUsuario },
            ApiResponse<UsuarioDto>.Ok(resultado, "Usuário criado com sucesso"));
    }

    // ========================================
    // ALTERAR (A) - Atualizar usuário
    // ========================================

    /// <summary>
    /// Atualiza dados de um usuário existente
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.USUARIOS.A (Alterar)
    /// </remarks>
    [HttpPut("{cdusuario}")]
    [HasPermission("SEG.SEG_USUARIOS.A")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> AtualizarUsuario(
        string cdusuario,
        [FromBody] AtualizarUsuarioRequest request)
    {
        var usuario = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.CdUsuario == cdusuario);

        if (usuario == null)
        {
            return NotFound(ApiResponse<UsuarioDto>.Fail("Usuário não encontrado"));
        }

        // Validar email único (se alterado)
        if (!string.IsNullOrEmpty(request.EmailUsuario) &&
            request.EmailUsuario != usuario.EmailUsuario &&
            await _context.Set<User>().AnyAsync(u => u.EmailUsuario == request.EmailUsuario))
        {
            return BadRequest(ApiResponse<object>.Fail("Email já cadastrado para outro usuário"));
        }

        // Atualizar campos
        usuario.DcUsuario = request.DcUsuario;
        usuario.EmailUsuario = request.EmailUsuario;
        usuario.CdEmpresa = request.CdEmpresa;
        usuario.CdFilial = request.CdFilial;

        if (!string.IsNullOrEmpty(request.NovaSenha))
        {
            usuario.SenhaUser = request.NovaSenha; // ⚠️ TODO: Hash em produção
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuário {CdUsuario} atualizado por {User}",
            cdusuario, User.Identity?.Name);

        var resultado = new UsuarioDto
        {
            CdUsuario = usuario.CdUsuario,
            DcUsuario = usuario.DcUsuario,
            EmailUsuario = usuario.EmailUsuario,
            TpUsuario = usuario.TpUsuario,
            FlAtivo = usuario.FlAtivo,
            CdEmpresa = usuario.CdEmpresa,
            CdFilial = usuario.CdFilial
        };

        return Ok(ApiResponse<UsuarioDto>.Ok(resultado, "Usuário atualizado com sucesso"));
    }

    // ========================================
    // EXCLUIR (E) - Desativar usuário
    // ========================================

    /// <summary>
    /// Desativa um usuário (soft delete)
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.USUARIOS.E (Excluir)
    /// </remarks>
    [HttpDelete("{cdusuario}")]
    [HasPermission("SEG.SEG_USUARIOS.E")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<object>>> DesativarUsuario(string cdusuario)
    {
        var usuario = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.CdUsuario == cdusuario);

        if (usuario == null)
        {
            return NotFound(ApiResponse<object>.Fail("Usuário não encontrado"));
        }

        // Soft delete - apenas marca como inativo
        usuario.FlAtivo = 'N';
        await _context.SaveChangesAsync();

        _logger.LogWarning("Usuário {CdUsuario} desativado por {User}",
            cdusuario, User.Identity?.Name);

        return Ok(ApiResponse<object>.Ok(null, "Usuário desativado com sucesso"));
    }

    // ========================================
    // AÇÃO ADICIONAL - Reativar usuário
    // ========================================

    /// <summary>
    /// Reativa um usuário previamente desativado
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.USUARIOS.A (Alterar)
    /// </remarks>
    [HttpPatch("{cdusuario}/reativar")]
    [HasPermission("SEG.SEG_USUARIOS.A")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<object>>> ReativarUsuario(string cdusuario)
    {
        var usuario = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.CdUsuario == cdusuario);

        if (usuario == null)
        {
            return NotFound(ApiResponse<object>.Fail("Usuário não encontrado"));
        }

        usuario.FlAtivo = 'S';
        await _context.SaveChangesAsync();

        _logger.LogInformation("Usuário {CdUsuario} reativado por {User}",
            cdusuario, User.Identity?.Name);

        return Ok(ApiResponse<object>.Ok(null, "Usuário reativado com sucesso"));
    }

    // ========================================
    // AÇÃO ADICIONAL - Resetar senha
    // ========================================

    /// <summary>
    /// Reseta a senha de um usuário
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.USUARIOS.A (Alterar)
    /// </remarks>
    [HttpPost("{cdusuario}/resetar-senha")]
    [HasPermission("SEG.SEG_USUARIOS.A")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<object>>> ResetarSenha(
        string cdusuario,
        [FromBody] ResetarSenhaRequest request)
    {
        var usuario = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.CdUsuario == cdusuario);

        if (usuario == null)
        {
            return NotFound(ApiResponse<object>.Fail("Usuário não encontrado"));
        }

        usuario.SenhaUser = request.NovaSenha; // ⚠️ TODO: Hash em produção
        await _context.SaveChangesAsync();

        _logger.LogWarning("Senha do usuário {CdUsuario} resetada por {User}",
            cdusuario, User.Identity?.Name);

        return Ok(ApiResponse<object>.Ok(null, "Senha resetada com sucesso"));
    }

    // ========================================
    // MÉTODOS AUXILIARES PRIVADOS
    // ========================================

    private async Task<int> ObterProximoNoUser()
    {
        var maxNoUser = await _context.Set<User>()
            .MaxAsync(u => (int?)u.NoUser) ?? 0;

        return maxNoUser + 1;
    }
}

// ========================================
// DTOs
// ========================================

public class UsuarioDto
{
    public string CdUsuario { get; set; } = string.Empty;
    public string DcUsuario { get; set; } = string.Empty;
    public string? EmailUsuario { get; set; }
    public char TpUsuario { get; set; }
    public char FlAtivo { get; set; }
    public int? CdEmpresa { get; set; }
    public int? CdFilial { get; set; }
}

public class UsuarioDetalheDto : UsuarioDto
{
    public string? NoMatric { get; set; }
    public string? NmImpCche { get; set; }
}

public class CriarUsuarioRequest
{
    public string CdUsuario { get; set; } = string.Empty;
    public string DcUsuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string? EmailUsuario { get; set; }
    public char TpUsuario { get; set; } = 'U';
    public int? CdEmpresa { get; set; }
    public int? CdFilial { get; set; }
}

public class AtualizarUsuarioRequest
{
    public string DcUsuario { get; set; } = string.Empty;
    public string? EmailUsuario { get; set; }
    public int? CdEmpresa { get; set; }
    public int? CdFilial { get; set; }
    public string? NovaSenha { get; set; }
}

public class ResetarSenhaRequest
{
    public string NovaSenha { get; set; } = string.Empty;
}