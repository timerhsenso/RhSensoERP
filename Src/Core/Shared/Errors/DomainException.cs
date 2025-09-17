namespace RhSensoERP.Core.Shared.Errors;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
