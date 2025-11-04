namespace RhSensoERP.Shared.Core.Errors;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
