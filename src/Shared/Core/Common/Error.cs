// =============================================================================
// RHSENSOERP - SHARED CORE COMMON
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Core/Common/Error.cs
// Classe de erro para Result Pattern
// =============================================================================

namespace RhSensoERP.Shared.Core.Common;

/// <summary>
/// Representa um erro em uma operação.
/// Usado em conjunto com Result<T> para implementar o Result Pattern.
/// </summary>
public sealed class Error : IEquatable<Error>
{
    /// <summary>
    /// Código único do erro (ex: "USER_NOT_FOUND", "INVALID_EMAIL").
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Mensagem descritiva do erro.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Tipo do erro (para categorização e tratamento).
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Construtor privado (use os métodos estáticos).
    /// </summary>
    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    // =========================================================================
    // FÁBRICAS DE ERRO
    // =========================================================================

    /// <summary>
    /// Erro genérico de falha (500 Internal Server Error).
    /// Use quando algo inesperado aconteceu.
    /// </summary>
    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    /// <summary>
    /// Erro de recurso não encontrado (404 Not Found).
    /// Use quando um registro/entidade não existe.
    /// </summary>
    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    /// <summary>
    /// Erro de validação (400 Bad Request).
    /// Use quando os dados de entrada são inválidos.
    /// </summary>
    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    /// <summary>
    /// Erro de conflito (409 Conflict).
    /// Use quando há violação de constraint/regra de negócio.
    /// </summary>
    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    /// <summary>
    /// Erro de não autorizado (401 Unauthorized).
    /// Use quando o usuário não está autenticado ou o token é inválido.
    /// </summary>
    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    /// <summary>
    /// Erro de acesso proibido (403 Forbidden).
    /// Use quando o usuário está autenticado mas não tem permissão.
    /// </summary>
    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    /// <summary>
    /// Erro de operação não permitida (405 Method Not Allowed).
    /// Use quando a operação não é suportada no estado atual.
    /// </summary>
    public static Error NotAllowed(string code, string message) =>
        new(code, message, ErrorType.NotAllowed);

    /// <summary>
    /// Erro de dados inválidos (422 Unprocessable Entity).
    /// Use quando a sintaxe está correta mas a semântica é inválida.
    /// </summary>
    public static Error Invalid(string code, string message) =>
        new(code, message, ErrorType.Invalid);

    // =========================================================================
    // ERRO PADRÃO (NONE)
    // =========================================================================

    /// <summary>
    /// Representa a ausência de erro (usado em Result<T>.Success).
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    // =========================================================================
    // EQUALITY & COMPARISON
    // =========================================================================

    public bool Equals(Error? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Code == other.Code && Type == other.Type;
    }

    public override bool Equals(object? obj) =>
        obj is Error error && Equals(error);

    public override int GetHashCode() =>
        HashCode.Combine(Code, Type);

    public static bool operator ==(Error? left, Error? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Error? left, Error? right) =>
        !(left == right);

    public override string ToString() =>
        $"[{Type}] {Code}: {Message}";
}

/// <summary>
/// Tipos de erro suportados.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Sem erro (operação bem-sucedida).
    /// </summary>
    None = 0,

    /// <summary>
    /// Erro genérico/inesperado (500).
    /// </summary>
    Failure = 1,

    /// <summary>
    /// Recurso não encontrado (404).
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// Validação de entrada falhou (400).
    /// </summary>
    Validation = 3,

    /// <summary>
    /// Conflito de constraint/regra (409).
    /// </summary>
    Conflict = 4,

    /// <summary>
    /// Não autorizado - falta autenticação (401).
    /// </summary>
    Unauthorized = 5,

    /// <summary>
    /// Acesso proibido - sem permissão (403).
    /// </summary>
    Forbidden = 6,

    /// <summary>
    /// Operação não permitida (405).
    /// </summary>
    NotAllowed = 7,

    /// <summary>
    /// Dados semanticamente inválidos (422).
    /// </summary>
    Invalid = 8
}
