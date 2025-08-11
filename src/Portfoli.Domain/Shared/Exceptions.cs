namespace Portfoli.Domain.Shared;

public class InvalidDomainOperationException(string message) : Exception(message);
