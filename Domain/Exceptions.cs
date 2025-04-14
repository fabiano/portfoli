namespace Portfoli.Domain;

public class NotFoundException(string message) : Exception(message);

public class InvalidDomainOperationException(string message) : Exception(message);

public class PortfolioNotFoundException(Guid PortfolioId) : NotFoundException($"Portfolio {PortfolioId} not found.");

public class AssetNotFoundException(string Exchange, string Ticker) : NotFoundException($"Asset {Ticker} on {Exchange} not found.");

public class HoldingNotFoundException(Guid PortfolioId, Guid HoldingId) : NotFoundException($"Holding {HoldingId} in portfolio {PortfolioId} not found.");

public class TransactionNotFoundException(Guid PortfolioId, Guid HoldingId, Guid TransactionId) : NotFoundException($"Transaction {TransactionId} in holding {HoldingId} in portfolio {PortfolioId} not found.");
