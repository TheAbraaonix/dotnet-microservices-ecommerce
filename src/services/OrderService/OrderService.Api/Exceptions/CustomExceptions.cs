namespace PedidoService.Exceptions;

/// <summary>
/// Thrown when a domain rule is violated.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown when a requested resource is not found.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string resourceName, object id)
        : base($"Resource '{resourceName}' with ID '{id}' was not found") { }
}
