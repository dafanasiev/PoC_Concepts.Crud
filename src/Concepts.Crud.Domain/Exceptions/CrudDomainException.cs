namespace Concepts.Crud.Domain.Exceptions;

public class CrudDomainException: Exception
{
    public CrudDomainException()
    { }

    public CrudDomainException(string message)
        : base(message)
    { }

    public CrudDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}