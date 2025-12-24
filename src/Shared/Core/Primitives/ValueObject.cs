namespace RhSensoERP.Shared.Core.Primitives;

/// <summary>
/// Classe base para Value Objects.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Operador de igualdade.
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Operador de desigualdade.
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !Equals(left, right);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var valueObject = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <inheritdoc/>
    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }

    /// <summary>
    /// Obtém os componentes de igualdade do value object.
    /// </summary>
    /// <returns>Componentes usados para comparação de igualdade.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();
}
