// RhSensoERP.Shared.Core — Maybe<T>
// Finalidade: Representar valor opcional sem recorrer a null “solto”.
// Uso: Retornos que podem ou não conter valor, evitando checagens de null dispersas.

using System;

namespace RhSensoERP.Shared.Core.Primitives;

/// <summary>
/// Tipo opcional (Option) para evitar retorno de valores nulos sem semântica.
/// </summary>
public readonly struct Maybe<T>
{
    private readonly T _value;

    /// <summary>Indica se há valor presente.</summary>
    public bool HasValue { get; }

    /// <summary>
    /// Valor quando presente; lança exceção se não houver.
    /// </summary>
    public T Value => HasValue ? _value : throw new InvalidOperationException("No value present.");

    private Maybe(T value)
    {
        _value = value;
        HasValue = true;
    }

    /// <summary>None — representa ausência de valor.</summary>
    public static Maybe<T> None() => default;

    /// <summary>Cria um Maybe a partir de um valor (None se null).</summary>
    public static Maybe<T> From(T value) => value is null ? default : new Maybe<T>(value);
}
