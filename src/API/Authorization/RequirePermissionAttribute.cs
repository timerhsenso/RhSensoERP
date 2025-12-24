// ============================================================================
// ARQUIVO CORRIGIDO - FASE 2: src/API/Authorization/RequirePermissionAttribute.cs
// ============================================================================
// CORREÇÃO: Removido "sealed" da classe base para permitir herança
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace RhSensoERP.API.Authorization;

/// <summary>
/// Atributo de autorização customizado para verificar permissões legadas.
/// Verifica se o usuário tem permissão para executar uma ação em uma função.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAuthorizationFilter // ✅ REMOVIDO "sealed"
{
    private readonly string _cdFuncao;
    private readonly char _acao;

    /// <summary>
    /// Construtor do atributo de permissão.
    /// </summary>
    /// <param name="cdFuncao">Código da função (ex: "CADUSER")</param>
    /// <param name="acao">Ação requerida: I=Incluir, A=Alterar, E=Excluir, C=Consultar</param>
    public RequirePermissionAttribute(string cdFuncao, char acao)
    {
        _cdFuncao = cdFuncao ?? throw new ArgumentNullException(nameof(cdFuncao));
        _acao = char.ToUpper(acao);

        if (_acao != 'I' && _acao != 'A' && _acao != 'E' && _acao != 'C')
        {
            throw new ArgumentException(
                "Ação deve ser I (Incluir), A (Alterar), E (Excluir) ou C (Consultar)",
                nameof(acao));
        }
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Verificar se o usuário está autenticado
        if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = "UNAUTHORIZED",
                message = "Usuário não autenticado."
            });
            return;
        }

        // Obter permissões dos claims
        var permissoes = context.HttpContext.User
            .FindAll("permissao")
            .Select(c => c.Value)
            .ToList();

        // Verificar se tem a permissão necessária
        // Formato do claim: "FUNCAO:ACOES" (ex: "CADUSER:IAEC")
        var temPermissao = permissoes.Any(p =>
        {
            var parts = p.Split(':');
            if (parts.Length != 2)
                return false;

            var funcao = parts[0];
            var acoes = parts[1];

            return funcao.Equals(_cdFuncao, StringComparison.OrdinalIgnoreCase) &&
                   acoes.Contains(_acao);
        });

        if (!temPermissao)
        {
            var logger = context.HttpContext.RequestServices
                .GetService<ILogger<RequirePermissionAttribute>>();

            var cdUsuario = context.HttpContext.User.FindFirstValue("cdusuario");

            logger?.LogWarning(
                "🚫 AUTORIZAÇÃO: Usuário {CdUsuario} sem permissão para {Acao} em {Funcao}",
                cdUsuario,
                _acao,
                _cdFuncao);

            context.Result = new ForbidResult();
        }
    }
}

/// <summary>
/// Atributos de atalho para ações comuns.
/// </summary>
public sealed class RequireIncluirAttribute : RequirePermissionAttribute
{
    public RequireIncluirAttribute(string cdFuncao) : base(cdFuncao, 'I') { }
}

public sealed class RequireAlterarAttribute : RequirePermissionAttribute
{
    public RequireAlterarAttribute(string cdFuncao) : base(cdFuncao, 'A') { }
}

public sealed class RequireExcluirAttribute : RequirePermissionAttribute
{
    public RequireExcluirAttribute(string cdFuncao) : base(cdFuncao, 'E') { }
}

public sealed class RequireConsultarAttribute : RequirePermissionAttribute
{
    public RequireConsultarAttribute(string cdFuncao) : base(cdFuncao, 'C') { }
}
