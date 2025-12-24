// RhSensoERP.Shared.Core — AuditableEntity
// Finalidade: Fornecer propriedades e operações padrão de auditoria (created/modified) para entidades.
// Uso: Herde nas entidades que precisam registrar usuário e data de criação/alteração.
// Observações: Interceptors de Infra podem preencher automaticamente usando IDateTimeProvider/ICurrentUser.

using System;

namespace RhSensoERP.Shared.Core.Primitives;

/// <summary>
/// Entidade base com metadados de auditoria.
/// </summary>
/// <remarks>
/// <para><b>Quando usar:</b> sempre que precisar rastrear quem criou/alterou e quando.</para>
/// <para><b>Integração:</b> normalmente preenchida por interceptors na camada Infrastructure.</para>
/// </remarks>
public abstract class AuditableEntity
{
    /// <summary>Usuário que criou o registro.</summary>
    public string? CreatedBy { get; protected set; }

    /// <summary>Data/hora UTC da criação.</summary>
    public DateTime CreatedOn { get; protected set; }

    /// <summary>Usuário que realizou a última alteração.</summary>
    public string? ModifiedBy { get; protected set; }

    /// <summary>Data/hora UTC da última alteração.</summary>
    public DateTime? ModifiedOn { get; protected set; }

    /// <summary>Define os metadados de criação.</summary>
    public void SetCreated(string? user, DateTime nowUtc)
    {
        CreatedBy = user;
        CreatedOn = nowUtc;
    }

    /// <summary>Define os metadados de modificação.</summary>
    public void SetModified(string? user, DateTime nowUtc)
    {
        ModifiedBy = user;
        ModifiedOn = nowUtc;
    }
}
